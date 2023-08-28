using FixedBox2D.Common;
using TrueSync;

namespace FixedBox2D.Collision
{
    /// Input parameters for b2ShapeCast
    public struct ShapeCastInput
    {
        public DistanceProxy ProxyA;

        public DistanceProxy ProxyB;

        public Transform TransformA;

        public Transform TransformB;

        public TSVector2 TranslationB;
    }
}