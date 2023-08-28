using TrueSync;

namespace FixedBox2D.Collision.Collider
{
    /// Ray-cast input data. The ray extends from p1 to p1 + maxFraction * (p2 - p1).
    public struct RayCastInput
    {
        public TSVector2 P1;

        public TSVector2 P2;

        public FP MaxFraction;
    }
}