using TrueSync;

namespace FixedBox2D.Collision.Collider
{
    /// Ray-cast output data. The ray hits at p1 + fraction * (p2 - p1), where p1 and p2
    /// come from b2RayCastInput.
    public struct RayCastOutput
    {
        public TSVector2 Normal;

        public FP Fraction;
    }
}