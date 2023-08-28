using TrueSync;

namespace FixedBox2D.Ropes
{
    /// 
    public struct RopeDef
    {
        public TSVector2 Position;

        public TSVector2[] Vertices;

        public int Count;

        public FP[] Masses;

        public TSVector2 Gravity;

        public RopeTuning Tuning;
    };
}