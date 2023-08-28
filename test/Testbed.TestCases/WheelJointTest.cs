using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Joints", "Wheel")]
    public class WheelJointTest : TestBase
    {
        protected WheelJoint Joint;

        protected FP MotorSpeed;

        protected bool EnableMotor;

        protected bool EnableLimit;

        public WheelJointTest()
        {
            Body ground = null;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            EnableLimit = true;
            EnableMotor = false;
            MotorSpeed = 10.0f;

            {
                CircleShape shape = new CircleShape();
                shape.Radius = FP.Two;

                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 10.0f);
                bd.AllowSleep = false;
                var body = World.CreateBody(bd);
                body.CreateFixture(shape, 5.0f);

                var mass = body.Mass;
                var hertz = FP.One;
                var dampingRatio = 0.7f;
                var omega = FP.Two * Settings.Pi * hertz;

                var jd = new WheelJointDef();

                // Horizontal
                jd.Initialize(ground, body, bd.Position, new TSVector2(FP.Zero, FP.One));

                jd.MotorSpeed = MotorSpeed;
                jd.MaxMotorTorque = 10000.0f;
                jd.EnableMotor = EnableMotor;
                jd.Stiffness = mass * omega * omega;
                jd.Damping = FP.Two * mass * dampingRatio * omega;
                jd.LowerTranslation = -3.0f;
                jd.UpperTranslation = 3.0f;
                jd.EnableLimit = EnableLimit;

                Joint = (WheelJoint)World.CreateJoint(jd);
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            var torque = Joint.GetMotorTorque(TestSettings.Hertz);
            DrawString($"Motor Torque = {torque}");

            var F = Joint.GetReactionForce(TestSettings.Hertz);
            DrawString($"Reaction Force = ({F.X}, {F.X})");
        }
    }
}