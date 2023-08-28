using System;
using System.Runtime.CompilerServices;
using TrueSync;

namespace FixedBox2D.Common
{
    /// Rotation
    public struct Rotation
    {
        /// Sine and cosine
        public FP Sin;

        public FP Cos;

        public Rotation(FP sin, FP cos)
        {
            Sin = sin;
            Cos = cos;
        }

        /// Initialize from an angle in radians
        public Rotation(FP angle)
        {
            // TODO_ERIN optimize
            Sin = FP.FastSinAngle(angle);
            Cos = FP.FastCosAngle(angle);
        }

        /// Set using an angle in radians.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(FP angle)
        {
            // TODO_ERIN optimize
            Sin = FP.FastSinAngle(angle);
            Cos = FP.FastCosAngle(angle);
        }

        /// Set to the identity rotation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIdentity()
        {
            Sin = FP.Zero;
            Cos = FP.One;
        }

        /// Get the x-axis
        public TSVector2 GetXAxis()
        {
            return new TSVector2(Cos, Sin);
        }

        /// Get the u-axis
        public TSVector2 GetYAxis()
        {
            return new TSVector2(-Sin, Cos);
        }
    }
}