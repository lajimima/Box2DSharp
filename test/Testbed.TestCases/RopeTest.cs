using TrueSync;
using FixedBox2D.Common;
using FixedBox2D.Ropes;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Rope", "Bending")]
    public class RopeTest : TestBase
    {
        protected readonly Rope Rope1;

        protected readonly Rope Rope2;

        protected readonly RopeTuning Tuning1;

        protected readonly RopeTuning Tuning2;

        protected int Iterations1;

        protected int Iterations2;

        protected TSVector2 Position1;

        protected TSVector2 Position2;

        protected FP Speed;

        static FP L = FP.Half;
        public RopeTest()
        {
            const int N = 20;
            var vertices = new TSVector2[N];
            var masses = new FP[N];

            for (var i = 0; i < N; ++i)
            {
                vertices[i].Set(FP.Zero, L * (N - i));
                masses[i] = FP.One;
            }

            masses[0] = FP.Zero;
            masses[1] = FP.Zero;
            Tuning1 = new RopeTuning
            {
                BendHertz = 30.0f,
                BendDamping = 4.0f,
                BendStiffness = FP.One,
                BendingModel = BendingModel.PbdTriangleBendingModel,
                Isometric = true,
                StretchHertz = 30.0f,
                StretchDamping = 4.0f,
                StretchStiffness = FP.One,
                StretchingModel = StretchingModel.PbdStretchingModel
            };

            Tuning2 = new RopeTuning
            {
                BendHertz = 30.0f,
                BendDamping = 0.7f,
                BendStiffness = FP.One,
                BendingModel = BendingModel.PbdHeightBendingModel,
                Isometric = true,
                StretchHertz = 30.0f,
                StretchDamping = FP.One,
                StretchStiffness = FP.One,
                StretchingModel = StretchingModel.PbdStretchingModel
            };

            Position1.Set(-5.0f, 15.0f);
            Position2.Set(5.0f, 15.0f);

            var def = new RopeDef
            {
                Vertices = vertices,
                Count = N,
                Gravity = new TSVector2(FP.Zero, -10.0f),
                Masses = masses,
                Position = Position1,
                Tuning = Tuning1
            };
            Rope1 = new Rope();
            Rope1.Create(def);

            def.Position = Position2;
            def.Tuning = Tuning2;
            Rope2 = new Rope();
            Rope2.Create(def);

            Iterations1 = 8;
            Iterations2 = 8;

            Speed = 10.0f;
        }

        protected override void PreStep()
        {
            var dt = TestSettings.Hertz > FP.Zero ? FP.One / TestSettings.Hertz : FP.Zero;
            if (Input.IsKeyDown(KeyCodes.Comma))
            {
                Position1.X -= Speed * dt;
                Position2.X -= Speed * dt;
            }

            if (Input.IsKeyDown(KeyCodes.Period))
            {
                Position1.X += Speed * dt;
                Position2.X += Speed * dt;
            }

            if (TestSettings.Pause && !TestSettings.SingleStep)
            {
                dt = FP.Zero;
            }

            Rope1.SetTuning(Tuning1);
            Rope2.SetTuning(Tuning2);
            Rope1.Step(dt, Iterations1, Position1);
            Rope2.Step(dt, Iterations2, Position2);
        }
    }
}