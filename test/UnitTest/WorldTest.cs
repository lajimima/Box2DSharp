using TrueSync;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Contacts;
using Shouldly;
using Xunit;

namespace UnitTest
{
    public class MyContactListener : IContactListener
    {
        public bool begin_contact = false;

        /// <inheritdoc />
        public void BeginContact(Contact contact)
        {
            begin_contact = true;
        }

        /// <inheritdoc />
        public void EndContact(Contact contact)
        { }

        /// <inheritdoc />
        public void PreSolve(Contact contact, in Manifold oldManifold)
        { }

        /// <inheritdoc />
        public void PostSolve(Contact contact, in ContactImpulse impulse)
        { }
    }

    public class WorldTest
    {
        [Fact(DisplayName = "begin contact")]
        public void BeginContact()
        {
            World world = new World(new TSVector2(0.0f, -10.0f));
            MyContactListener listener = new MyContactListener();
            world.SetContactListener(listener);

            CircleShape circle = new CircleShape();
            circle.Radius = 5;

            BodyDef bodyDef = new BodyDef();
            bodyDef.BodyType = BodyType.DynamicBody;

            var bodyA = world.CreateBody(bodyDef);
            var bodyB = world.CreateBody(bodyDef);
            bodyA.CreateFixture(circle, 0.0f);
            bodyB.CreateFixture(circle, 0.0f);

            bodyA.SetTransform(new TSVector2(0f, 0f), 0f);
            bodyB.SetTransform(new TSVector2(100f, 0f), 0f);

            const FP timeStep = 1f / 60f;
            const int velocityIterations = 6;
            const int positionIterations = 2;

            world.Step(timeStep, velocityIterations, positionIterations);

            world.ContactManager.ContactList.ShouldBeEmpty();
            listener.begin_contact.ShouldBeFalse();

            bodyB.SetTransform(new TSVector2(1f, 0f), 0f);

            world.Step(timeStep, velocityIterations, positionIterations);

            world.ContactManager.ContactList.ShouldNotBeEmpty();
            listener.begin_contact.ShouldBeTrue();
        }
    }
}