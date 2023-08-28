using TrueSync;

namespace FixedBox2D.Collision
{
    public struct SimplexVertex
    {
        public TSVector2 Wa; // support point in proxyA

        public TSVector2 Wb; // support point in proxyB

        public TSVector2 W; // wB - wA

        public FP A; // barycentric coordinate for closest point

        public int IndexA; // wA index

        public int IndexB; // wB index
    }
}