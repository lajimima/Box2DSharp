using TrueSync;

namespace FixedBox2D.Collision.Collider
{
    /// Used for computing contact manifolds.
    public struct ClipVertex
    {
        public TSVector2 Vector;

        public ContactId Id;
    }
}