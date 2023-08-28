using System;
using System.Collections.Generic;
using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Collision Processing")]
    public class CollisionProcessing : TestBase
    {
        public CollisionProcessing()
        {
            // Ground body
            {
                var shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-50.0f, FP.Zero), new TSVector2(50.0f, FP.Zero));

                var sd = new FixtureDef();
                sd.Shape = shape;
                ;

                var bd = new BodyDef();
                var ground = World.CreateBody(bd);
                ground.CreateFixture(sd);
            }

            FP xLo = -5.0f, xHi = 5.0f;
            FP yLo = FP.Two, yHi = 35.0f;

            // Small triangle
            var vertices = new TSVector2[3];
            vertices[0].Set(-FP.One, FP.Zero);
            vertices[1].Set(FP.One, FP.Zero);
            vertices[2].Set(FP.Zero, FP.Two);

            var polygon = new PolygonShape();
            polygon.Set(vertices);

            var triangleShapeDef = new FixtureDef();
            triangleShapeDef.Shape = polygon;
            triangleShapeDef.Density = FP.One;

            var triangleBodyDef = new BodyDef();
            triangleBodyDef.BodyType = BodyType.DynamicBody;
            triangleBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body1 = World.CreateBody(triangleBodyDef);
            body1.CreateFixture(triangleShapeDef);

            // Large triangle (recycle definitions)
            vertices[0] *= FP.Two;
            vertices[1] *= FP.Two;
            vertices[2] *= FP.Two;
            polygon.Set(vertices);

            triangleBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body2 = World.CreateBody(triangleBodyDef);
            body2.CreateFixture(triangleShapeDef);

            // Small box
            polygon.SetAsBox(FP.One, 0.5f);

            var boxShapeDef = new FixtureDef();
            boxShapeDef.Shape = polygon;
            boxShapeDef.Density = FP.One;

            var boxBodyDef = new BodyDef();
            boxBodyDef.BodyType = BodyType.DynamicBody;
            boxBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body3 = World.CreateBody(boxBodyDef);
            body3.CreateFixture(boxShapeDef);

            // Large box (recycle definitions)
            polygon.SetAsBox(FP.Two, FP.One);
            boxBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body4 = World.CreateBody(boxBodyDef);
            body4.CreateFixture(boxShapeDef);

            // Small circle
            var circle = new CircleShape();
            circle.Radius = FP.One;

            var circleShapeDef = new FixtureDef();
            circleShapeDef.Shape = circle;
            circleShapeDef.Density = FP.One;

            var circleBodyDef = new BodyDef();
            circleBodyDef.BodyType = BodyType.DynamicBody;
            circleBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body5 = World.CreateBody(circleBodyDef);
            body5.CreateFixture(circleShapeDef);

            // Large circle
            circle.Radius *= FP.Two;
            circleBodyDef.Position.Set(RandomFloat(xLo, xHi), RandomFloat(yLo, yHi));

            var body6 = World.CreateBody(circleBodyDef);
            body6.CreateFixture(circleShapeDef);
        }

        /// <inheritdoc />
        protected override void PostStep()
        {
            // We are going to destroy some bodies according to contact
            // points. We must buffer the bodies that should be destroyed
            // because they may belong to multiple contact points.
            const int maxNuke = 6;
            var nuke = new Body[maxNuke];
            var nukeCount = 0;

            // Traverse the contact results. Destroy bodies that
            // are touching heavier bodies.
            for (var i = 0; i < PointsCount; ++i)
            {
                var point = Points[i];

                var body1 = point.FixtureA.Body;
                var body2 = point.FixtureB.Body;
                var mass1 = body1.Mass;
                var mass2 = body2.Mass;

                if (mass1 > FP.Zero && mass2 > FP.Zero)
                {
                    if (mass2 > mass1)
                    {
                        nuke[nukeCount++] = body1;
                    }
                    else
                    {
                        nuke[nukeCount++] = body2;
                    }

                    if (nukeCount == maxNuke)
                    {
                        break;
                    }
                }
            }

            // Sort the nuke array to group duplicates.
            Array.Sort(nuke, 0, nukeCount, new BodyComparer());

            // Destroy the bodies, skipping duplicates.
            {
                var i = 0;
                while (i < nukeCount)
                {
                    var b = nuke[i++];
                    while (i < nukeCount && nuke[i] == b)
                    {
                        ++i;
                    }

                    if (b != Bomb)
                    {
                        World.DestroyBody(b);
                    }
                }
            }
        }
    }

    struct BodyComparer : IComparer<Body>
    {
        /// <inheritdoc />
        public int Compare(Body x, Body y)
        {
            return x.GetHashCode() - y.GetHashCode();
        }
    }
}