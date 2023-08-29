using System;
using FixedBox2D.Collision;
using FixedBox2D.Common;
using ImGuiNET;
using OpenTK.Mathematics;
using Testbed.Abstractions;
using TrueSync;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Testbed.Render
{
    public class DebugDrawer : IDebugDrawer
    {
        /// <inheritdoc />
        public DrawFlag Flags { get; set; }

        public bool ShowUI = true;

        private GLRenderPoints _points;

        private GLRenderLines _lines;

        private GLRenderTriangles _triangles;

        public void Create()
        {
            _points = new GLRenderPoints();
            _points.Create();
            _lines = new GLRenderLines();
            _lines.Create();
            _triangles = new GLRenderTriangles();
            _triangles.Create();
        }

        public void Destroy()
        {
            _points.Destroy();
            _points = null;
            _lines.Destroy();
            _lines = null;
            _triangles.Destroy();
            _triangles = null;
        }

        public void Flush()
        {
            _triangles?.Flush();
            _lines?.Flush();
            _points?.Flush();
        }

        private readonly Vector4 _textColor = new Vector4(0.9f, 0.6f, 0.6f, 1);

        public void DrawString(FP x, FP y, string strings)
        {
            if (ShowUI == false)
            {
                return;
            }

            ImGui.Begin("Overlay", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar);
            ImGui.SetCursorPos(new Vector2(x.AsFloat(), y.AsFloat()));
            ImGui.TextColored(_textColor, strings);
            ImGui.End();
        }

        public void DrawString(int x, int y, string strings)
        {
            if (ShowUI == false)
            {
                return;
            }

            ImGui.Begin("Overlay", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar);
            ImGui.SetCursorPos(new Vector2(x, y));
            ImGui.TextColored(_textColor, strings);
            ImGui.End();
        }

        public void DrawString(TSVector2 worldPosition, string strings)
        {
            var ps = Global.Camera.ConvertWorldToScreen(worldPosition);
            ImGui.Begin("Overlay", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar);
            ImGui.SetCursorPos(ps.ToVector2());
            ImGui.TextColored(_textColor, strings);
            ImGui.End();
        }

        /// <inheritdoc />
        public void DrawPolygon(Span<TSVector2> vertices, int vertexCount, in Color color)
        {
            var p1 = vertices[vertexCount - 1];
            for (var i = 0; i < vertexCount; ++i)
            {
                var p2 = vertices[i];
                _lines.Vertex(p1.ToVector2(), color.ToColor4());
                _lines.Vertex(p2.ToVector2(), color.ToColor4());
                p1 = p2;
            }
        }

        /// <inheritdoc />
        public void DrawSolidPolygon(Span<TSVector2> vertices, int vertexCount, in Color color)
        {
            var color4 = color.ToColor4();
            var fillColor = new Color4(color4.R * 0.5f, color4.G * 0.5f, color4.B * 0.5f, color4.A * 0.5f);

            for (var i = 1; i < vertexCount - 1; ++i)
            {
                _triangles.Vertex(vertices[0].ToVector2(), fillColor);
                _triangles.Vertex(vertices[i].ToVector2(), fillColor);
                _triangles.Vertex(vertices[i + 1].ToVector2(), fillColor);
            }

            var p1 = vertices[vertexCount - 1];
            for (var i = 0; i < vertexCount; ++i)
            {
                var p2 = vertices[i];
                _lines.Vertex(p1.ToVector2(), color4);
                _lines.Vertex(p2.ToVector2(), color4);
                p1 = p2;
            }
        }

        /// <inheritdoc />
        public void DrawCircle(in TSVector2 center, FP radius, in Color color)
        {
            var color4 = color.ToColor4();
            float Segments = 16.0f;
            float Increment = (2.0f * Settings.Pi / Segments).AsFloat();
            var sinInc = (float)Math.Sin(Increment);
            var cosInc = (float)Math.Cos(Increment);
            var r1 = new Vector2(1.0f, 0.0f);
            var v1 = center.ToVector2() + radius.AsFloat() * r1;
            for (var i = 0; i < Segments; ++i)
            {
                // Perform rotation to avoid additional trigonometry.
                var r2 = new Vector2
                {
                    X = cosInc * r1.X - sinInc * r1.Y,
                    Y = sinInc * r1.X + cosInc * r1.Y
                };
                var v2 = center.ToVector2() + radius.AsFloat() * r2;
                _lines.Vertex(v1, color4);
                _lines.Vertex(v2, color4);
                r1 = r2;
                v1 = v2;
            }
        }

        /// <inheritdoc />
        public void DrawSolidCircle(in TSVector2 center, FP radius, in TSVector2 axis, in Color color)
        {
            var color4 = color.ToColor4();
            float Segments = 16.0f;
            float Increment = (2.0f * Settings.Pi / Segments).AsFloat();
            var sinInc = (float)Math.Sin(Increment);
            var cosInc = (float)Math.Cos(Increment);
            var v0 = center;
            var r1 = new Vector2(cosInc, sinInc);
            var v1 = center.ToVector2() + radius.AsFloat() * r1;
            var fillColor = new Color4(color4.R * 0.5f, color4.G * 0.5f, color4.B * 0.5f, color4.A * 0.5f);
            for (var i = 0; i < Segments; ++i)
            {
                // Perform rotation to avoid additional trigonometry.
                var r2 = new Vector2
                {
                    X = cosInc * r1.X - sinInc * r1.Y,
                    Y = sinInc * r1.X + cosInc * r1.Y
                };
                var v2 = center.ToVector2() + radius.AsFloat() * r2;
                _triangles.Vertex(v0.ToVector2(), fillColor);
                _triangles.Vertex(v1, fillColor);
                _triangles.Vertex(v2, fillColor);
                r1 = r2;
                v1 = v2;
            }

            r1.Set(1.0f, 0.0f);
            v1 = center.ToVector2() + radius.AsFloat() * r1;
            for (var i = 0; i < Segments; ++i)
            {
                var r2 = new Vector2
                {
                    X = cosInc * r1.X - sinInc * r1.Y,
                    Y = sinInc * r1.X + cosInc * r1.Y
                };
                var v2 = center.ToVector2() + radius.AsFloat() * r2;
                _lines.Vertex(v1, color4);
                _lines.Vertex(v2, color4);
                r1 = r2;
                v1 = v2;
            }

            // Draw a line fixed in the circle to animate rotation.
            var p = center + radius * axis;
            _lines.Vertex(center.ToVector2(), color4);
            _lines.Vertex(p.ToVector2(), color4);
        }

        /// <inheritdoc />
        public void DrawSegment(in TSVector2 p1, in TSVector2 p2, in Color color)
        {
            var color4 = color.ToColor4();
            _lines.Vertex(p1.ToVector2(), color4);
            _lines.Vertex(p2.ToVector2(), color4);
        }

        /// <inheritdoc />
        public void DrawTransform(in Transform xf)
        {
            const float AxisScale = 0.4f;

            var p1 = xf.Position;
            _lines.Vertex(p1.ToVector2(), Color4.Red);
            var p2 = p1 + AxisScale * xf.Rotation.GetXAxis();
            _lines.Vertex(p2.ToVector2(), Color4.Red);

            _lines.Vertex(p1.ToVector2(), Color4.Green);
            p2 = p1 + AxisScale * xf.Rotation.GetYAxis();
            _lines.Vertex(p2.ToVector2(), Color4.Green);
        }

        /// <inheritdoc />
        public void DrawPoint(in TSVector2 p, FP size, in Color color)
        {
            _points.Vertex(p.ToVector2(), color.ToColor4(), size.AsFloat());
        }

        public void DrawAABB(AABB aabb, Color c)
        {
            var color4 = c.ToColor4();
            var p1 = aabb.LowerBound.ToVector2();
            var p2 = new Vector2(aabb.UpperBound.X.AsFloat(), aabb.LowerBound.Y.AsFloat());
            var p3 = aabb.UpperBound.ToVector2();
            var p4 = new Vector2(aabb.LowerBound.X.AsFloat(), aabb.UpperBound.Y.AsFloat());

            _lines.Vertex(p1, color4);
            _lines.Vertex(p2, color4);

            _lines.Vertex(p2, color4);
            _lines.Vertex(p3, color4);

            _lines.Vertex(p3, color4);
            _lines.Vertex(p4, color4);

            _lines.Vertex(p4, color4);
            _lines.Vertex(p1, color4);
        }
    }
}