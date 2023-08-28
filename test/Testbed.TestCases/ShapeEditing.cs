using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Shape Editing")]
    public class ShapeEditing : TestBase
    {
        private Body _body;

        private Fixture _fixture1;

        private Fixture _fixture2;

        private bool _sensor;

        public ShapeEditing()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 10.0f);
                _body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(4.0f, 4.0f, new TSVector2(FP.Zero, FP.Zero), FP.Zero);
                _fixture1 = _body.CreateFixture(shape, 10.0f);
            }
            _fixture2 = null;
            _sensor = false;
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.C)
            {
                if (_fixture2 == null)
                {
                    var shape = new CircleShape();
                    shape.Radius = 3.0f;
                    shape.Position.Set(0.5f, -4.0f);
                    _fixture2 = _body.CreateFixture(shape, 10.0f);
                    _body.IsAwake = true;
                }
            }

            if (keyInput.Key == KeyCodes.D)
            {
                if (_fixture2 != null)
                {
                    _body.DestroyFixture(_fixture2);
                    _fixture2 = null;
                    _body.IsAwake = true;
                }
            }

            if (keyInput.Key == KeyCodes.S)
            {
                if (_fixture2 != null)
                {
                    _sensor = !_sensor;
                    _fixture2.IsSensor = _sensor;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            DrawString("Press: (c) create a shape, (d) destroy a shape. (s) set sensor");
            DrawString($"sensor = {_sensor}");
        }
    }
}