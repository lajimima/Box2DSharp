using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Extra", "Position Test")]
    public class Position : TestBase
    {
        /// <inheritdoc />
        public Position()
        {
            var gshape = new EdgeShape();
            gshape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));

            var ground = World.CreateBody(new BodyDef() {BodyType = BodyType.StaticBody, Position = new TSVector2(0, -5)})
                              .CreateFixture(gshape, FP.One);
            for (var i = 0; i < 100; i++)
            {
                var b1 = World.CreateBody(
                    new BodyDef() {BodyType = BodyType.DynamicBody, Position = new TSVector2(RandomFloat(0, 5), RandomFloat(0, 5))});
                var shape = new CircleShape() {Radius = 1};

                b1.CreateFixture(shape, FP.One);
            }
        }
    }
}