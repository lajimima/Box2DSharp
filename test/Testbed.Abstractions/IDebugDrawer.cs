using TrueSync;
using FixedBox2D.Collision;
using FixedBox2D.Common;

namespace Testbed.Abstractions
{
    public interface IDebugDrawer : IDrawer
    {
        void DrawAABB(AABB aabb, Color color);

        void DrawString(FP x, FP y, string strings);

        void DrawString(int x, int y, string strings);

        void DrawString(TSVector2 position, string strings);
    }
}