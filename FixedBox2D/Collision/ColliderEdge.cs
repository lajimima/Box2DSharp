using System;
using System.Buffers;
using System.Diagnostics;
using TrueSync;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;

namespace FixedBox2D.Collision
{
    public static partial class CollisionUtils
    {
        /// <summary>
        ///     Compute contact points for edge versus circle.
        ///     This accounts for edge connectivity.
        ///     计算边缘和圆的碰撞点
        /// </summary>
        /// <param name="manifold"></param>
        /// <param name="edgeA"></param>
        /// <param name="xfA"></param>
        /// <param name="circleB"></param>
        /// <param name="xfB"></param>
        public static void CollideEdgeAndCircle(
            ref Manifold manifold,
            EdgeShape edgeA,
            in Transform xfA,
            CircleShape circleB,
            in Transform xfB)
        {
            manifold.PointCount = 0;

            // Compute circle in frame of edge
            // 在边缘形状的外框处理圆形
            var Q = MathUtils.MulT(xfA, MathUtils.Mul(xfB, circleB.Position));

            TSVector2 A = edgeA.Vertex1, B = edgeA.Vertex2;
            var e = B - A;

            // Normal points to the right for a CCW winding
            var n = new TSVector2(e.Y, -e.X);
            var offset = TSVector2.Dot(n, Q - A);

            var oneSided = edgeA.OneSided;
            if (oneSided && offset < FP.Zero)
            {
                return;
            }

            // Barycentric coordinates
            // 质心坐标
            var u = TSVector2.Dot(e, B - Q);
            var v = TSVector2.Dot(e, Q - A);

            var radius = edgeA.Radius + circleB.Radius;

            var cf = new ContactFeature {IndexB = 0, TypeB = (byte)ContactFeature.FeatureType.Vertex};

            // Region A
            if (v <= FP.Zero)
            {
                var P = A;
                var d = Q - P;
                var dd = TSVector2.Dot(d, d);
                if (dd > radius * radius)
                {
                    return;
                }

                // Is there an edge connected to A?
                if (edgeA.OneSided)
                {
                    var A1 = edgeA.Vertex0;
                    var B1 = A;
                    var e1 = B1 - A1;
                    var u1 = TSVector2.Dot(e1, B1 - Q);

                    // Is the circle in Region AB of the previous edge?
                    if (u1 > FP.Zero)
                    {
                        return;
                    }
                }

                cf.IndexA = 0;
                cf.TypeA = (byte)ContactFeature.FeatureType.Vertex;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.Circles;
                manifold.LocalNormal.SetZero();
                manifold.LocalPoint = P;
                ref var point = ref manifold.Points.Value0;
                point.Id.Key = 0;
                point.Id.ContactFeature = cf;
                point.LocalPoint = circleB.Position;
                return;
            }

            // Region B
            if (u <= FP.Zero)
            {
                var P = B;
                var d = Q - P;
                var dd = TSVector2.Dot(d, d);
                if (dd > radius * radius)
                {
                    return;
                }

                // Is there an edge connected to B?
                if (edgeA.OneSided)
                {
                    var B2 = edgeA.Vertex3;
                    var A2 = B;
                    var e2 = B2 - A2;
                    var v2 = TSVector2.Dot(e2, Q - A2);

                    // Is the circle in Region AB of the next edge?
                    if (v2 > FP.Zero)
                    {
                        return;
                    }
                }

                cf.IndexA = 1;
                cf.TypeA = (byte)ContactFeature.FeatureType.Vertex;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.Circles;
                manifold.LocalNormal.SetZero();
                manifold.LocalPoint = P;
                ref var point = ref manifold.Points.Value0;
                point.Id.Key = 0;
                point.Id.ContactFeature = cf;
                point.LocalPoint = circleB.Position;
                return;
            }

            {
                // Region AB
                var den = TSVector2.Dot(e, e);
                Debug.Assert(den > FP.Zero);
                var P = FP.One / den * (u * A + v * B);
                var d = Q - P;
                var dd = TSVector2.Dot(d, d);
                if (dd > radius * radius)
                {
                    return;
                }

                if (offset < FP.Zero)
                {
                    n.Set(-n.X, -n.Y);
                }

                n.Normalize();

                cf.IndexA = 0;
                cf.TypeA = (byte)ContactFeature.FeatureType.Face;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.FaceA;
                manifold.LocalNormal = n;
                manifold.LocalPoint = A;
                ref var point = ref manifold.Points.Value0;
                point.Id.Key = 0;
                point.Id.ContactFeature = cf;
                point.LocalPoint = circleB.Position;
            }
        }

