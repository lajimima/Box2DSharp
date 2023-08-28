using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Joints", "Cantilever")]
    public class Cantilever : TestBase
    {
        private const int Count = 8;

        public Cantilever()
        {
            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.125f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 20.0f;

                var jd = new WeldJointDef();

                var prevBody = ground;
                for (var i = 0; i < Count; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-14.5f + FP.One * i, 5.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);

                    var anchor = new TSVector2(-15.0f + FP.One * i, 5.0f);
                    jd.Initialize(prevBody, body, anchor);
                    World.CreateJoint(jd);

                    prevBody = body;
                }
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, 0.125f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 20.0f;

                var jd = new WeldJointDef();
                var frequencyHz = 5.0f;
                var dampingRatio = 0.7f;

                var prevBody = ground;
                for (var i = 0; i < 3; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-14.0f + FP.Two * i, 15.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);

                    var anchor = new TSVector2(-15.0f + FP.Two * i, 15.0f);
                    jd.Initialize(prevBody, body, anchor);
                    JointUtils.AngularStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, prevBody, body);
                    World.CreateJoint(jd);

                    prevBody = body;
                }
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.125f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 20.0f;

                var jd = new WeldJointDef();

                var prevBody = ground;
                for (var i = 0; i < Count; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-4.5f + FP.One * i, 5.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);

                    if (i > 0)
                    {
                        var anchor = new TSVector2(-5.0f + FP.One * i, 5.0f);
                        jd.Initialize(prevBody, body, anchor);
                        World.CreateJoint(jd);
                    }

                    prevBody = body;
                }
            }

            {
                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.125f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = 20.0f;

                var jd = new WeldJointDef();
                var frequencyHz = 8.0f;
                var dampingRatio = 0.7f;

                var prevBody = ground;
                for (var i = 0; i < Count; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(5.5f + FP.One * i, 10.0f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);

                    if (i > 0)
                    {
                        var anchor = new TSVector2(5.0f + FP.One * i, 10.0f);
                        jd.Initialize(prevBody, body, anchor);
                        JointUtils.AngularStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                        World.CreateJoint(jd);
                    }

                    prevBody = body;
                }
            }

            for (var i = 0; i < 2; ++i)
            {
                var vertices = new TSVector2 [3];
                vertices[0].Set(-0.5f, FP.Zero);
                vertices[1].Set(0.5f, FP.Zero);
                vertices[2].Set(FP.Zero, 1.5f);

                var shape = new PolygonShape();
                shape.Set(vertices);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.One;

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-8.0f + 8.0f * i, 12.0f);
                var body = World.CreateBody(bd);
                body.CreateFixture(fd);
            }

            for (var i = 0; i < 2; ++i)
            {
                var shape = new CircleShape();
                shape.Radius = 0.5f;

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.One;

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-6.0f + 6.0f * i, 10.0f);
                var body = World.CreateBody(bd);
                body.CreateFixture(fd);
            }
        }
    }
}