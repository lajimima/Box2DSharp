using System;
using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Bugs", "Skier")]
    public class Skier : TestBase
    {
        public Body SkierBody;

        public FP PlatformWidth;

        public bool FixedCamera;

        public Skier()
        {
            Body ground = null;
            {
                BodyDef bd = new BodyDef();
                ground = World.CreateBody(bd);

                FP PlatformWidth = 8.0f;

                /*
                First angle is from the horizontal and should be negative for a downward slope.
                Second angle is relative to the preceding slope, and should be positive, creating a kind of
                loose 'Z'-shape from the 3 edges.
                If A1 = -10, then A2 <= ~1.5 will result in the collision glitch.
                If A1 = -30, then A2 <= ~10.0 will result in the glitch.
                */
                FP Angle1Degrees = -30.0f;
                FP Angle2Degrees = 10.0f;

                /*
                The larger the value of SlopeLength, the less likely the glitch will show up.
                */
                FP SlopeLength = FP.Two;

                FP SurfaceFriction = 0.2f;

                // Convert to radians
                FP Slope1Incline = -Angle1Degrees * Settings.Pi / 180.0f;
                FP Slope2Incline = Slope1Incline - Angle2Degrees * Settings.Pi / 180.0f;

                //

                this.PlatformWidth = PlatformWidth;

                // Horizontal platform
                var v1 = new TSVector2(-PlatformWidth, FP.Zero);
                var v2 = new TSVector2(FP.Zero, FP.Zero);
                var v3 = new TSVector2((FP)(SlopeLength * FP.FastCosAngle(Slope1Incline)), (FP)(-SlopeLength * FP.FastSinAngle(Slope1Incline)));
                var v4 = new TSVector2((FP)(v3.X + SlopeLength * FP.FastCosAngle(Slope2Incline)), (FP)(v3.Y - SlopeLength * FP.FastSinAngle(Slope2Incline)));
                var v5 = new TSVector2(v4.X, v4.Y - FP.One);

                var vertices = new[] {v5, v4, v3, v2, v1};

                ChainShape shape = new ChainShape();
                shape.CreateLoop(vertices, 5);
                FixtureDef fd = new FixtureDef();
                fd.Shape = shape;
                fd.Density = FP.Zero;
                fd.Friction = SurfaceFriction;

                ground.CreateFixture(fd);
            }

            {
                // const FP BodyWidth = FP.One;
                FP BodyHeight = 2.5f;
                FP SkiLength = 3.0f;

                /*
                Larger values for this seem to alleviate the issue to some extent.
                */
                FP SkiThickness = 0.3f;

                FP SkiFriction = FP.Zero;
                FP SkiRestitution = 0.15f;

                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;

                FP initial_y = BodyHeight / 2 + SkiThickness;
                bd.Position.Set(-PlatformWidth / 2, initial_y);

                var skier = World.CreateBody(bd);

                PolygonShape ski = new PolygonShape();
                var verts = new TSVector2[4];
                verts[0].Set(-SkiLength / 2 - SkiThickness, -BodyHeight / 2);
                verts[1].Set(-SkiLength / 2, -BodyHeight / 2 - SkiThickness);
                verts[2].Set(SkiLength / 2, -BodyHeight / 2 - SkiThickness);
                verts[3].Set(SkiLength / 2 + SkiThickness, -BodyHeight / 2);
                ski.Set(verts, 4);

                FixtureDef fd = new FixtureDef();
                fd.Density = FP.One;

                fd.Friction = SkiFriction;
                fd.Restitution = SkiRestitution;

                fd.Shape = ski;
                skier.CreateFixture(fd);

                skier.SetLinearVelocity(new TSVector2(0.5f, FP.Zero));

                SkierBody = skier;
            }

            Global.Camera.Center = new TSVector2(PlatformWidth / FP.Two, FP.Zero);
            Global.Camera.Zoom = 0.4f;
            FixedCamera = true;
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            switch (keyInput.Key)
            {
            case KeyCodes.C:
                FixedCamera = !FixedCamera;
                if (FixedCamera)
                {
                    Global.Camera.Center = new TSVector2(PlatformWidth / FP.Two, FP.Zero);
                }

                break;
            }
        }

        protected override void OnRender()
        {
            DrawString("Keys: c = Camera fixed/tracking");

            if (!FixedCamera)
            {
                Global.Camera.Center = SkierBody.GetPosition();
            }
        }
    }
}