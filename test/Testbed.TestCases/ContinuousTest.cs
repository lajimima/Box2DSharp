using TrueSync;
using FixedBox2D.Collision;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Continuous","Continuous Test")]
    public class ContinuousTest : TestBase
    {
        private FP _angularVelocity;

        private Body _body;

        private GJkProfile _gJkProfile = new GJkProfile();

        private ToiProfile _toiProfile = new ToiProfile();

        public ContinuousTest()
        {
            {
                World.ToiProfile = _toiProfile;
                World.GJkProfile = _gJkProfile;
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
                bd.Position.Set(FP.Zero, 20.0f);

                //bd.angle = FP.EN1;

                var shape = new PolygonShape();
                shape.SetAsBox(FP.Two, FP.EN1);

                _body = World.CreateBody(bd);
                _body.CreateFixture(shape, FP.One);

                _angularVelocity = RandomFloat(-50.0f, 50.0f);

                //m_angularVelocity = 46.661274f;
                _body.SetLinearVelocity(new TSVector2(FP.Zero, -100.0f));
                _body.SetAngularVelocity(_angularVelocity);
            }
        }

        private void Launch()
        {
            _gJkProfile.GjkCalls = 0;
            _gJkProfile.GjkIters = 0;
            _gJkProfile.GjkMaxIters = 0;

            _toiProfile.ToiCalls = 0;
            _toiProfile.ToiIters = 0;

            _toiProfile.ToiRootIters = 0;
            _toiProfile.ToiMaxRootIters = 0;

            _toiProfile.ToiTime = FP.Zero;
            _toiProfile.ToiMaxTime = FP.Zero;

            _body.SetTransform(new TSVector2(FP.Zero, 20.0f), FP.Zero);

            _angularVelocity = RandomFloat(-50.0f, 50.0f);

            _body.SetLinearVelocity(new TSVector2(FP.Zero, -100.0f));

            _body.SetAngularVelocity(_angularVelocity);
        }

        protected override void PreStep()
        {
            if (StepCount % 60 == 0)
            {
                Launch();
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            if (_gJkProfile.GjkCalls > 0)
            {
                DrawString(
                    $"gjk calls = {_gJkProfile.GjkCalls}, ave gjk iters = {_gJkProfile.GjkIters / (FP) _gJkProfile.GjkCalls}, max gjk iters = {_gJkProfile.GjkMaxIters}"
                );
            }

            if (_toiProfile.ToiCalls > 0)
            {
                DrawString(
                    $"toi calls = {_toiProfile.ToiCalls}, ave [max] toi iters = {_toiProfile.ToiIters / (FP) _toiProfile.ToiCalls} [{_toiProfile.ToiMaxRootIters}]");

                DrawString($"ave [max] toi root iters = {_toiProfile.ToiRootIters / (FP) _toiProfile.ToiCalls} [ToiMaxRootIters]");
                DrawString(
                    $"ave [max] toi time = {1000.0f * _toiProfile.ToiTime / (FP) _toiProfile.ToiCalls} [{1000.0f * _toiProfile.ToiMaxTime}] (microseconds)");
            }
        }
    }
}