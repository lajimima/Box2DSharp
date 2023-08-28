using System;
using TrueSync;

namespace FixedBox2D.Common
{
    public interface IDrawer
    {
        DrawFlag Flags { get; set; }

        /// Draw a closed polygon provided in CCW order.
        void DrawPolygon(Span<TSVector2> vertices, int vertexCount, in Color color);

        /// Draw a solid closed polygon provided in CCW order.
        void DrawSolidPolygon(Span<TSVector2> vertices, int vertexCount, in Color color);

        /// Draw a circle.
        void DrawCircle(in TSVector2 center, FP radius, in Color color);

        /// Draw a solid circle.
        void DrawSolidCircle(in TSVector2 center, FP radius, in TSVector2 axis, in Color color);

        /// Draw a line segment.
        void DrawSegment(in TSVector2 p1, in TSVector2 p2, in Color color);

        /// Draw a transform. Choose your own length scale.
        /// @param xf a transform.
        void DrawTransform(in Transform xf);

        /// Draw a point.
        void DrawPoint(in TSVector2 p, FP size, in Color color);
    }
}