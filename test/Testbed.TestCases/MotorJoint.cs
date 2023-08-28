using System;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;
using Color = FixedBox2D.Common.Color;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Joints", "Motor Joint")]
    public class MotorJoint : TestBase
    {
        private bool _go;

        private FP _time;

        private FixedBox2D.Dynamics.Joints.MotorJoint _joint;

        public MotorJoint()
        {
            Body ground;
            {
                var bd = new BodyDef();
                ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));

                var fd = new FixtureDef();
                fd.Shape = shape;

                ground.CreateFixture(fd);
            }

            // Define motorized body
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 8.0f);
                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.Two, 0.5f);

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Friction = 0.6f;
                fd.Density = FP.Two;
                body.CreateFixture(fd);

                var mjd = new MotorJointDef();
                mjd.Initialize(ground, body);
                mjd.MaxForce = 1000.0f;
                mjd.MaxTorque = 1000.0f;
                _joint = (FixedBox2D.Dynamics.Joints.MotorJoint)World.CreateJoint(mjd);
            }

            _go = false;
            _time = FP.Zero;
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.S)
            {
                _go = !_go;
            }
        }

        protected override void PreStep()
        {
            if (_go && 1 / TestSettings.Hertz > FP.Zero)
            {
                _time += 1 / TestSettings.Hertz;
            }

            _linearOffset = new TSVector2
            {
                X = 6.0f * FP.Sin(FP.Two * _time), Y = 8.0f + 4.0f * FP.Sin(FP.One * _time)
            };

            var angularOffset = 4.0f * _time;

            _joint.SetLinearOffset(_linearOffset);
            _joint.SetAngularOffset(angularOffset);
        }

        private TSVector2 _linearOffset;

        protected override void OnRender()
        {
            DrawString("Keys: (s) pause");

            Drawer.DrawPoint(_linearOffset, 4.0f, Color.FromArgb(230, 230, 230));
        }
    }
}