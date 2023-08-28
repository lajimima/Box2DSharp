using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Joints
{
    /// Weld joint definition. You need to specify local anchor points
    /// where they are attached and the relative body angle. The position
    /// of the anchor points is important for computing the reaction torque.
    public class WeldJointDef : JointDef
    {
        /// The rotational stiffness in N*m
        /// Disable softness with a value of 0
        public FP Stiffness;

        /// The rotational damping in N*m*s
        public FP Damping;

        /// The local anchor point relative to bodyA's origin.
        public TSVector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public TSVector2 LocalAnchorB;

        /// The bodyB angle minus bodyA angle in the reference state (radians).
        public FP ReferenceAngle;

        public WeldJointDef()
        {
            JointType = JointType.WeldJoint;
            LocalAnchorA.Set(FP.Zero, FP.Zero);
            LocalAnchorB.Set(FP.Zero, FP.Zero);
            ReferenceAngle = FP.Zero;
            Stiffness = FP.Zero;
            Damping = FP.Zero;
        }

        /// <summary>
        /// Initialize the bodies, anchors, reference angle, stiffness, and damping.
        /// </summary>
        /// <param name="bA">the first body connected by this joint</param>
        /// <param name="bB">the second body connected by this joint</param>
        /// <param name="anchor">the point of connection in world coordinates</param>
        public void Initialize(Body bA, Body bB, in TSVector2 anchor)
        {
            BodyA = bA;
            BodyB = bB;
            LocalAnchorA = BodyA.GetLocalPoint(anchor);
            LocalAnchorB = BodyB.GetLocalPoint(anchor);
            ReferenceAngle = BodyB.GetAngle() - BodyA.GetAngle();
        }
    }
}