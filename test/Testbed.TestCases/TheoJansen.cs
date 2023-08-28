using System;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Theo Jansen")]
    public class TheoJansen : TestBase
    {
        private Body _chassis;

        private RevoluteJoint _motorJoint;

        private bool _motorOn;

        private FP _motorSpeed;

        private TSVector2 _offset;

        private Body _wheel;

        public TheoJansen()
        {
            _offset.Set(FP.Zero, 8.0f);
            _motorSpeed = FP.Two;
            _motorOn = true;
            var pivot = new TSVector2(FP.Zero, 0.8f);

            // Ground
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-50.0f, FP.Zero), new TSVector2(50.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);

                shape.SetTwoSided(new TSVector2(-50.0f, FP.Zero), new TSVector2(-50.0f, 10.0f));
                ground.CreateFixture(shape, FP.Zero);

                shape.SetTwoSided(new TSVector2(50.0f, FP.Zero), new TSVector2(50.0f, 10.0f));
                ground.CreateFixture(shape, FP.Zero);
            }

            // Balls
            for (var i = 0; i < 40; ++i)
            {
                var shape = new CircleShape();
                shape.Radius = 0.25f;

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(-40.0f + FP.Two * i, 0.5f);

                var body = World.CreateBody(bd);
                body.CreateFixture(shape, FP.One);
            }

            // Chassis
            {
                var shape = new PolygonShape();
                shape.SetAsBox(2.5f, FP.One);

                var sd = new FixtureDef();
                sd.Density = FP.One;
                sd.Shape = shape;
                sd.Filter.GroupIndex = -1;
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = pivot + _offset;
                _chassis = World.CreateBody(bd);
                _chassis.CreateFixture(sd);
            }

            {
                var shape = new CircleShape();
                shape.Radius = 1.6f;
                var sd = new FixtureDef();
                sd.Density = FP.One;
                sd.Shape = shape;
                sd.Filter.GroupIndex = -1;
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position = pivot + _offset;
                _wheel = World.CreateBody(bd);
                _wheel.CreateFixture(sd);
            }

            {
                var jd = new RevoluteJointDef();
                jd.Initialize(_wheel, _chassis, pivot + _offset);
                jd.CollideConnected = false;
                jd.MotorSpeed = _motorSpeed;
                jd.MaxMotorTorque = 400.0f;
                jd.EnableMotor = _motorOn;
                _motorJoint = (RevoluteJoint)World.CreateJoint(jd);
            }

            TSVector2 wheelAnchor;

            wheelAnchor = pivot + new TSVector2(FP.Zero, -0.8f);

            CreateLeg(-FP.One, wheelAnchor);
            CreateLeg(FP.One, wheelAnchor);

            _wheel.SetTransform(_wheel.GetPosition(), 120.0f * (FP)Math.PI / 180.0f);
            CreateLeg(-FP.One, wheelAnchor);
            CreateLeg(FP.One, wheelAnchor);

            _wheel.SetTransform(_wheel.GetPosition(), -120.0f * (FP)Math.PI / 180.0f);
            CreateLeg(-FP.One, wheelAnchor);
            CreateLeg(FP.One, wheelAnchor);
        }

        private void CreateLeg(FP s, TSVector2 wheelAnchor)
        {
            var p1 = new TSVector2(5.4f * s, -6.1f);
            var p2 = new TSVector2(7.2f * s, -1.2f);
            var p3 = new TSVector2(4.3f * s, -1.9f);
            var p4 = new TSVector2(3.1f * s, 0.8f);
            var p5 = new TSVector2(6.0f * s, 1.5f);
            var p6 = new TSVector2(2.5f * s, 3.7f);

            var fd1 = new FixtureDef {Filter = {GroupIndex = -1}, Density = FP.One};
            var fd2 = new FixtureDef {Filter = {GroupIndex = -1}, Density = FP.One};

            var poly1 = new PolygonShape();
            var poly2 = new PolygonShape();

            if (s > FP.Zero)
            {
                var vertices = new TSVector2[3];

                vertices[0] = p1;
                vertices[1] = p2;
                vertices[2] = p3;
                poly1.Set(vertices);

                vertices[0] = TSVector2.Zero;
                vertices[1] = p5 - p4;
                vertices[2] = p6 - p4;
                poly2.Set(vertices);
            }
            else
            {
                var vertices = new TSVector2[3];

                vertices[0] = p1;
                vertices[1] = p3;
                vertices[2] = p2;
                poly1.Set(vertices);

                vertices[0] = TSVector2.Zero;
                vertices[1] = p6 - p4;
                vertices[2] = p5 - p4;
                poly2.Set(vertices);
            }

            fd1.Shape = poly1;
            fd2.Shape = poly2;

            var bd1 = new BodyDef();
            var bd2 = new BodyDef();
            bd1.BodyType = BodyType.DynamicBody;
            bd2.BodyType = BodyType.DynamicBody;
            bd1.Position = _offset;
            bd2.Position = p4 + _offset;

            bd1.AngularDamping = 10.0f;
            bd2.AngularDamping = 10.0f;

            var body1 = World.CreateBody(bd1);
            var body2 = World.CreateBody(bd2);

            body1.CreateFixture(fd1);
            body2.CreateFixture(fd2);

            {
                var jd = new DistanceJointDef();

                // Using a soft distance constraint can reduce some jitter.
                // It also makes the structure seem a bit more fluid by
                // acting like a suspension system.
                var dampingRatio = 0.5f;
                var frequencyHz = 10.0f;

                jd.Initialize(body1, body2, p2 + _offset, p5 + _offset);
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                World.CreateJoint(jd);

                jd.Initialize(body1, body2, p3 + _offset, p4 + _offset);
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                World.CreateJoint(jd);

                jd.Initialize(body1, _wheel, p3 + _offset, wheelAnchor + _offset);
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                World.CreateJoint(jd);

                jd.Initialize(body2, _wheel, p6 + _offset, wheelAnchor + _offset);
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                World.CreateJoint(jd);
            }

            {
                var jd = new RevoluteJointDef();
                jd.Initialize(body2, _chassis, p4 + _offset);
                World.CreateJoint(jd);
            }
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.A)
            {
                _motorJoint.SetMotorSpeed(-_motorSpeed);
            }

            if (keyInput.Key == KeyCodes.S)
            {
                _motorJoint.SetMotorSpeed(FP.Zero);
            }

            if (keyInput.Key == KeyCodes.D)
            {
                _motorJoint.SetMotorSpeed(_motorSpeed);
            }

            if (keyInput.Key == KeyCodes.M)
            {
                _motorJoint.EnableMotor(!_motorJoint.IsMotorEnabled());
            }
        }

        protected override void OnRender()
        {
            DrawString("Keys: left = a, brake = s, right = d, toggle motor = m");
        }
    }
}