        // This structure is used to keep track of the best separating axis.
        public struct EPAxis
        {
            public enum EPAxisType
            {
                Unknown,

                EdgeA,

                EdgeB
            }

            public TSVector2 Normal;

            public EPAxisType Type;

            public int Index;

            public FP Separation;
        }

        // This holds polygon B expressed in frame A.
        public struct TempPolygon
        {
            /// <summary>
            /// Size Settings.MaxPolygonVertices
            /// </summary>
            public FixedArray8<TSVector2> Vertices;

            /// <summary>
            /// Size Settings.MaxPolygonVertices
            /// </summary>
            public FixedArray8<TSVector2> Normals;

            public int Count;
        }

        // Reference face used for clipping
        private struct ReferenceFace
        {
            public int I1, I2;

            public TSVector2 Normal;

            public TSVector2 SideNormal1;

            public TSVector2 SideNormal2;

            public FP SideOffset1;

            public FP SideOffset2;

            public TSVector2 V1, V2;
        }

        static EPAxis ComputeEdgeSeparation(in TempPolygon polygonB, in TSVector2 v1, TSVector2 normal1)
        {
            EPAxis axis = new EPAxis
            {
                Type = EPAxis.EPAxisType.EdgeA,
                Index = -1,
                Separation = -Settings.MaxFloat,
                Normal = default
            };

            var axes = new[] {normal1, -normal1};

            // Find axis with least overlap (min-max problem)
            for (int j = 0; j < 2; ++j)
            {
                FP sj = Settings.MaxFloat;

                // Find deepest polygon vertex along axis j
                for (int i = 0; i < polygonB.Count; ++i)
                {
                    FP si = TSVector2.Dot(axes[j], polygonB.Vertices[i] - v1);
                    if (si < sj)
                    {
                        sj = si;
                    }
                }

                if (sj > axis.Separation)
                {
                    axis.Index = j;
                    axis.Separation = sj;
                    axis.Normal = axes[j];
                }
            }

            return axis;
        }

        static EPAxis ComputePolygonSeparation(in TempPolygon polygonB, in TSVector2 v1, in TSVector2 v2)
        {
            var axis = new EPAxis
            {
                Type = EPAxis.EPAxisType.Unknown,
                Index = -1,
                Separation = -Settings.MaxFloat,
                Normal = default
            };

            for (var i = 0; i < polygonB.Count; ++i)
            {
                var n = -polygonB.Normals[i];

                var s1 = TSVector2.Dot(n, polygonB.Vertices[i] - v1);
                var s2 = TSVector2.Dot(n, polygonB.Vertices[i] - v2);
                var s = FP.Min(s1, s2);

                if (s > axis.Separation)
                {
                    axis.Type = EPAxis.EPAxisType.EdgeB;
                    axis.Index = i;
                    axis.Separation = s;
                    axis.Normal = n;
                }
            }

            return axis;
        }

        // Use hysteresis for jitter reduction.
        static FP k_relativeTol = FP.One - FP.EN2 * 2;
        static FP k_absoluteTol = FP.EN3;

