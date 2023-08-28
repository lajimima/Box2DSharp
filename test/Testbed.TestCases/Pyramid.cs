using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Stacking", "Pyramid")]
    public class Pyramid : TestBase
    {
        private const int Count = 20;

        public Pyramid()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var a = 0.5f;
                var shape = new PolygonShape();
                shape.SetAsBox(a, a);

                var x = new TSVector2(-7.0f, 0.75f);
                TSVector2 y;
                var deltaX = new TSVector2(0.5625f, 1.25f);
                var deltaY = new TSVector2(1.125f, FP.Zero);

                for (var i = 0; i < Count; ++i)
                {
                    y = x;

                    for (var j = i; j < Count; ++j)
                    {
                        var bd = new BodyDef();
                        bd.BodyType = BodyType.DynamicBody;
                        bd.Position = y;
                        var body = World.CreateBody(bd);
                        body.CreateFixture(shape, 5.0f);

                        y += deltaY;
                    }

                    x += deltaX;
                }
            }
        }
    }
}