using System;
using FixedBox2D.Collision;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;
using Color = FixedBox2D.Common.Color;
using Transform = FixedBox2D.Common.Transform;
using TrueSync;

namespace Testbed.TestCases
{
    internal class PolyShapesCallback : IQueryCallback
    {
        private const int MaxCount = 4;

        private readonly IDrawer _drawer;

        public CircleShape Circle = new CircleShape();

        public int Count;

        public Transform Transform = new Transform();

        public PolyShapesCallback(IDrawer drawer)
        {
            _drawer = drawer;
            Count = 0;
        }

        /// Called for each fixture found in the query AABB.
        /// @return false to terminate the query.
        public bool QueryCallback(Fixture fixture)
        {
            if (Count == MaxCount)
            {
                return false;
            }

            var body = fixture.Body;
            var shape = fixture.Shape;

            var overlap = CollisionUtils.TestOverlap(
                shape,
                0,
                Circle,
                0,
                body.GetTransform(),
                Transform,
                fixture.Body.World.GJkProfile);

            if (overlap)
            {
                var color = FixedBox2D.Common.Color.FromArgb(0.95f, 0.95f, 0.6f);
                var center = body.GetWorldCenter();
                _drawer.DrawPoint(center, 5.0f, color);
                ++Count;
            }

            return true;
        }
    }

    [TestCase("Geometry", "Polygon Shapes")]
    public class PolygonShapes : TestBase
    {
        private const int MaxBodies = 256;

        private readonly Body[] _bodies = new Body[MaxBodies];

        private readonly PolygonShape[] _polygons = new PolygonShape[4]
        {
            new PolygonShape(),
            new PolygonShape(),
            new PolygonShape(),
            new PolygonShape()
        };

        private int _bodyIndex;

        private readonly CircleShape _circle = new CircleShape();

        public PolygonShapes()
        {
            // Ground body
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                var vertices = new TSVector2[3];
                vertices[0].Set(-0.5f, FP.Zero);
                vertices[1].Set(0.5f, FP.Zero);
                vertices[2].Set(FP.Zero, 1.5f);
                _polygons[0].Set(vertices);
            }

            {
                var vertices = new TSVector2[3];
                vertices[0].Set(-FP.EN1, FP.Zero);
                vertices[1].Set(FP.EN1, FP.Zero);
                vertices[2].Set(FP.Zero, 1.5f);
                _polygons[1].Set(vertices);
            }

            {
                var w = FP.One;
                var b = w / (FP.Two + (FP)FP.Sqrt(FP.Two));
                var s = (FP)FP.Sqrt(FP.Two) * b;

                var vertices = new TSVector2[8];
                vertices[0].Set(0.5f * s, FP.Zero);
                vertices[1].Set(0.5f * w, b);
                vertices[2].Set(0.5f * w, b + s);
                vertices[3].Set(0.5f * s, w);
                vertices[4].Set(-0.5f * s, w);
                vertices[5].Set(-0.5f * w, b + s);
                vertices[6].Set(-0.5f * w, b);
                vertices[7].Set(-0.5f * s, FP.Zero);

                _polygons[2].Set(vertices);
            }

            {
                _polygons[3].SetAsBox(0.5f, 0.5f);
            }

            {
                _circle.Radius = 0.5f;
            }

            _bodyIndex = 0;
        }

        private void Create(int index)
        {
            if (_bodies[_bodyIndex] != null)
            {
                World.DestroyBody(_bodies[_bodyIndex]);
                _bodies[_bodyIndex] = null;
            }

            var bd = new BodyDef();
            bd.BodyType = BodyType.DynamicBody;

            var x = RandomFloat(-FP.Two, FP.Two);
            bd.Position.Set(x, 10.0f);
            bd.Angle = RandomFloat(-Settings.Pi, Settings.Pi);

            if (index == 4)
            {
                bd.AngularDamping = 0.02f;
            }

            _bodies[_bodyIndex] = World.CreateBody(bd);

            if (index < 4)
            {
                var fd = new FixtureDef();
                fd.Shape = _polygons[index];
                fd.Density = FP.One;
                fd.Friction = 0.3f;
                _bodies[_bodyIndex].CreateFixture(fd);
            }
            else
            {
                var fd = new FixtureDef();
                fd.Shape = _circle;
                fd.Density = FP.One;
                fd.Friction = 0.3f;

                _bodies[_bodyIndex].CreateFixture(fd);
            }

            _bodyIndex = (_bodyIndex + 1) % MaxBodies;
        }

        private void DestroyBody()
        {
            for (var i = 0; i < MaxBodies; ++i)
            {
                if (_bodies[i] != null)
                {
                    World.DestroyBody(_bodies[i]);
                    _bodies[i] = null;
                    return;
                }
            }
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            var k = -1;
            if (keyInput.Key == KeyCodes.D1)
            {
                k = 0;
            }

            if (keyInput.Key == KeyCodes.D2)
            {
                k = 1;
            }

            if (keyInput.Key == KeyCodes.D3)
            {
                k = 2;
            }

            if (keyInput.Key == KeyCodes.D4)
            {
                k = 3;
            }

            if (keyInput.Key == KeyCodes.D5)
            {
                k = 4;
            }

            if (k > -1)
            {
                Create(k);
            }

            if (keyInput.Key == KeyCodes.A)
            {
                for (var i = 0; i < MaxBodies; i += 2)
                {
                    if (_bodies[i] != null)
                    {
                        var isEnabled = _bodies[i].IsEnabled;
                        _bodies[i].IsEnabled = !isEnabled;
                    }
                }
            }

            if (keyInput.Key == KeyCodes.D)
            {
                DestroyBody();
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            DrawString("Press 1-5 to drop stuff");
            DrawString("Press 'a' to (de)activate some bodies");
            DrawString("Press 'd' to destroy a body");

            var callback = new PolyShapesCallback(Drawer) {Circle = {Radius = FP.Two}};
            callback.Circle.Position.Set(FP.Zero, 1.1f);
            callback.Circle.ComputeAABB(out var aabb, callback.Transform, 0);
            callback.Transform.SetIdentity();

            World.QueryAABB(callback, aabb);

            var color = Color.FromArgb(102, 178, 204);
            Drawer.DrawCircle(callback.Circle.Position, callback.Circle.Radius, color);
        }
    }
}