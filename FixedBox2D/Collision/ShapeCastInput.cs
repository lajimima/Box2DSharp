using System.Numerics;
using FixedBox2D.Common;

namespace FixedBox2D.Collision
{
    /// Input parameters for b2ShapeCast
    public struct ShapeCastInput
    {
        public DistanceProxy ProxyA;

        public DistanceProxy ProxyB;

        public Transform TransformA;

        public Transform TransformB;

        public Vector2 TranslationB;
    }
}