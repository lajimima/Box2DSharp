using TrueSync;

namespace FixedBox2D.Collision
{
    /// Output for b2Distance.
    public struct DistanceOutput
    {
        /// closest point on shapeA
        public TSVector2 PointA;

        /// closest point on shapeB
        public TSVector2 PointB;

        public FP Distance;

        /// number of GJK iterations used
        public int Iterations;
    }
}