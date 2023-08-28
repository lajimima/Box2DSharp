using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Collision.Collider
{
    /// This is used to compute the current state of a contact manifold.
    public struct WorldManifold
    {
        /// Evaluate the manifold with supplied transforms. This assumes
        /// modest motion from the original state. This does not change the
        /// point count, impulses, etc. The radii must come from the shapes
        /// that generated the manifold.
        public void Initialize(
            in Manifold manifold,
            in Transform xfA,
            FP radiusA,
            in Transform xfB,
            FP radiusB)
        {
            if (manifold.PointCount == 0)
            {
                return;
            }

            switch (manifold.Type)
            {
            case ManifoldType.Circles:
            {
                Normal.Set(FP.One, FP.Zero);
                var pointA = MathUtils.Mul(xfA, manifold.LocalPoint);
                var pointB = MathUtils.Mul(xfB, manifold.Points.Value0.LocalPoint);
                if (TSVector2.DistanceSquared(pointA, pointB) > Settings.Epsilon * Settings.Epsilon)
                {
                    Normal = pointB - pointA;
                    Normal.Normalize();
                }

                var cA = pointA + radiusA * Normal;
                var cB = pointB - radiusB * Normal;
                Points.Value0 = FP.Half * (cA + cB);
                Separations.Value0 = TSVector2.Dot(cB - cA, Normal);
            }
                break;

            case ManifoldType.FaceA:
            {
                Normal = MathUtils.Mul(xfA.Rotation, manifold.LocalNormal);
                var planePoint = MathUtils.Mul(xfA, manifold.LocalPoint);

                for (var i = 0; i < manifold.PointCount; ++i)
                {
                    var clipPoint = MathUtils.Mul(xfB, manifold.Points[i].LocalPoint);
                    var cA = clipPoint + (radiusA - TSVector2.Dot(clipPoint - planePoint, Normal)) * Normal;
                    var cB = clipPoint - radiusB * Normal;
                    Points[i] = FP.Half * (cA + cB);
                    Separations[i] = TSVector2.Dot(cB - cA, Normal);
                }
            }
                break;

            case ManifoldType.FaceB:
            {
                Normal = MathUtils.Mul(xfB.Rotation, manifold.LocalNormal);
                var planePoint = MathUtils.Mul(xfB, manifold.LocalPoint);

                for (var i = 0; i < manifold.PointCount; ++i)
                {
                    var clipPoint = MathUtils.Mul(xfA, manifold.Points[i].LocalPoint);
                    var cB = clipPoint + (radiusB - TSVector2.Dot(clipPoint - planePoint, Normal)) * Normal;
                    var cA = clipPoint - radiusA * Normal;
                    Points[i] = FP.Half * (cA + cB);
                    Separations[i] = TSVector2.Dot(cA - cB, Normal);
                }

                // Ensure normal points from A to B.
                Normal = -Normal;
            }
                break;
            }
        }

        /// world vector pointing from A to B
        public TSVector2 Normal;

        /// <summary>
        /// world contact point (point of intersection), size Settings.MaxManifoldPoints
        /// </summary>
        public FixedArray2<TSVector2> Points;

        /// <summary>
        /// a negative value indicates overlap, in meters, size Settings.MaxManifoldPoints
        /// </summary>
        public FixedArray2<FP> Separations;
    }
}