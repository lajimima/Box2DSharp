using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Benchmark", "Add Pair Stress Test")]
    public class AddPair : TestBase
    {
        public AddPair()
        {
            World.Gravity = new TSVector2(FP.Zero, FP.Zero);
            {
                var shape = new CircleShape();
                shape.Position.SetZero();
                shape.Radius = FP.EN1;

                var minX = -6.0f;
                var maxX = FP.Zero;
                var minY = 4.0f;
                var maxY = 6.0f;

                for (var i = 0; i < 400; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position = new TSVector2(RandomFloat(minX, maxX), RandomFloat(minY, maxY));
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, 0.01f);
                }
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(1.5f, 1.5f);
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-40.0f, 5.0f);
                bd.Bullet = true;
                var body = World.CreateBody(bd);
                body.CreateFixture(shape, FP.One);
                body.SetLinearVelocity(new TSVector2(10.0f, FP.Zero));
            }
        }
    }
}