using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Dominos")]
    public class Dominos : TestBase
    {
        public Dominos()
        {
            Body b1;

            {
                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));

                var bd = new BodyDef();
                b1 = World.CreateBody(bd);
                b1.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(6.0f, 0.25f);

                var bd = new BodyDef();
                bd.Position.Set(-1.5f, 10.0f);
                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(FP.EN1, FP.One);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 20.0f;
                fd.Friction = FP.EN1;

                for (var i = 0; i < 10; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-6.0f + FP.One * i, 11.25f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);
                }
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(7.0f, 0.25f, TSVector2.Zero, 0.3f);

                var bd = new BodyDef();
                bd.Position.Set(FP.One, 6.0f);
                var ground = World.CreateBody(bd);
                ground.CreateFixture(shape, FP.Zero);
            }

            Body b2;
            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.25f, 1.5f);

                var bd = new BodyDef();
                bd.Position.Set(-7.0f, 4.0f);
                b2 = World.CreateBody(bd);
                b2.CreateFixture(shape, FP.Zero);
            }

            Body b3;
            {
                var shape = new PolygonShape();
                shape.SetAsBox(6.0f, 0.125f);

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-0.9f, FP.One);
                bd.Angle = -0.15f;

                b3 = World.CreateBody(bd);
                b3.CreateFixture(shape, 10.0f);
            }

            var jd = new RevoluteJointDef();
            var anchor = new TSVector2();

            anchor.Set(-FP.Two, FP.One);
            jd.Initialize(b1, b3, anchor);
            jd.CollideConnected = true;
            World.CreateJoint(jd);

            Body b4;

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.25f, 0.25f);

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-10.0f, 15.0f);
                b4 = World.CreateBody(bd);
                b4.CreateFixture(shape, 10.0f);
            }

            anchor.Set(-7.0f, 15.0f);
            jd.Initialize(b2, b4, anchor);
            World.CreateJoint(jd);

            Body b5;

            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(6.5f, 3.0f);
                b5 = World.CreateBody(bd);

                var shape = new PolygonShape();
                var fd = new FixtureDef();

                fd.Shape = shape;
                fd.Density = 10.0f;
                fd.Friction = FP.EN1;

                shape.SetAsBox(FP.One, FP.EN1, new TSVector2(FP.Zero, -0.9f), FP.Zero);
                b5.CreateFixture(fd);

                shape.SetAsBox(FP.EN1, FP.One, new TSVector2(-0.9f, FP.Zero), FP.Zero);
                b5.CreateFixture(fd);

                shape.SetAsBox(FP.EN1, FP.One, new TSVector2(0.9f, FP.Zero), FP.Zero);
                b5.CreateFixture(fd);
            }

            anchor.Set(6.0f, FP.Two);
            jd.Initialize(b1, b5, anchor);
            World.CreateJoint(jd);

            Body b6;

            {
                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, FP.EN1);

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(6.5f, 4.1f);
                b6 = World.CreateBody(bd);
                b6.CreateFixture(shape, 30.0f);
            }

            anchor.Set(7.5f, 4.0f);
            jd.Initialize(b5, b6, anchor);
            World.CreateJoint(jd);

            Body b7;
            {
                var shape = new PolygonShape();
                shape.SetAsBox(FP.EN1, FP.One);

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(7.4f, FP.One);

                b7 = World.CreateBody(bd);
                b7.CreateFixture(shape, 10.0f);
            }

            var djd = new DistanceJointDef();
            djd.BodyA = b3;
            djd.BodyB = b7;
            djd.LocalAnchorA.Set(6.0f, FP.Zero);
            djd.LocalAnchorB.Set(FP.Zero, -FP.One);
            var d = djd.BodyB.GetWorldPoint(djd.LocalAnchorB) - djd.BodyA.GetWorldPoint(djd.LocalAnchorA);
            djd.Length = d.magnitude;

            JointUtils.LinearStiffness(out djd.Stiffness, out djd.Damping, FP.One, FP.One, djd.BodyA, djd.BodyB);
            World.CreateJoint(djd);

            {
                var radius = 0.2f;

                var shape = new CircleShape();
                shape.Radius = radius;

                for (var i = 0; i < 4; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(5.9f + FP.Two * radius * i, 2.4f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(shape, 10.0f);
                }
            }
        }
    }
}