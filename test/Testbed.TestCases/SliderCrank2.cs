using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Slider Crank 2")]
    public class SliderCrank2 : TestBase
    {
        private RevoluteJoint _joint1;

        private PrismaticJoint _joint2;

        public SliderCrank2()
        {
            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var prevBody = ground;

                // Define crank.
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(0.5f, FP.Two);

                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position = new TSVector2(FP.Zero, 7.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(FP.Zero, 5.0f));
                    rjd.MotorSpeed = FP.One * Settings.Pi;
                    rjd.MaxMotorTorque = 10000.0f;
                    rjd.EnableMotor = true;
                    _joint1 = (RevoluteJoint)World.CreateJoint(rjd);

                    prevBody = body;
                }

                // Define follower.
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(0.5f, 4.0f);

                    var bd = new BodyDef {BodyType = BodyType.DynamicBody, Position = new TSVector2(FP.Zero, 13.0f)};
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(FP.Zero, 9.0f));
                    rjd.EnableMotor = false;
                    World.CreateJoint(rjd);

                    prevBody = body;
                }

                // Define piston
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(1.5f, 1.5f);

                    var bd = new BodyDef
                    {
                        BodyType = BodyType.DynamicBody, FixedRotation = true,
                        Position = new TSVector2(FP.Zero, 17.0f)
                    };
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(FP.Zero, 17.0f));
                    World.CreateJoint(rjd);

                    var pjd = new PrismaticJointDef();
                    pjd.Initialize(ground, body, new TSVector2(FP.Zero, 17.0f), new TSVector2(FP.Zero, FP.One));

                    pjd.MaxMotorForce = 1000.0f;
                    pjd.EnableMotor = true;

                    _joint2 = (PrismaticJoint)World.CreateJoint(pjd);
                }

                // Create a payload
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(1.5f, 1.5f);

                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position = new TSVector2(FP.Zero, 23.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);
                }
            }
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.F)
            {
                _joint2.EnableMotor(!_joint2.IsMotorEnabled());
                _joint2.BodyB.IsAwake = true;
            }

            if (keyInput.Key == KeyCodes.M)
            {
                _joint1.EnableMotor(!_joint1.IsMotorEnabled());
                _joint1.BodyB.IsAwake = true;
            }
        }

        protected override void OnRender()
        {
            DrawString("Keys: F toggle friction, M toggle motor");
            var torque = _joint1.GetMotorTorque(TestSettings.Hertz);
            DrawString($"Motor Torque = {torque}");
            DrawString($"Friction: {_joint2.IsMotorEnabled()}");
            DrawString($"Motor: {_joint1.IsMotorEnabled()}");
        }
    }
}