using System.Numerics;

namespace FixedBox2D.Collision.Collider
{
    /// Used for computing contact manifolds.
    public struct ClipVertex
    {
        public Vector2 Vector;

        public ContactId Id;
    }
}