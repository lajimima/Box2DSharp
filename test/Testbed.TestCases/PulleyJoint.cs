using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Joints", "Pulley")]
    public class PulleyJoint : TestBase
    {
        private FixedBox2D.Dynamics.Joints.PulleyJoint _joint1;

        public PulleyJoint()
        {
            var y = 16.0f;
            var L = 12.0f;
            var a = FP.One;
            var b = FP.Two;

            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var circle = new CircleShape();
                circle.Radius = FP.Two;

                circle.Position.Set(-10.0f, y + b + L);
                ground.CreateFixture(circle, FP.Zero);

                circle.Position.Set(10.0f, y + b + L);
                ground.CreateFixture(circle, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(a, b);

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;

                //bd.fixedRotation = true;
                bd.Position.Set(-10.0f, y);
                var body1 = World.CreateBody(bd);
                body1.CreateFixture(shape, 5.0f);

                bd.Position.Set(10.0f, y);
                var body2 = World.CreateBody(bd);
                body2.CreateFixture(shape, 5.0f);

                var pulleyDef = new PulleyJointDef();
                var anchor1 = new TSVector2(-10.0f, y + b);
                var anchor2 = new TSVector2(10.0f, y + b);
                var groundAnchor1 = new TSVector2(-10.0f, y + b + L);
                var groundAnchor2 = new TSVector2(10.0f, y + b + L);
                pulleyDef.Initialize(
                    body1,
                    body2,
                    groundAnchor1,
                    groundAnchor2,
                    anchor1,
                    anchor2,
                    1.5f);

                _joint1 = (FixedBox2D.Dynamics.Joints.PulleyJoint)World.CreateJoint(pulleyDef);
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            var ratio = _joint1.GetRatio();
            var L = _joint1.GetCurrentLengthA() + ratio * _joint1.GetCurrentLengthB();
            DrawString($"L1 + {ratio:F2} * L2 = {L:F2}");
        }
    }
}