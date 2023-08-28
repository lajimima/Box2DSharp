using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Slider Crank 1")]
    public class SliderCrank1 : TestBase
    {
        public SliderCrank1()
        {
            Body ground;
            {
                var bd = new BodyDef {Position = new TSVector2(FP.Zero, 17.0f)};
                ground = World.CreateBody(bd);
            }

            {
                var prevBody = ground;

                // Define crank.
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(4.0f, FP.One);

                    var bd = new BodyDef {BodyType = BodyType.DynamicBody, Position = new TSVector2(-8.0f, 20.0f)};
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(-12.0f, 20.0f));
                    World.CreateJoint(rjd);

                    prevBody = body;
                }

                // Define connecting rod
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(8.0f, FP.One);

                    var bd = new BodyDef {BodyType = BodyType.DynamicBody, Position = new TSVector2(4.0f, 20.0f)};
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(-4.0f, 20.0f));
                    World.CreateJoint(rjd);

                    prevBody = body;
                }

                // Define piston
                {
                    var shape = new PolygonShape();
                    shape.SetAsBox(3.0f, 3.0f);

                    var bd = new BodyDef
                    {
                        BodyType = BodyType.DynamicBody, FixedRotation = true,
                        Position = new TSVector2(12.0f, 20.0f)
                    };
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, FP.Two);

                    var rjd = new RevoluteJointDef();
                    rjd.Initialize(prevBody, body, new TSVector2(12.0f, 20.0f));
                    World.CreateJoint(rjd);

                    var pjd = new PrismaticJointDef();
                    pjd.Initialize(ground, body, new TSVector2(12.0f, 17.0f), new TSVector2(FP.One, FP.Zero));
                    World.CreateJoint(pjd);
                }
            }
        }
    }
}