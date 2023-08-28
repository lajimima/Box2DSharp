using TrueSync;
using FixedBox2D.Collision;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Continuous", "Bullet Test")]
    public class BulletTest : TestBase
    {
        private Body _body;

        private Body _bullet;

        private FP _x;

        private GJkProfile _gJkProfile = new GJkProfile();

        private ToiProfile _toiProfile = new ToiProfile();

        public BulletTest()
        {
            {
                var bd = new BodyDef();
                bd.Position.Set(FP.Zero, FP.Zero);
                var body = World.CreateBody(bd);

                var edge = new EdgeShape();

                edge.SetTwoSided(new TSVector2(-10.0f, FP.Zero), new TSVector2(10.0f, FP.Zero));
                body.CreateFixture(edge, FP.Zero);

                var shape = new PolygonShape();
                shape.SetAsBox(0.2f, FP.One, new TSVector2(0.5f, FP.One), FP.Zero);
                body.CreateFixture(shape, FP.Zero);
            }

            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.Position.Set(FP.Zero, 4.0f);

                var box = new PolygonShape();
                box.SetAsBox(FP.Two, FP.EN1);

                _body = World.CreateBody(bd);
                _body.CreateFixture(box, FP.One);

                box.SetAsBox(0.25f, 0.25f);

                //m_x = RandomFloat(-FP.One, FP.One);
                _x = 0.20352793f;
                bd.Position = new TSVector2(_x, 10.0f);
                bd.Bullet = true;

                _bullet = World.CreateBody(bd);
                _bullet.CreateFixture(box, 100.0f);

                _bullet.SetLinearVelocity(new TSVector2(FP.Zero, -50.0f));
            }
        }

        private void Launch()
        {
            _body.SetTransform(new TSVector2(FP.Zero, 4.0f), FP.Zero);
            _body.SetLinearVelocity(TSVector2.Zero);
            _body.SetAngularVelocity(FP.Zero);

            _x = RandomFloat(-FP.One, FP.One);
            _bullet.SetTransform(new TSVector2(_x, 10.0f), FP.Zero);
            _bullet.SetLinearVelocity(new TSVector2(FP.Zero, -50.0f));
            _bullet.SetAngularVelocity(FP.Zero);
        }

        protected override void PreStep()
        {
            if (StepCount % 60 == 0)
            {
                Launch();
            }
        }

        protected override void OnRender()
        {
            if (_gJkProfile.GjkCalls > 0)
            {
                DrawString(
                    $"gjk calls = {_gJkProfile.GjkCalls}, ave gjk iters = {_gJkProfile.GjkIters / (FP)_gJkProfile.GjkCalls}, max gjk iters = {_gJkProfile.GjkMaxIters}");
            }

            if (_toiProfile.ToiCalls > 0)
            {
                DrawString(
                    $"toi calls = {_toiProfile.ToiCalls}, ave toi iters = {_toiProfile.ToiIters / (FP)_toiProfile.ToiCalls}, max toi iters = {_toiProfile.ToiMaxRootIters}");
                DrawString(
                    $"ave toi root iters = {_toiProfile.ToiRootIters / (FP)_toiProfile.ToiCalls}, max toi root iters = {_toiProfile.ToiMaxRootIters}");
            }
        }
    }
}