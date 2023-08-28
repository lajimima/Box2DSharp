using TrueSync;

namespace FixedBox2D.Dynamics.Contacts
{
    public struct VelocityConstraintPoint
    {
        public FP NormalImpulse;

        public FP NormalMass;

        public TSVector2 Ra;

        public TSVector2 Rb;

        public FP TangentImpulse;

        public FP TangentMass;

        public FP VelocityBias;
    }
}