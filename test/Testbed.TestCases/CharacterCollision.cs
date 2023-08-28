using System;
using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Character Collision")]
    public class CharacterCollision : TestBase
    {
        private Body _character;

        public CharacterCollision()
        {
            // Ground body
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            // Collinear edges with no adjacency information.
            // This shows the problematic case where a box shape can hit
            // an internal vertex.
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-8.0f, FP.One), new TSVector2(-6.0f, FP.One));
                ground.CreateFixture(shape, FP.Zero);
                shape.SetTwoSided(new TSVector2(-6.0f, FP.One), new TSVector2(-4.0f, FP.One));
                ground.CreateFixture(shape, FP.Zero);
                shape.SetTwoSided(new TSVector2(-4.0f, FP.One), new TSVector2(-FP.Two, FP.One));
                ground.CreateFixture(shape, FP.Zero);
            }

            // Chain shape
            {
                var bd = new BodyDef {Angle = 0.25f * Settings.Pi};
                var ground = World.CreateBody(bd);

                var vs = new TSVector2[4]
                {
                    new TSVector2(5.0f, 7.0f),
                    new TSVector2(6.0f, 8.0f),
                    new TSVector2(7.0f, 8.0f),
                    new TSVector2(8.0f, 7.0f)
                };
                var shape = new ChainShape();
                shape.CreateLoop(vs);
                ground.CreateFixture(shape, FP.Zero);
            }

            // Square tiles. This shows that adjacency shapes may
            // have non-smooth collision. There is no solution
            // to this problem.
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, FP.One, new TSVector2(4.0f, 3.0f), FP.Zero);
                ground.CreateFixture(shape, FP.Zero);
                shape.SetAsBox(FP.One, FP.One, new TSVector2(6.0f, 3.0f), FP.Zero);
                ground.CreateFixture(shape, FP.Zero);
                shape.SetAsBox(FP.One, FP.One, new TSVector2(8.0f, 3.0f), FP.Zero);
                ground.CreateFixture(shape, FP.Zero);
            }

            // Square made from an edge loop. Collision should be smooth.
            {
                var bd = new BodyDef();
                var ground = World.CreateBody(bd);

                var vs = new TSVector2[4]
                {
                    new TSVector2(-FP.One, 3.0f),
                    new TSVector2(FP.One, 3.0f),
                    new TSVector2(FP.One, 5.0f),
                    new TSVector2(-FP.One, 5.0f)
                };

                var shape = new ChainShape();
                shape.CreateLoop(vs);
                ground.CreateFixture(shape, FP.Zero);
            }

            // Edge loop. Collision should be smooth.
            {
                var bd = new BodyDef {Position = new TSVector2(-10.0f, 4.0f)};
                var ground = World.CreateBody(bd);

                var vs = new TSVector2[10]
                {
                    new TSVector2(FP.Zero, FP.Zero),
                    new TSVector2(6.0f, FP.Zero),
                    new TSVector2(6.0f, FP.Two),
                    new TSVector2(4.0f, FP.One),
                    new TSVector2(FP.Two, FP.Two),
                    new TSVector2(FP.Zero, FP.Two),
                    new TSVector2(-FP.Two, FP.Two),
                    new TSVector2(-4.0f, 3.0f),
                    new TSVector2(-6.0f, FP.Two),
                    new TSVector2(-6.0f, FP.Zero)
                };
                var shape = new ChainShape();
                shape.CreateLoop(vs);
                ground.CreateFixture(shape, FP.Zero);
            }

            // Square character 1
            {
                var bd = new BodyDef
                {
                    Position = new TSVector2(-3.0f, 8.0f),
                    BodyType = BodyType.DynamicBody,
                    FixedRotation = true,
                    AllowSleep = false
                };

                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);

                var fd = new FixtureDef {Shape = shape, Density = 20.0f};
                body.CreateFixture(fd);
            }

            // Square character 2
            {
                var bd = new BodyDef
                {
                    Position = new TSVector2(-5.0f, 5.0f),
                    BodyType = BodyType.DynamicBody,
                    FixedRotation = true,
                    AllowSleep = false
                };

                var body = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(0.25f, 0.25f);

                var fd = new FixtureDef {Shape = shape, Density = 20.0f};
                body.CreateFixture(fd);
            }

            // Hexagon character
            {
                var bd = new BodyDef
                {
                    Position = new TSVector2(-5.0f, 8.0f),
                    BodyType = BodyType.DynamicBody,
                    FixedRotation = true,
                    AllowSleep = false
                };

                var body = World.CreateBody(bd);

                var angle = FP.Zero;
                FP delta = Settings.Pi / 3.0f;
                var vertices = new TSVector2[6];
                for (var i = 0; i < 6; ++i)
                {
                    vertices[i].Set(0.5f * FP.FastCosAngle(angle), 0.5f * (FP)FP.FastSinAngle(angle));
                    angle += delta;
                }

                var shape = new PolygonShape();
                shape.Set(vertices);

                var fd = new FixtureDef {Shape = shape, Density = 20.0f};
                body.CreateFixture(fd);
            }

            // Circle character
            {
                var bd = new BodyDef
                {
                    Position = new TSVector2(3.0f, 5.0f),
                    BodyType = BodyType.DynamicBody,
                    FixedRotation = true,
                    AllowSleep = false
                };

                var body = World.CreateBody(bd);

                var shape = new CircleShape {Radius = 0.5f};

                var fd = new FixtureDef {Shape = shape, Density = 20.0f};
                body.CreateFixture(fd);
            }

            // Circle character
            {
                var bd = new BodyDef
                {
                    Position = new TSVector2(-7.0f, 6.0f),
                    BodyType = BodyType.DynamicBody,
                    AllowSleep = false
                };

                _character = World.CreateBody(bd);

                var shape = new CircleShape {Radius = 0.25f};

                var fd = new FixtureDef
                {
                    Shape = shape,
                    Density = 20.0f,
                    Friction = FP.One
                };
                _character.CreateFixture(fd);
            }
        }

        /// <inheritdoc />
        protected override void PreStep()
        {
            var v = _character.LinearVelocity;
            v.X = -5.0f;
            _character.SetLinearVelocity(v);
        }

        protected override void OnRender()
        {
            DrawString("This tests various character collision shapes.");
            DrawString("Limitation: square and hexagon can snag on aligned boxes.");
            DrawString("Feature: edge chains have smooth collision inside and out.");
        }
    }
}