        public static void CollideEdgeAndPolygon(
            ref Manifold manifold,
            EdgeShape edgeA,
            Transform xfA,
            PolygonShape polygonB,
            in Transform xfB)
        {
            manifold.PointCount = 0;

            Transform xf = MathUtils.MulT(xfA, xfB);

            TSVector2 centroidB = MathUtils.Mul(xf, polygonB.Centroid);

            TSVector2 v1 = edgeA.Vertex1;
            TSVector2 v2 = edgeA.Vertex2;

            TSVector2 edge1 = v2 - v1;
            edge1.Normalize();

            // Normal points to the right for a CCW winding
            TSVector2 normal1 = new TSVector2(edge1.Y, -edge1.X);
            FP offset1 = TSVector2.Dot(normal1, centroidB - v1);

            bool oneSided = edgeA.OneSided;
            if (oneSided && offset1 < FP.Zero)
            {
                return;
            }

            // Get polygonB in frameA
            TempPolygon tempPolygonB = new TempPolygon();
            tempPolygonB.Count = polygonB.Count;
            for (int i = 0; i < polygonB.Count; ++i)
            {
                tempPolygonB.Vertices[i] = MathUtils.Mul(xf, polygonB.Vertices[i]);
                tempPolygonB.Normals[i] = MathUtils.Mul(xf.Rotation, polygonB.Normals[i]);
            }

            FP radius = polygonB.Radius + edgeA.Radius;

            EPAxis edgeAxis = ComputeEdgeSeparation(tempPolygonB, v1, normal1);
            if (edgeAxis.Separation > radius)
            {
                return;
            }

            EPAxis polygonAxis = ComputePolygonSeparation(tempPolygonB, v1, v2);
            if (polygonAxis.Separation > radius)
            {
                return;
            }

            var primaryAxis = new EPAxis();
            if (primaryAxis.Separation - radius > k_relativeTol * (edgeAxis.Separation - radius) + k_absoluteTol)
            {
                primaryAxis = polygonAxis;
            }
            else
            {
                primaryAxis = edgeAxis;
            }

            if (oneSided)
            {
                // Smooth collision
                // See https://box2d.org/posts/2020/06/ghost-collisions/

                TSVector2 edge0 = v1 - edgeA.Vertex0;
                edge0.Normalize();
                TSVector2 normal0 = new TSVector2(edge0.Y, -edge0.X);
                bool convex1 = MathUtils.Cross(edge0, edge1) >= FP.Zero;

                TSVector2 edge2 = edgeA.Vertex3 - v2;
                edge2.Normalize();
                TSVector2 normal2 = new TSVector2(edge2.Y, -edge2.X);
                bool convex2 = MathUtils.Cross(edge1, edge2) >= FP.Zero;

                bool side1 = TSVector2.Dot(primaryAxis.Normal, edge1) <= FP.Zero;

                // Check Gauss Map
                if (side1)
                {
                    if (convex1)
                    {
                        if (MathUtils.Cross(primaryAxis.Normal, normal0) > FP.EN1)
                        {
                            // Skip region
                            return;
                        }

                        // Admit region
                    }
                    else
                    {
                        // Snap region
                        primaryAxis = edgeAxis;
                    }
                }
                else
                {
                    if (convex2)
                    {
                        if (MathUtils.Cross(normal2, primaryAxis.Normal) > FP.EN1)
                        {
                            // Skip region
                            return;
                        }

                        // Admit region
                    }
                    else
                    {
                        // Snap region
                        primaryAxis = edgeAxis;
                    }
                }
            }

            ClipVertex[] clipPoints = new ClipVertex[2];
            ReferenceFace refFace = new ReferenceFace();
            if (primaryAxis.Type == EPAxis.EPAxisType.EdgeA)
            {
                manifold.Type = ManifoldType.FaceA;

                // Search for the polygon normal that is most anti-parallel to the edge normal.
                int bestIndex = 0;
                FP bestValue = TSVector2.Dot(primaryAxis.Normal, tempPolygonB.Normals[0]);
                for (int i = 1; i < tempPolygonB.Count; ++i)
                {
                    FP value = TSVector2.Dot(primaryAxis.Normal, tempPolygonB.Normals[i]);
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestIndex = i;
                    }
                }

                int i1 = bestIndex;
                int i2 = i1 + 1 < tempPolygonB.Count ? i1 + 1 : 0;

                clipPoints[0].Vector = tempPolygonB.Vertices[i1];
                clipPoints[0].Id.ContactFeature.IndexA = 0;
                clipPoints[0].Id.ContactFeature.IndexB = (byte)i1;
                clipPoints[0].Id.ContactFeature.TypeA = (byte)ContactFeature.FeatureType.Face;
                clipPoints[0].Id.ContactFeature.TypeB = (byte)ContactFeature.FeatureType.Vertex;

                clipPoints[1].Vector = tempPolygonB.Vertices[i2];
                clipPoints[1].Id.ContactFeature.IndexA = 0;
                clipPoints[1].Id.ContactFeature.IndexB = (byte)i2;
                clipPoints[1].Id.ContactFeature.TypeA = (byte)ContactFeature.FeatureType.Face;
                clipPoints[1].Id.ContactFeature.TypeB = (byte)ContactFeature.FeatureType.Vertex;

                refFace.I1 = 0;
                refFace.I2 = 1;
                refFace.V1 = v1;
                refFace.V2 = v2;
                refFace.Normal = primaryAxis.Normal;
                refFace.SideNormal1 = -edge1;
                refFace.SideNormal2 = edge1;
            }
            else
            {
                manifold.Type = ManifoldType.FaceB;

                clipPoints[0].Vector = v2;
                clipPoints[0].Id.ContactFeature.IndexA = 1;
                clipPoints[0].Id.ContactFeature.IndexB = (byte)primaryAxis.Index;
                clipPoints[0].Id.ContactFeature.TypeA = (byte)ContactFeature.FeatureType.Vertex;
                clipPoints[0].Id.ContactFeature.TypeB = (byte)ContactFeature.FeatureType.Face;

                clipPoints[1].Vector = v1;
                clipPoints[1].Id.ContactFeature.IndexA = 0;
                clipPoints[1].Id.ContactFeature.IndexB = (byte)primaryAxis.Index;
                clipPoints[1].Id.ContactFeature.TypeA = (byte)ContactFeature.FeatureType.Vertex;
                clipPoints[1].Id.ContactFeature.TypeB = (byte)ContactFeature.FeatureType.Face;

                refFace.I1 = primaryAxis.Index;
                refFace.I2 = refFace.I1 + 1 < tempPolygonB.Count ? refFace.I1 + 1 : 0;
                refFace.V1 = tempPolygonB.Vertices[refFace.I1];
                refFace.V2 = tempPolygonB.Vertices[refFace.I2];
                refFace.Normal = tempPolygonB.Normals[refFace.I1];

                // CCW winding
                refFace.SideNormal1.Set(refFace.Normal.Y, -refFace.Normal.X);
                refFace.SideNormal2 = -refFace.SideNormal1;
            }

