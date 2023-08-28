using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Compound Shapes")]
    public class CompoundShapes : TestBase
    {
        Body m_table1;

        Body m_table2;

        Body m_ship1;

        Body m_ship2;

        public CompoundShapes()
        {
            {
                BodyDef bd = new BodyDef();
                bd.Position.Set(FP.Zero, FP.Zero);
                Body body = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(50.0f, FP.Zero), new TSVector2(-50.0f, FP.Zero));

                body.CreateFixture(shape, FP.Zero);
            }

            // Table 1
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-15.0f, FP.One);
                m_table1 = World.CreateBody(bd);

                PolygonShape top = new PolygonShape();
                top.SetAsBox(3.0f, 0.5f, new TSVector2(FP.Zero, 3.5f), FP.Zero);

                PolygonShape leftLeg = new PolygonShape();
                leftLeg.SetAsBox(0.5f, 1.5f, new TSVector2(-2.5f, 1.5f), FP.Zero);

                PolygonShape rightLeg = new PolygonShape();
                rightLeg.SetAsBox(0.5f, 1.5f, new TSVector2(2.5f, 1.5f), FP.Zero);

                m_table1.CreateFixture(top, FP.Two);
                m_table1.CreateFixture(leftLeg, FP.Two);
                m_table1.CreateFixture(rightLeg, FP.Two);
            }

            // Table 2
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-5.0f, FP.One);
                m_table2 = World.CreateBody(bd);

                PolygonShape top = new PolygonShape();
                top.SetAsBox(3.0f, 0.5f, new TSVector2(FP.Zero, 3.5f), FP.Zero);

                PolygonShape leftLeg = new PolygonShape();
                leftLeg.SetAsBox(0.5f, FP.Two, new TSVector2(-2.5f, FP.Two), FP.Zero);

                PolygonShape rightLeg = new PolygonShape();
                rightLeg.SetAsBox(0.5f, FP.Two, new TSVector2(2.5f, FP.Two), FP.Zero);

                m_table2.CreateFixture(top, FP.Two);
                m_table2.CreateFixture(leftLeg, FP.Two);
                m_table2.CreateFixture(rightLeg, FP.Two);
            }

            // Spaceship 1
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(5.0f, FP.One);
                m_ship1 = World.CreateBody(bd);

                TSVector2[] vertices = new TSVector2[3];

                PolygonShape left = new PolygonShape();
                vertices[0].Set(-FP.Two, FP.Zero);
                vertices[1].Set(FP.Zero, 4.0f / 3.0f);
                vertices[2].Set(FP.Zero, 4.0f);
                left.Set(vertices, 3);

                PolygonShape right = new PolygonShape();
                vertices[0].Set(FP.Two, FP.Zero);
                vertices[1].Set(FP.Zero, 4.0f / 3.0f);
                vertices[2].Set(FP.Zero, 4.0f);
                right.Set(vertices, 3);

                m_ship1.CreateFixture(left, FP.Two);
                m_ship1.CreateFixture(right, FP.Two);
            }

            // Spaceship 2
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(15.0f, FP.One);
                m_ship2 = World.CreateBody(bd);

                TSVector2[] vertices = new TSVector2[3];

                PolygonShape left = new PolygonShape();
                vertices[0].Set(-FP.Two, FP.Zero);
                vertices[1].Set(FP.One, FP.Two);
                vertices[2].Set(FP.Zero, 4.0f);
                left.Set(vertices, 3);

                PolygonShape right = new PolygonShape();
                vertices[0].Set(FP.Two, FP.Zero);
                vertices[1].Set(-FP.One, FP.Two);
                vertices[2].Set(FP.Zero, 4.0f);
                right.Set(vertices, 3);

                m_ship2.CreateFixture(left, FP.Two);
                m_ship2.CreateFixture(right, FP.Two);
            }
        }

        protected void Spawn()
        {
            // Table 1 obstruction
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = m_table1.GetPosition();
                bd.Angle = m_table1.GetAngle();

                Body body = World.CreateBody(bd);

                PolygonShape box = new PolygonShape();
                box.SetAsBox(4.0f, FP.EN1, new TSVector2(FP.Zero, 3.0f), FP.Zero);

                body.CreateFixture(box, FP.Two);
            }

            // Table 2 obstruction
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = m_table2.GetPosition();
                bd.Angle = m_table2.GetAngle();

                Body body = World.CreateBody(bd);

                PolygonShape box = new PolygonShape();
                box.SetAsBox(4.0f, FP.EN1, new TSVector2(FP.Zero, 3.0f), FP.Zero);

                body.CreateFixture(box, FP.Two);
            }

            // Ship 1 obstruction
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = m_ship1.GetPosition();
                bd.Angle = m_ship1.GetAngle();
                bd.GravityScale = FP.Zero;

                Body body = World.CreateBody(bd);

                CircleShape circle = new CircleShape();
                circle.Radius = 0.5f;
                circle.Position.Set(FP.Zero, FP.Two);

                body.CreateFixture(circle, FP.Two);
            }

            // Ship 2 obstruction
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = m_ship2.GetPosition();
                bd.Angle = m_ship2.GetAngle();
                bd.GravityScale = FP.Zero;

                Body body = World.CreateBody(bd);

                CircleShape circle = new CircleShape();
                circle.Radius = 0.5f;
                circle.Position.Set(FP.Zero, FP.Two);

                body.CreateFixture(circle, FP.Two);
            }
        }
    }
}