using System;
using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Contacts;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Breakable")]
    public class Breakable : TestBase
    {
        private FP _angularVelocity;

        private Body _body1;

        private bool _break;

        private bool _broke;

        private Fixture _piece1;

        private Fixture _piece2;

        private PolygonShape _shape1 = new PolygonShape();

        private PolygonShape _shape2 = new PolygonShape();

        private TSVector2 _velocity;

        public Breakable()
        {
            // Ground body
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            // Breakable dynamic body
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 40.0f);
                bd.Angle = 0.25f * Settings.Pi;
                _body1 = World.CreateBody(bd);

                _shape1.SetAsBox(0.5f, 0.5f, new TSVector2(-0.5f, FP.Zero), FP.Zero);
                _piece1 = _body1.CreateFixture(_shape1, FP.One);

                _shape2.SetAsBox(0.5f, 0.5f, new TSVector2(0.5f, FP.Zero), FP.Zero);
                _piece2 = _body1.CreateFixture(_shape2, FP.One);
            }

            _break = false;
            _broke = false;
        }

        /// <inheritdoc />
        public override void PostSolve(Contact contact, in ContactImpulse impulse)
        {
            base.PostSolve(contact, impulse);

            if (_broke)
            {
                // The body already broke.
                return;
            }

            // Should the body break?
            var count = contact.Manifold.PointCount;

            var maxImpulse = FP.Zero;
            for (var i = 0; i < count; ++i)
            {
                maxImpulse = FP.Max(maxImpulse, impulse.NormalImpulses[i]);
            }

            if (maxImpulse > 40.0f)
            {
                // Flag the body for breaking.
                _break = true;
            }
        }

        private void Break()
        {
            // Create two bodies from one.
            var body1 = _piece1.Body;
            var center = body1.GetWorldCenter();

            body1.DestroyFixture(_piece2);
            _piece2 = null;

            var bd = new BodyDef();
            bd.BodyType = BodyType.DynamicBody;
            bd.Position = body1.GetPosition();
            bd.Angle = body1.GetAngle();

            var body2 = World.CreateBody(bd);
            _piece2 = body2.CreateFixture(_shape2, FP.One);

            // Compute consistent velocities for new bodies based on
            // cached velocity.
            var center1 = body1.GetWorldCenter();
            var center2 = body2.GetWorldCenter();

            var velocity1 = _velocity + MathUtils.Cross(_angularVelocity, center1 - center);
            var velocity2 = _velocity + MathUtils.Cross(_angularVelocity, center2 - center);

            body1.SetAngularVelocity(_angularVelocity);
            body1.SetLinearVelocity(velocity1);

            body2.SetAngularVelocity(_angularVelocity);
            body2.SetLinearVelocity(velocity2);
        }

        protected override void PreStep()
        {
            if (_break)
            {
                Break();
                _broke = true;
                _break = false;
            }

            // Cache velocities to improve movement on breakage.
            if (_broke == false)
            {
                _velocity = _body1.LinearVelocity;
                _angularVelocity = _body1.AngularVelocity;
            }
        }
    }
}