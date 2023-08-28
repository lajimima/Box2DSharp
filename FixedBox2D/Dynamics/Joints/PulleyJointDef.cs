using System.Diagnostics;
using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Joints
{
    /// Pulley joint definition. This requires two ground anchors,
    /// two dynamic body anchor points, and a pulley ratio.
    public class PulleyJointDef : JointDef
    {
        /// The first ground anchor in world coordinates. This point never moves.
        public TSVector2 GroundAnchorA;

        /// The second ground anchor in world coordinates. This point never moves.
        public TSVector2 GroundAnchorB;

        /// The a reference length for the segment attached to bodyA.
        public FP LengthA;

        /// The a reference length for the segment attached to bodyB.
        public FP LengthB;

        /// The local anchor point relative to bodyA's origin.
        public TSVector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public TSVector2 LocalAnchorB;

        /// The pulley ratio, used to simulate a block-and-tackle.
        public FP Ratio;

        public PulleyJointDef()
        {
            JointType = JointType.PulleyJoint;

            GroundAnchorA.Set(-FP.One, FP.One);

            GroundAnchorB.Set(FP.One, FP.One);

            LocalAnchorA.Set(-FP.One, FP.Zero);

            LocalAnchorB.Set(FP.One, FP.Zero);

            LengthA = FP.Zero;

            LengthB = FP.Zero;

            Ratio = FP.One;

            CollideConnected = true;
        }

        /// Initialize the bodies, anchors, lengths, max lengths, and ratio using the world anchors.
        public void Initialize(
            Body bA,
            Body bB,
            in TSVector2 groundA,
            in TSVector2 groundB,
            in TSVector2 anchorA,
            in TSVector2 anchorB,
            FP r)
        {
            BodyA = bA;
            BodyB = bB;
            GroundAnchorA = groundA;
            GroundAnchorB = groundB;
            LocalAnchorA = BodyA.GetLocalPoint(anchorA);
            LocalAnchorB = BodyB.GetLocalPoint(anchorB);
            var dA = anchorA - groundA;
            LengthA = dA.magnitude;
            var dB = anchorB - groundB;
            LengthB = dB.magnitude;
            Ratio = r;
            Debug.Assert(Ratio > Settings.Epsilon);
        }
    }
}