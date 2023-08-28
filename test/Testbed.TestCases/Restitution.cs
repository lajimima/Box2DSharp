using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Forces", "Restitution")]
    public class Restitution : TestBase
    {
        static FP threshold = 10;
        public Restitution()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));

                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                fd.RestitutionThreshold = threshold;
                ground.CreateFixture(fd);
            }

            {
                var shape = new CircleShape();
                shape.Radius = FP.One;

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.One;

                FP[] restitution = {FP.Zero, FP.EN1, 0.3f, 0.5f, 0.75f, 0.9f, FP.One};

                for (var i = 0; i < 7; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-10.0f + 3.0f * i, 20.0f);

                    var body = World.CreateBody(bd);

                    fd.Restitution = restitution[i];
                    fd.RestitutionThreshold = threshold;
                    body.CreateFixture(fd);
                }
            }
        }
    }
}