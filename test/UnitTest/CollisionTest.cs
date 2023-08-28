using System;
using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using Shouldly;
using Xunit;

namespace UnitTest
{
    public class CollisionTest
    {
        [Fact(DisplayName = "polygon mass data")]
        public void PolygonMassData()
        {
            var center = new TSVector2(100.0f, -50.0f);
            const FP hx = 0.5f, hy = 1.5f;
            const FP angle1 = 0.25f;

            // Data from issue #422. Not used because the data exceeds accuracy limits.
            //const b2Vec2 center(-15000.0f, -15000.0f);
            //const FP hx = 0.72f, hy = 0.72f;
            //const FP angle1 = 0.0f;

            PolygonShape polygon1 = new PolygonShape();
            polygon1.SetAsBox(hx, hy, center, angle1);

            const FP absTol = FP.Two * Settings.Epsilon;
            const FP relTol = FP.Two * Settings.Epsilon;

            Math.Abs(polygon1.Centroid.X - center.X).ShouldBeLessThan(absTol + relTol * Math.Abs(center.X));
            Math.Abs(polygon1.Centroid.Y - center.Y).ShouldBeLessThan(absTol + relTol * Math.Abs(center.Y));

            var vertices = new TSVector2[4];
            vertices[0].Set(center.X - hx, center.Y - hy);
            vertices[1].Set(center.X + hx, center.Y - hy);
            vertices[2].Set(center.X - hx, center.Y + hy);
            vertices[3].Set(center.X + hx, center.Y + hy);

            PolygonShape polygon2 = new PolygonShape();
            polygon2.Set(vertices, 4);

            Math.Abs(polygon2.Centroid.X - center.X).ShouldBeLessThan(absTol + relTol * Math.Abs(center.X));
            Math.Abs(polygon2.Centroid.Y - center.Y).ShouldBeLessThan(absTol + relTol * Math.Abs(center.Y));

            const FP mass = 4.0f * hx * hy;
            FP inertia = (mass / 3.0f) * (hx * hx + hy * hy) + mass * TSVector2.Dot(center, center);

            polygon1.ComputeMass(out var massData1, 1.0f);

            Math.Abs(massData1.Center.X - center.X).ShouldBeLessThan(absTol + relTol * Math.Abs(center.X));
            Math.Abs(massData1.Center.Y - center.Y).ShouldBeLessThan(absTol + relTol * Math.Abs(center.Y));
            Math.Abs(massData1.Mass - mass).ShouldBeLessThan(20.0f * (absTol + relTol * mass));
            Math.Abs(massData1.RotationInertia - inertia).ShouldBeLessThan(40.0f * (absTol + relTol * inertia));

            polygon2.ComputeMass(out var massData2, 1.0f);

            Math.Abs(massData2.Center.X - center.X).ShouldBeLessThan(absTol + relTol * Math.Abs(center.X));
            Math.Abs(massData2.Center.Y - center.Y).ShouldBeLessThan(absTol + relTol * Math.Abs(center.Y));
            Math.Abs(massData2.Mass - mass).ShouldBeLessThan(20.0f * (absTol + relTol * mass));
            Math.Abs(massData2.RotationInertia - inertia).ShouldBeLessThan(40.0f * (absTol + relTol * inertia));
        }
    }
}