using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Forces", "Friction")]
    public class Friction : TestBase
    {
        public Friction()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(13.0f, 0.25f);

                var bd = new BodyDef();
                bd.Position.Set(-4.0f, 22.0f);
                bd.Angle = -0.25f;

                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.25f, FP.One);

                var bd = new BodyDef();
                bd.Position.Set(10.5f, 19.0f);

                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(13.0f, 0.25f);

                var bd = new BodyDef();
                bd.Position.Set(4.0f, 14.0f);
                bd.Angle = 0.25f;

                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.25f, FP.One);

                var bd = new BodyDef();
                bd.Position.Set(-10.5f, 11.0f);

                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(13.0f, 0.25f);

                var bd = new BodyDef();
                bd.Position.Set(-4.0f, 6.0f);
                bd.Angle = -0.25f;

                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 25.0f;

                FP[] friction = {0.75f, 0.5f, 0.35f, FP.EN1, FP.Zero};

                for (var i = 0; i < 5; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-15.0f + 4.0f * i, 28.0f);
                    var body = World.CreateBody(bd);

                    fd.Friction = friction[i];
                    body.CreateFixture(fd);
                }
            }
        }
    }
}