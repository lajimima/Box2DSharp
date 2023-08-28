using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Solver", "Mobile Unbalanced")]
    public class Mobile : TestBase
    {
        private const int Depth = 4;

        public Mobile()
        {
            Body ground;

            // Create ground body.
            {
                var bodyDef = new BodyDef();
                bodyDef.Position.Set(FP.Zero, 20.0f);
                ground = World.CreateBody(bodyDef);
            }

            var a = 0.5f;
            var h = new TSVector2(FP.Zero, a);

            var root = AddNode(ground, TSVector2.Zero, 0, 3.0f, a);

            var jointDef = new RevoluteJointDef();
            jointDef.BodyA = ground;
            jointDef.BodyB = root;
            jointDef.LocalAnchorA.SetZero();
            jointDef.LocalAnchorB = h;
            World.CreateJoint(jointDef);
        }

        private Body AddNode(Body parent, TSVector2 localAnchor, int depth, FP offset, FP a)
        {
            var density = 20.0f;
            var h = new TSVector2(FP.Zero, a);

            var p = parent.GetPosition() + localAnchor - h;

            var bodyDef = new BodyDef();
            bodyDef.BodyType = BodyType.DynamicBody;
            bodyDef.Position = p;
            var body = World.CreateBody(bodyDef);

            var shape = new PolygonShape();
            shape.SetAsBox(0.25f * a, a);
            body.CreateFixture(shape, density);

            if (depth == Depth)
            {
                return body;
            }

            var a1 = new TSVector2(offset, -a);
            var a2 = new TSVector2(-offset, -a);
            var body1 = AddNode(body, a1, depth + 1, 0.5f * offset, a);
            var body2 = AddNode(body, a2, depth + 1, 0.5f * offset, a);

            var jointDef = new RevoluteJointDef();
            jointDef.BodyA = body;
            jointDef.LocalAnchorB = h;

            jointDef.LocalAnchorA = a1;
            jointDef.BodyB = body1;
            World.CreateJoint(jointDef);

            jointDef.LocalAnchorA = a2;
            jointDef.BodyB = body2;
            World.CreateJoint(jointDef);

            return body;
        }
    }
}