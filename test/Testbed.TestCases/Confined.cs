using System.Threading;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Solver", "Confined")]
    public class Confined : TestBase
    {
        private readonly int e_columnCount = 0;

        private readonly int e_rowCount = 0;

        private bool _autoCreate = false;

        public Confined()
        {
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();

                // Floor
                shape.SetTwoSided(new TSVector2(-10.0f, FP.Zero), new TSVector2(10.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);

                // Left wall
                shape.SetTwoSided(new TSVector2(-10.0f, FP.Zero), new TSVector2(-10.0f, 20.0f));
                ground.CreateFixture(shape, FP.Zero);

                // Right wall
                shape.SetTwoSided(new TSVector2(10.0f, FP.Zero), new TSVector2(10.0f, 20.0f));
                ground.CreateFixture(shape, FP.Zero);

                // Roof
                shape.SetTwoSided(new TSVector2(-10.0f, 20.0f), new TSVector2(10.0f, 20.0f));
                ground.CreateFixture(shape, FP.Zero);
            }
            {
                var radius = 0.5f;
                var shape = new CircleShape();
                shape.Position.SetZero();
                shape.Radius = radius;

                var fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.One;
                fd.Friction = FP.EN1;

                for (var j = 0; j < e_columnCount; ++j)
                {
                    for (var i = 0; i < e_rowCount; ++i)
                    {
                        var bd = new BodyDef();
                        bd.BodyType = BodyType.DynamicBody;
                        bd.Position.Set(-10.0f + (2.1f * j + FP.One + 0.01f * i) * radius, (FP.Two * i + FP.One) * radius);
                        var body = World.CreateBody(bd);

                        body.CreateFixture(fd);
                    }
                }
            }
            World.Gravity = new TSVector2(FP.Zero, FP.Zero);
        }

        private void CreateCircle()
        {
            var radius = FP.Two;
            var shape = new CircleShape();
            shape.Position.SetZero();
            shape.Radius = radius;

            var fd = new FixtureDef();
            fd.Shape = shape;
            fd.Density = FP.One;
            fd.Friction = FP.Zero;

            var p = new TSVector2(RandomFloat(), 3.0f + RandomFloat());
            var bd = new BodyDef();
            bd.BodyType = BodyType.DynamicBody;
            bd.Position = p;

            var body = World.CreateBody(bd);

            body.CreateFixture(fd);
        }

        /// <inheritdoc />
        protected override void PreStep()
        {
            if (!_autoCreate)
            {
                return;
            }

            var sleeping = true;
            foreach (var b in World.BodyList)
            {
                if (b.BodyType != BodyType.DynamicBody)
                {
                    continue;
                }

                if (b.IsAwake)
                {
                    sleeping = false;
                }
            }

            if (sleeping)
            {
                Thread.Sleep(500);
                CreateCircle();
            }

            // foreach (var b in World.BodyList)
            // {
            //     if (b.BodyType != BodyType.DynamicBody)
            //     {
            //         continue;
            //     }
            //
            //     var p = b.GetPosition();
            //     if (p.X <= -10.0f || 10.0f <= p.X || p.Y <= FP.Zero || 20.0f <= p.Y)
            //     {
            //         p.X += FP.EN1;
            //     }
            // }
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.C)
            {
                CreateCircle();
            }

            if (keyInput.Key == KeyCodes.A)
            {
                _autoCreate = !_autoCreate;
            }
        }

        protected override void OnRender()
        {
            DrawString("Press 'c' to create a circle.");
            DrawString("Press 'a' to toggle auto generate circles.");
        }
    }
}