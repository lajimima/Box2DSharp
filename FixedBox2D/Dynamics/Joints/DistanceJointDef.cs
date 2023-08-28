using System;
using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Joints
{
    /// Distance joint definition. This requires defining an anchor point on both
    /// bodies and the non-zero distance of the distance joint. The definition uses
    /// local anchor points so that the initial configuration can violate the
    /// constraint slightly. This helps when saving and loading a game.
    public class DistanceJointDef : JointDef
    {
        /// Minimum length. Clamped to a stable minimum value.
        public FP MinLength;

        /// Maximum length. Must be greater than or equal to the minimum length.
        public FP MaxLength;

        /// The linear stiffness in N/m.
        public FP Stiffness;

        /// The linear damping in N*s/m.
        public FP Damping;

        /// The rest length of this joint. Clamped to a stable minimum value.
        public FP Length;

        /// The local anchor point relative to bodyA's origin.
        public TSVector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public TSVector2 LocalAnchorB;

        public DistanceJointDef()
        {
            JointType = JointType.DistanceJoint;
            LocalAnchorA.Set(FP.Zero, FP.Zero);
            LocalAnchorB.Set(FP.Zero, FP.Zero);
            MinLength = FP.Zero;
            MaxLength = Settings.MaxFloat;
            Length = FP.One;
            Stiffness = FP.Zero;
            Damping = FP.Zero;
        }

        /// Initialize the bodies, anchors, and rest length using world space anchors.
        /// The minimum and maximum lengths are set to the rest length.
        public void Initialize(
            Body b1,
            Body b2,
            in TSVector2 anchor1,
            in TSVector2 anchor2)
        {
            BodyA = b1;
            BodyB = b2;
            LocalAnchorA = BodyA.GetLocalPoint(anchor1);
            LocalAnchorB = BodyB.GetLocalPoint(anchor2);
            var d = anchor2 - anchor1;
            Length = FP.Max(d.magnitude, Settings.LinearSlop);
            MinLength = Length;
            MaxLength = Length;
        }
    }
}