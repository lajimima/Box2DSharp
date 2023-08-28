using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Benchmark", "Tumbler")]
    public class Tumbler : TestBase
    {
        private const int Count = 800;

        private RevoluteJoint _joint;

        private int _count;

        public Tumbler()
        {
            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);
            }

            {
                var bd = new BodyDef
                {
                    BodyType = BodyType.DynamicBody,
                    AllowSleep = false,
                    Position = new TSVector2(FP.Zero, 10.0f)
                };
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 10.0f, new TSVector2(10.0f, FP.Zero), FP.Zero);
                body.CreateFixture(shape, 5.0f);
                shape.SetAsBox(0.5f, 10.0f, new TSVector2(-10.0f, FP.Zero), FP.Zero);
                body.CreateFixture(shape, 5.0f);
                shape.SetAsBox(10.0f, 0.5f, new TSVector2(FP.Zero, 10.0f), FP.Zero);
                body.CreateFixture(shape, 5.0f);
                shape.SetAsBox(10.0f, 0.5f, new TSVector2(FP.Zero, -10.0f), FP.Zero);
                body.CreateFixture(shape, 5.0f);

                var jd = new RevoluteJointDef
                {
                    BodyA = ground,
                    BodyB = body,
                    LocalAnchorA = new TSVector2(FP.Zero, 10.0f),
                    LocalAnchorB = new TSVector2(FP.Zero, FP.Zero),
                    ReferenceAngle = FP.Zero,
                    MotorSpeed = 0.05f * Settings.Pi,
                    MaxMotorTorque = 1e8f,
                    EnableMotor = true
                };
                _joint = (RevoluteJoint)World.CreateJoint(jd);
            }

            _count = 0;
        }

        /// <inheritdoc />
        protected override void PreStep()
        {
            if (_count < Count)
            {
                var bd = new BodyDef
                {
                    BodyType = BodyType.DynamicBody,
                    Position = new TSVector2(FP.Zero, 10.0f)
                };
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(0.125f, 0.125f);
                body.CreateFixture(shape, FP.One);

                ++_count;
            }
        }
    }
}