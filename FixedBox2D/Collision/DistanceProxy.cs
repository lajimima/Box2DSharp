using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using FixedBox2D.Collision.Shapes;
using TrueSync;

namespace FixedBox2D.Collision
{
    /// A distance proxy is used by the GJK algorithm.
    /// It encapsulates any shape.
    public struct DistanceProxy
    {
        /// Initialize the proxy using the given shape. The shape
        /// must remain in scope while the proxy is in use.
        public void Set(Shape shape, int index)
        {
            switch (shape)
            {
            case CircleShape circle:
            {
                Vertices = new[] {circle.Position};
                Count = 1;
                Radius = circle.Radius;
            }
                break;

            case PolygonShape polygon:
            {
                Vertices = polygon.Vertices;
                Count = polygon.Count;
                Radius = polygon.Radius;
            }
                break;

            case ChainShape chain:
            {
                Debug.Assert(0 <= index && index < chain.Count);
                Count = 2;
                Vertices = new TSVector2[Count];
                Vertices[0] = chain.Vertices[index];
                if (index + 1 < chain.Count)
                {
                    Vertices[1] = chain.Vertices[index + 1];
                }
                else
                {
                    Vertices[1] = chain.Vertices[0];
                }

                Radius = chain.Radius;
            }
                break;

            case EdgeShape edge:
            {
                Vertices = new TSVector2[]
                {
                    edge.Vertex1,
                    edge.Vertex2
                };
                Count = 2;
                Radius = edge.Radius;
            }
                break;

            default:
                throw new NotSupportedException();
            }
        }

        /// Initialize the proxy using a vertex cloud and radius. The vertices
        /// must remain in scope while the proxy is in use.
        public void Set(TSVector2[] vertices, int count, FP radius)
        {
            Vertices = new TSVector2[vertices.Length];
            Array.Copy(vertices, Vertices, vertices.Length);
            Count = count;
            Radius = radius;
        }

        /// Get the supporting vertex index in the given direction.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public int GetSupport(in TSVector2 d)
        {
            var bestIndex = 0;
            var bestValue = TSVector2.Dot(Vertices[0], d);
            for (var i = 1; i < Count; ++i)
            {
                var value = TSVector2.Dot(Vertices[i], d);
                if (value > bestValue)
                {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            return bestIndex;
        }

        /// Get the supporting vertex in the given direction.
        public ref readonly TSVector2 GetSupportVertex(in TSVector2 d)
        {
            var bestIndex = 0;
            var bestValue = TSVector2.Dot(Vertices[0], d);
            for (var i = 1; i < Count; ++i)
            {
                var value = TSVector2.Dot(Vertices[i], d);
                if (value > bestValue)
                {
                    bestIndex = i;
                    bestValue = value;
                }
            }

            return ref Vertices[bestIndex];
        }

        /// Get the vertex count.
        public int GetVertexCount()
        {
            return Count;
        }

        /// Get a vertex by index. Used by b2Distance.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public ref readonly TSVector2 GetVertex(int index)
        {
            Debug.Assert(0 <= index && index < Count);
            return ref Vertices[index];
        }

        public TSVector2[] Vertices;

        public int Count;

        public FP Radius;
    }

    public class GJkProfile
    {
        // GJK using Voronoi regions (Christer Ericson) and Barycentric coordinates.
        public int GjkCalls;

        public int GjkIters;

        public int GjkMaxIters;
    }
}