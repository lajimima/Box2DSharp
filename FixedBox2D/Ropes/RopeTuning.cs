using TrueSync;

namespace FixedBox2D.Ropes
{
    ///
    public class RopeTuning
    {
        public RopeTuning()
        {
            StretchingModel = StretchingModel.PbdStretchingModel;
            BendingModel = BendingModel.PbdAngleBendingModel;
            Damping = FP.Zero;
            StretchStiffness = FP.One;
            StretchHertz = FP.One;
            StretchDamping = FP.Zero;
            BendStiffness = FP.Half;
            BendHertz = FP.One;
            BendDamping = FP.Zero;
            Isometric = false;
            FixedEffectiveMass = false;
            WarmStart = false;
        }

        public StretchingModel StretchingModel;

        public BendingModel BendingModel;

        public FP Damping;

        public FP StretchStiffness;

        public FP StretchHertz;

        public FP StretchDamping;

        public FP BendStiffness;

        public FP BendHertz;

        public FP BendDamping;

        public bool Isometric;

        public bool FixedEffectiveMass;

        public bool WarmStart;
    };
}