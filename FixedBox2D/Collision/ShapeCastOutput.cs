using TrueSync;

namespace FixedBox2D.Collision
{
    /// Output results for b2ShapeCast
    public struct ShapeCastOutput
    {
        public TSVector2 Point;

        public TSVector2 Normal;

        public FP Lambda;

        public int Iterations;
    }
}