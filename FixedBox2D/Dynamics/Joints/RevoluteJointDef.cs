using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Joints
{
    /// Revolute joint definition. This requires defining an anchor point where the
    /// bodies are joined. The definition uses local anchor points so that the
    /// initial configuration can violate the constraint slightly. You also need to
    /// specify the initial relative angle for joint limits. This helps when saving
    /// and loading a game.
    /// The local anchor points are measured from the body's origin
    /// rather than the center of mass because:
    /// 1. you might not know where the center of mass will be.
    /// 2. if you add/remove shapes from a body and recompute the mass,
    ///    the joints will be broken.
    public class RevoluteJointDef : JointDef
    {
        /// A flag to enable joint limits.
        public bool EnableLimit;

        /// A flag to enable the joint motor.
        public bool EnableMotor;

        /// The local anchor point relative to bodyA's origin.
        public TSVector2 LocalAnchorA;

        /// The local anchor point relative to bodyB's origin.
        public TSVector2 LocalAnchorB;

        /// The lower angle for the joint limit (radians).
        public FP LowerAngle;

        /// The maximum motor torque used to achieve the desired motor speed.
        /// Usually in N-m.
        public FP MaxMotorTorque;

        /// The desired motor speed. Usually in radians per second.
        public FP MotorSpeed;

        /// The bodyB angle minus bodyA angle in the reference state (radians).
        public FP ReferenceAngle;

        /// The upper angle for the joint limit (radians).
        public FP UpperAngle;

        public RevoluteJointDef()
        {
            JointType = JointType.RevoluteJoint;
            LocalAnchorA.Set(FP.Zero, FP.Zero);
            LocalAnchorB.Set(FP.Zero, FP.Zero);
            ReferenceAngle = FP.Zero;
            LowerAngle = FP.Zero;
            UpperAngle = FP.Zero;
            MaxMotorTorque = FP.Zero;
            MotorSpeed = FP.Zero;
            EnableLimit = false;
            EnableMotor = false;
        }

        /// Initialize the bodies, anchors, and reference angle using a world
        /// anchor point.
        public void Initialize(Body bA, Body bB, TSVector2 anchor)
        {
            BodyA = bA;
            BodyB = bB;
            LocalAnchorA = BodyA.GetLocalPoint(anchor);
            LocalAnchorB = BodyB.GetLocalPoint(anchor);
            ReferenceAngle = BodyB.GetAngle() - BodyA.GetAngle();
        }
    }
}