using TrueSync;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Contacts;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Conveyor Belt")]
    public class ConveyorBelt : TestBase
    {
        private Fixture _platform;

        public ConveyorBelt()
        {
            // Ground
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            // Platform
            {
                var bd = new BodyDef();
                bd.Position.Set(-5.0f, 5.0f);
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(10.0f, 0.5f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Friction = 0.8f;
                _platform = body.CreateFixture(fd);
            }

            // Boxes
            for (var i = 0; i < 5; ++i)
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-10.0f + FP.Two * i, 7.0f);
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);
                body.CreateFixture(shape, 20.0f);
            }
        }

        /// <inheritdoc />
        public override void PreSolve(Contact contact, in Manifold oldManifold)
        {
            base.PreSolve(contact, oldManifold);

            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;

            if (fixtureA == _platform)
            {
                contact.SetTangentSpeed(5.0f);
            }

            if (fixtureB == _platform)
            {
                contact.SetTangentSpeed(-5.0f);
            }
        }
    }
}