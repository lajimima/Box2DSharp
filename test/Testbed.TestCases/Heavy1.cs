using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Solver", "Heavy 1")]
    public class Heavy1 : TestBase
    {
        public Heavy1()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }
            {
                var bd = new BodyDef {BodyType = BodyType.DynamicBody, Position = new TSVector2(FP.Zero, 0.5f)};
                var body = World.CreateBody(bd);

                var shape = new CircleShape {Radius = 0.5f};
                body.CreateFixture(shape, 10.0f);

                bd.Position = new TSVector2(FP.Zero, 6.0f);
                body = World.CreateBody(bd);
                shape.Radius = 5.0f;
                body.CreateFixture(shape, 10.0f);
            }
        }
    }
}