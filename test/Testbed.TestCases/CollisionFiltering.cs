using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Collision Filtering")]
    public class CollisionFiltering : TestBase
    {
        private const short SmallGroup = 1;

        private const short LargeGroup = -1;

        private const ushort DefaultCategory = 0x0001;

        private const ushort TriangleCategory = 0x0002;

        private const ushort BoxCategory = 0x0004;

        private const ushort CircleCategory = 0x0008;

        private const ushort TriangleMask = 0xFFFF;

        private const ushort BoxMask = 0xFFFF ^ TriangleCategory;

        private const ushort CircleMask = 0xFFFF;

        public CollisionFiltering()
        {
            {
                // Ground body
                {
                    var shape = new EdgeShape();
                    shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));

                    var sd = new FixtureDef();
                    sd.Shape = shape;
                    sd.Friction = 0.3f;

                    var bd = new BodyDef();
                    var ground = World.CreateBody(bd);
                    ground.CreateFixture(sd);
                }

                // Small triangle
                var vertices = new TSVector2[3];
                vertices[0].Set(-FP.One, FP.Zero);
                vertices[1].Set(FP.One, FP.Zero);
                vertices[2].Set(FP.Zero, FP.Two);
                var polygon = new PolygonShape();
                polygon.Set(vertices);

                var triangleShapeDef = new FixtureDef();
                triangleShapeDef.Shape = polygon;
                triangleShapeDef.Density = FP.One;
                triangleShapeDef.Filter.GroupIndex = SmallGroup;
                triangleShapeDef.Filter.CategoryBits = TriangleCategory;
                triangleShapeDef.Filter.MaskBits = TriangleMask;

                var triangleBodyDef = new BodyDef();
                triangleBodyDef.BodyType = BodyType.DynamicBody;
                triangleBodyDef.Position.Set(-5.0f, FP.Two);

                var body1 = World.CreateBody(triangleBodyDef);
                body1.CreateFixture(triangleShapeDef);

                // Large triangle (recycle definitions)
                vertices[0] *= FP.Two;
                vertices[1] *= FP.Two;
                vertices[2] *= FP.Two;
                polygon.Set(vertices);
                triangleShapeDef.Filter.GroupIndex = LargeGroup;
                triangleBodyDef.Position.Set(-5.0f, 6.0f);
                triangleBodyDef.FixedRotation = true; // look at me!

                var body2 = World.CreateBody(triangleBodyDef);
                body2.CreateFixture(triangleShapeDef);

                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(-5.0f, 10.0f);
                    var body = World.CreateBody(bd);

                    var p = new PolygonShape();
                    p.SetAsBox(0.5f, FP.One);
                    body.CreateFixture(p, FP.One);

                    var jd = new PrismaticJointDef();
                    jd.BodyA = body2;
                    jd.BodyB = body;
                    jd.EnableLimit = true;
                    jd.LocalAnchorA.Set(FP.Zero, 4.0f);
                    jd.LocalAnchorB.SetZero();
                    jd.LocalAxisA.Set(FP.Zero, FP.One);
                    jd.LowerTranslation = -FP.One;
                    jd.UpperTranslation = FP.One;

                    World.CreateJoint(jd);
                }

                // Small box
                polygon.SetAsBox(FP.One, 0.5f);
                var boxShapeDef = new FixtureDef();
                boxShapeDef.Shape = polygon;
                boxShapeDef.Density = FP.One;
                boxShapeDef.Restitution = FP.EN1;
                boxShapeDef.Filter.GroupIndex = SmallGroup;
                boxShapeDef.Filter.CategoryBits = BoxCategory;
                boxShapeDef.Filter.MaskBits = BoxMask;

                var boxBodyDef = triangleBodyDef;
                boxBodyDef.BodyType = BodyType.DynamicBody;
                boxBodyDef.Position.Set(FP.Zero, FP.Two);

                var body3 = World.CreateBody(boxBodyDef);
                body3.CreateFixture(boxShapeDef);

                // Large box (recycle definitions)
                polygon.SetAsBox(FP.Two, FP.One);
                boxShapeDef.Filter.GroupIndex = LargeGroup;
                boxBodyDef.Position.Set(FP.Zero, 6.0f);

                var body4 = World.CreateBody(boxBodyDef);
                body4.CreateFixture(boxShapeDef);

                // Small circle
                var circle = new CircleShape();
                circle.Radius = FP.One;

                var circleShapeDef = new FixtureDef();
                circleShapeDef.Shape = circle;
                circleShapeDef.Density = FP.One;
                circleShapeDef.Filter.GroupIndex = SmallGroup;
                circleShapeDef.Filter.CategoryBits = CircleCategory;
                circleShapeDef.Filter.MaskBits = CircleMask;

                var circleBodyDef = new BodyDef();
                circleBodyDef.BodyType = BodyType.DynamicBody;
                circleBodyDef.Position.Set(5.0f, FP.Two);

                var body5 = World.CreateBody(circleBodyDef);
                body5.CreateFixture(circleShapeDef);

                // Large circle
                circle.Radius *= FP.Two;
                circleShapeDef.Filter.GroupIndex = LargeGroup;
                circleBodyDef.Position.Set(5.0f, 6.0f);

                var body6 = World.CreateBody(circleBodyDef);
                body6.CreateFixture(circleShapeDef);
            }
        }
    }
}