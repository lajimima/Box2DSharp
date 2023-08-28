using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Bugs", "Chain Problem")]
    public class ChainProblem : TestBase
    {
        public ChainProblem()
        {
            TSVector2 g = new TSVector2(FP.Zero, -10.0f);
            World.Gravity = g;
            var bodies = new Body[2];
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.StaticBody;
                bodies[0] = World.CreateBody(bd);

                {
                    var v1 = new TSVector2(FP.Zero, FP.One);
                    var v2 = new TSVector2(FP.Zero, FP.Zero);
                    var v3 = new TSVector2(4.0f, FP.Zero);

                    EdgeShape shape = new EdgeShape();
                    shape.SetTwoSided(v1, v2);
                    bodies[0].CreateFixture(shape, FP.Zero);

                    shape.SetTwoSided(v2, v3);
                    bodies[0].CreateFixture(shape, FP.Zero);
                }
            }
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;

                //bd.position.Set(6.033980250358582e-01f, 3.028350114822388e+00f);
                bd.Position.Set(FP.One, 3.0f);
                bodies[1] = World.CreateBody(bd);

                {
                    FixtureDef fd = new FixtureDef();
                    fd.Friction = 0.2f;
                    fd.Density = 10.0f;
                    PolygonShape shape = new PolygonShape();
                    var vs = new TSVector2[8];
                    vs[0].Set(0.5f, -3.0f);
                    vs[1].Set(0.5f, 3.0f);
                    vs[2].Set(-0.5f, 3.0f);
                    vs[3].Set(-0.5f, -3.0f);
                    shape.Set(vs, 4);

                    fd.Shape = shape;

                    bodies[1].CreateFixture(fd);
                }
            }
            bodies = default;
        }
    }
}