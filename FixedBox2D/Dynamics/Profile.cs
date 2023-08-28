using TrueSync;

namespace FixedBox2D.Dynamics
{
    /// Profiling data. Times are in milliseconds.
    public struct Profile
    {
        public FP Step;

        public FP Collide;

        public FP Solve;

        public FP SolveInit;

        public FP SolveVelocity;

        public FP SolvePosition;

        public FP Broadphase;

        public FP SolveTOI;
    }
}