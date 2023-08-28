using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Car")]
    public class Car : TestBase
    {
        private Body _car;

        private Body _wheel1;

        private Body _wheel2;

        private FP _speed;

        private WheelJoint _spring1;

        private WheelJoint _spring2;

        public Car()
        {
            _speed = 50.0f;

            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var shape = new EdgeShape();

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.Zero;
                fd.Friction = 0.6f;

                shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));
                ground.CreateFixture(fd);

                FP[] hs = {0.25f, FP.One, 4.0f, FP.Zero, FP.Zero, -FP.One, -FP.Two, -FP.Two, -1.25f, FP.Zero};

                FP x = 20.0f, y1 = FP.Zero, dx = 5.0f;

                for (var i = 0; i < 10; ++i)
                {
                    var y2 = hs[i];
                    shape.SetTwoSided(new TSVector2(x, y1), new TSVector2(x + dx, y2));
                    ground.CreateFixture(fd);
                    y1 = y2;
                    x += dx;
                }

                for (var i = 0; i < 10; ++i)
                {
                    var y2 = hs[i];
                    shape.SetTwoSided(new TSVector2(x, y1), new TSVector2(x + dx, y2));
                    ground.CreateFixture(fd);
                    y1 = y2;
                    x += dx;
                }

                shape.SetTwoSided(new TSVector2(x, FP.Zero), new TSVector2(x + 40.0f, FP.Zero));
                ground.CreateFixture(fd);

                x += 80.0f;
                shape.SetTwoSided(new TSVector2(x, FP.Zero), new TSVector2(x + 40.0f, FP.Zero));
                ground.CreateFixture(fd);

                x += 40.0f;
                shape.SetTwoSided(new TSVector2(x, FP.Zero), new TSVector2(x + 10.0f, 5.0f));
                ground.CreateFixture(fd);

                x += 20.0f;
                shape.SetTwoSided(new TSVector2(x, FP.Zero), new TSVector2(x + 40.0f, FP.Zero));
                ground.CreateFixture(fd);

                x += 40.0f;
                shape.SetTwoSided(new TSVector2(x, FP.Zero), new TSVector2(x, 20.0f));
                ground.CreateFixture(fd);
            }

            // Teeter
            {
                var bd = new BodyDef();
                bd.Position.Set(140.0f, FP.One);
                bd.BodyType = BodyType.DynamicBody;
                var body = World.CreateBody(bd);

                var box = new PolygonShape();
                box.SetAsBox(10.0f, 0.25f);
                body.CreateFixture(box, FP.One);

                var jd = new RevoluteJointDef();
                jd.Initialize(ground, body, body.GetPosition());
                jd.LowerAngle = -8.0f * Settings.Pi / 180.0f;
                jd.UpperAngle = 8.0f * Settings.Pi / 180.0f;
                jd.EnableLimit = true;
                World.CreateJoint(jd);

                body.ApplyAngularImpulse(100.0f, true);
            }

            // Bridge
            {
                var N = 20;
                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, 0.125f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.One;
                fd.Friction = 0.6f;

                var jd = new RevoluteJointDef();

                var prevBody = ground;
                for (var i = 0; i < N; ++i)
                {
                    var bd = new BodyDef();
                    bd.BodyType = BodyType.DynamicBody;
                    bd.Position.Set(161.0f + FP.Two * i, -0.125f);
                    var body = World.CreateBody(bd);
                    body.CreateFixture(fd);

                    var anchor = new TSVector2(160.0f + FP.Two * i, -0.125f);
                    jd.Initialize(prevBody, body, anchor);
                    World.CreateJoint(jd);

                    prevBody = body;
                }

                {
                    var anchor = new TSVector2(160.0f + FP.Two * N, -0.125f);
                    jd.Initialize(prevBody, ground, anchor);
                    World.CreateJoint(jd);
                }
            }

            // Boxes
            {
                var box = new PolygonShape();
                box.SetAsBox(0.5f, 0.5f);

                Body body = null;
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;

                bd.Position.Set(230.0f, 0.5f);
                body = World.CreateBody(bd);
                body.CreateFixture(box, 0.5f);

                bd.Position.Set(230.0f, 1.5f);
                body = World.CreateBody(bd);
                body.CreateFixture(box, 0.5f);

                bd.Position.Set(230.0f, 2.5f);
                body = World.CreateBody(bd);
                body.CreateFixture(box, 0.5f);

                bd.Position.Set(230.0f, 3.5f);
                body = World.CreateBody(bd);
                body.CreateFixture(box, 0.5f);

                bd.Position.Set(230.0f, 4.5f);
                body = World.CreateBody(bd);
                body.CreateFixture(box, 0.5f);
            }

            // Car
            {
                var chassis = new PolygonShape();
                var vertices = new TSVector2[8];
                vertices[0].Set(-1.5f, -0.5f);
                vertices[1].Set(1.5f, -0.5f);
                vertices[2].Set(1.5f, FP.Zero);
                vertices[3].Set(FP.Zero, 0.9f);
                vertices[4].Set(-1.15f, 0.9f);
                vertices[5].Set(-1.5f, 0.2f);
                chassis.Set(vertices);

                var circle = new CircleShape();
                circle.Radius = 0.4f;

                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, FP.One);
                _car = World.CreateBody(bd);
                _car.CreateFixture(chassis, FP.One);

                var fd = new FixtureDef();
                fd.Shape = circle;
                fd.Density = FP.One;
                fd.Friction = 0.9f;

                bd.Position.Set(-FP.One, 0.35f);
                _wheel1 = World.CreateBody(bd);
                _wheel1.CreateFixture(fd);

                bd.Position.Set(FP.One, 0.4f);
                _wheel2 = World.CreateBody(bd);
                _wheel2.CreateFixture(fd);

                var jd = new WheelJointDef();
                var axis = new TSVector2(FP.Zero, FP.One);
                var mass1 = _wheel1.Mass;
                var mass2 = _wheel2.Mass;

                var hertz = 4.0f;
                var dampingRatio = 0.7f;
                var omega = FP.Two * Settings.Pi * hertz;
                jd.Initialize(_car, _wheel1, _wheel1.GetPosition(), axis);
                jd.MotorSpeed = FP.Zero;
                jd.MaxMotorTorque = 20.0f;
                jd.EnableMotor = true;
                jd.Stiffness = mass1 * omega * omega;
                jd.Damping = FP.Two * mass1 * dampingRatio * omega;
                jd.LowerTranslation = -0.25f;
                jd.UpperTranslation = 0.25f;
                jd.EnableLimit = true;
                _spring1 = (WheelJoint)World.CreateJoint(jd);

                jd.Initialize(_car, _wheel2, _wheel2.GetPosition(), axis);
                jd.MotorSpeed = FP.Zero;
                jd.MaxMotorTorque = 10.0f;
                jd.EnableMotor = false;
                jd.Stiffness = mass2 * omega * omega;
                jd.Damping = FP.Two * mass2 * dampingRatio * omega;
                jd.LowerTranslation = -0.25f;
                jd.UpperTranslation = 0.25f;
                jd.EnableLimit = true;
                _spring2 = (WheelJoint)World.CreateJoint(jd);
            }
        }

        protected override void PreStep()
        {
            var p = Global.Camera.Center;
            p.X = _car.GetPosition().X;
            Global.Camera.Center = p;
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            switch (keyInput.Key)
            {
            case KeyCodes.A:
                _spring1.SetMotorSpeed(_speed);
                break;
            case KeyCodes.S:
                _spring1.SetMotorSpeed(FP.Zero);
                break;
            case KeyCodes.D:
                _spring1.SetMotorSpeed(-_speed);
                break;
            }
        }

        protected override void OnRender()
        {
            DrawString("Keys: left = a, brake = s, right = d");
        }
    }
}