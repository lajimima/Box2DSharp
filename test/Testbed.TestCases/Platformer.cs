using TrueSync;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Contacts;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Platformer")]
    public class Platformer : TestBase
    {
        private Fixture _character;

        private Fixture _platform;

        private FP _radius;

        private FP _top;

        public Platformer()
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
                bd.Position.Set(FP.Zero, 10.0f);
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(3.0f, 0.5f);
                _platform = body.CreateFixture(shape, FP.Zero);

                _top = 10.0f + 0.5f;
            }

            // Actor
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 12.0f);
                var body = World.CreateBody(bd);

                _radius = 0.5f;
                var shape = new CircleShape();
                shape.Radius = _radius;
                _character = body.CreateFixture(shape, 20.0f);

                body.SetLinearVelocity(new TSVector2(FP.Zero, -50.0f));
            }
        }

        /// <inheritdoc />
        public override void PreSolve(Contact contact, in Manifold oldManifold)
        {
            base.PreSolve(contact, oldManifold);
            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;

            if (fixtureA != _platform && fixtureA != _character)
            {
                return;
            }

            if (fixtureB != _platform && fixtureB != _character)
            {
                return;
            }

            var position = _character.Body.GetPosition();

            if (position.Y < _top + _radius - 3.0f * Settings.LinearSlop)
            {
                contact.SetEnabled(false);
            }
        }

        protected override void OnRender()
        {
            var v = _character.Body.LinearVelocity;
            DrawString($"Character Linear Velocity: {v.Y}");
        }
    }
}