            refFace.SideOffset1 = TSVector2.Dot(refFace.SideNormal1, refFace.V1);
            refFace.SideOffset2 = TSVector2.Dot(refFace.SideNormal2, refFace.V2);

            // Clip incident edge against reference face side planes
            Span<ClipVertex> clipPoints1 = stackalloc ClipVertex[2];
            Span<ClipVertex> clipPoints2 = stackalloc ClipVertex[2];
            int np;

            // Clip to side 1
            np = ClipSegmentToLine(clipPoints1, clipPoints, refFace.SideNormal1, refFace.SideOffset1, refFace.I1);

            if (np < Settings.MaxManifoldPoints)
            {
                return;
            }

            // Clip to side 2
            np = ClipSegmentToLine(clipPoints2, clipPoints1, refFace.SideNormal2, refFace.SideOffset2, refFace.I2);

            if (np < Settings.MaxManifoldPoints)
            {
                return;
            }

            // Now clipPoints2 contains the clipped points.
            if (primaryAxis.Type == EPAxis.EPAxisType.EdgeA)
            {
                manifold.LocalNormal = refFace.Normal;
                manifold.LocalPoint = refFace.V1;
            }
            else
            {
                manifold.LocalNormal = polygonB.Normals[refFace.I1];
                manifold.LocalPoint = polygonB.Vertices[refFace.I1];
            }

            var pointCount = 0;
            for (var i = 0; i < Settings.MaxManifoldPoints; ++i)
            {
                var separation = TSVector2.Dot(refFace.Normal, clipPoints2[i].Vector - refFace.V1);

                if (separation <= radius)
                {
                    ref var cp = ref manifold.Points[pointCount];

                    if (primaryAxis.Type == EPAxis.EPAxisType.EdgeA)
                    {
                        cp.LocalPoint = MathUtils.MulT(xf, clipPoints2[i].Vector);
                        cp.Id = clipPoints2[i].Id;
                    }
                    else
                    {
                        cp.LocalPoint = clipPoints2[i].Vector;
                        cp.Id.ContactFeature.TypeA = clipPoints2[i].Id.ContactFeature.TypeB;
                        cp.Id.ContactFeature.TypeB = clipPoints2[i].Id.ContactFeature.TypeA;
                        cp.Id.ContactFeature.IndexA = clipPoints2[i].Id.ContactFeature.IndexB;
                        cp.Id.ContactFeature.IndexB = clipPoints2[i].Id.ContactFeature.IndexA;
                    }

                    ++pointCount;
                }
            }

            manifold.PointCount = pointCount;
        }
    }
}