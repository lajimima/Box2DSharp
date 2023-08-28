using System.Numerics;

namespace FixedBox2D.Ropes
{
    /// 
    public struct RopeDef
    {
        public Vector2 Position;

        public Vector2[] Vertices;

        public int Count;

        public float[] Masses;

        public Vector2 Gravity;

        public RopeTuning Tuning;
    };
}