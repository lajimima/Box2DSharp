using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    /// <summary>
    /// Test the prismatic joint with limits and motor options.
    /// </summary>
    [TestCase("Joints", "Prismatic")]
    public class PrismaticJointTest : TestBase
    {
        protected PrismaticJoint Joint;

        protected float MotorSpeed;

        protected bool EnableMotor;

        protected bool EnableLimit;

        public PrismaticJointTest()
        {
            Body ground;
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
                PolygonShape shape = new PolygonShape();
                shape.SetAsBox(FP.One, FP.One);

                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 10.0f);
                bd.Angle = 0.5f * Settings.Pi;
                bd.AllowSleep = false;
                var body = World.CreateBody(bd);
                body.CreateFixture(shape, 5.0f);

                PrismaticJointDef pjd = new PrismaticJointDef();

                // Horizontal
                pjd.Initialize(ground, body, bd.Position, new TSVector2(FP.One, FP.Zero));

                pjd.MotorSpeed = MotorSpeed;
                pjd.MaxMotorForce = 10000.0f;
                pjd.EnableMotor = EnableMotor;
                pjd.LowerTranslation = -10.0f;
                pjd.UpperTranslation = 10.0f;
                pjd.EnableLimit = EnableLimit;

                Joint = (PrismaticJoint)World.CreateJoint(pjd);
            }
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.L)
            {
                Joint.EnableLimit(!Joint.IsLimitEnabled());
            }

            if (keyInput.Key == KeyCodes.L)
            {
                Joint.EnableMotor(!Joint.IsMotorEnabled());
            }

            if (keyInput.Key == KeyCodes.L)
            {
                Joint.SetMotorSpeed(-Joint.GetMotorSpeed());
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            var force = Joint.GetMotorForce(TestSettings.Hertz);
            DrawString($"Motor Force = {force}");
        }
    }
}