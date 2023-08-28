using System;
using System.Diagnostics;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Benchmark", "Tiles")]
    public class Tiles : TestBase
    {
        public long CreateTime;

        public long FixtureCount;

        public int DynamicTreeHeight;

        public int MinHeight;

        private const int Count = 20;

        private const int MaxBodies = 256;

        private readonly Body[] _bodies = new Body[MaxBodies];

        private int _bodyIndex;

        public Tiles()
        {
            var fixtureCount = 0;
            var timer = Stopwatch.StartNew();
            {
                var a = 0.5f;
                var bd = new BodyDef();
                bd.Position.Y = -a;
                var ground = World.CreateBody(bd);

                var N = 200;
                var M = 10;
                var position = new TSVector2();
                position.Y = FP.Zero;
                for (var j = 0; j < M; ++j)
                {
                    position.X = -N * a;
                    for (var i = 0; i < N; ++i)
                    {
                        var shape = new PolygonShape();
                        shape.SetAsBox(a, a, position, FP.Zero);
                        ground.CreateFixture(shape, FP.Zero);
                        ++fixtureCount;
                        position.X += FP.Two * a;
                    }

                    position.Y -= FP.Two * a;
                }
            }

            {
                var a = 0.5f;
                var shape = new PolygonShape();
                shape.SetAsBox(a, a);

                var x = new TSVector2(-7.0f, 0.75f);
                var y = new TSVector2();
                var deltaX = new TSVector2(0.5625f, 1.25f);
                var deltaY = new TSVector2(1.125f, FP.Zero);

                for (int i = 0; i < Count; ++i)
                {
                    y = x;

                    for (var j = i; j < Count; ++j)
                    {
                        var bd = new BodyDef {BodyType = BodyType.DynamicBody, Position = y};
                        var body = World.CreateBody(bd);
                        if (_bodies[_bodyIndex] != null)
                        {
                            World.DestroyBody(_bodies[_bodyIndex]);
                            _bodies[_bodyIndex] = null;
                        }

                        _bodies[_bodyIndex] = body;
                        _bodyIndex = (_bodyIndex + 1) % MaxBodies;
                        body.CreateFixture(shape, 5.0f);
                        ++fixtureCount;
                        y += deltaY;
                    }

                    x += deltaX;
                }
            }

            CreateTime = timer.ElapsedMilliseconds;
            FixtureCount = fixtureCount;
        }

        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            if (keyInput.Key == KeyCodes.A)
            {
                var j = 0;
                for (var z = 0; z < MaxBodies; z += 2)
                {
                    if (_bodies[z] != null)
                    {
                        ++j;

                        var isEnabled = _bodies[z].IsEnabled;
                        _bodies[z].IsEnabled = !isEnabled;
                    }
                }
            }
        }

        protected override void OnRender()
        {
            //var cm = World.ContactManager;
            //var height = cm.BroadPhase.GetTreeHeight();
            //var leafCount = cm.BroadPhase.GetProxyCount();
            //var minimumNodeCount = 2 * leafCount - 1;
            //var minimumHeight = (int)FP.Ceiling(Math.Log(minimumNodeCount) / Math.Log(FP.Two));
            //DynamicTreeHeight = height;
            //MinHeight = minimumHeight;
            //DrawString($"create time = {CreateTime} ms, fixture count = {FixtureCount}");
            //DrawString($"dynamic tree height = {DynamicTreeHeight}, min = {MinHeight}");
        }
    }
}