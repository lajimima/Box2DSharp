using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Stacking", "Circles")]
    public class CircleStack : TestBase
    {
        private const int Count = 10;

        private readonly Body[] _bodies = new Body[Count];

        public CircleStack()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new CircleShape {Radius = FP.One};

                for (var i = 0; i < Count; ++i)
                {
                    var bd = new BodyDef
                    {
                        BodyType = BodyType.DynamicBody, Position = new TSVector2(FP.Zero, 4.0f + 3.0f * i)
                    };

                    _bodies[i] = World.CreateBody(bd);

                    _bodies[i].CreateFixture(shape, FP.One);

                    _bodies[i].SetLinearVelocity(new TSVector2(FP.Zero, -50.0f));
                }
            }
        }
    }
}