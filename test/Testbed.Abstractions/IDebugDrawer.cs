using System.Numerics;
using FixedBox2D.Collision;
using FixedBox2D.Common;

namespace Testbed.Abstractions
{
    public interface IDebugDrawer : IDrawer
    {
        void DrawAABB(AABB aabb, Color color);

        void DrawString(float x, float y, string strings);

        void DrawString(int x, int y, string strings);

        void DrawString(Vector2 position, string strings);
    }
}