using System.Runtime.CompilerServices;
using TrueSync;

namespace FixedBox2D.Common
{
    public static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(in this TSVector2 vector2)
        {
            return !FP.IsInfinity(vector2.X) && !FP.IsInfinity(vector2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this FP x)
        {
            return !FP.IsInfinity(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZero(ref this TSVector2 vector2)
        {
            vector2.X = FP.Zero;
            vector2.Y = FP.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref this TSVector2 vector2, FP x, FP y)
        {
            vector2.X = x;
            vector2.Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetZero(ref this TSVector vector3)
        {
            vector3.X = FP.Zero;
            vector3.Y = FP.Zero;
            vector3.Z = FP.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref this TSVector vector3, FP x, FP y, FP z)
        {
            vector3.X = x;
            vector3.Y = y;
            vector3.Z = z;
        }

        /// Convert this vector into a unit vector. Returns the length.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Normalize(TSVector2 vector2)
        {
            var length = vector2.magnitude;
            if (length < Settings.Epsilon)
            {
                return FP.Zero;
            }

            var invLength = FP.One / length;
            vector2.X *= invLength;
            vector2.Y *= invLength;

            return length;
        }

        /// <summary>
        ///  Get the skew vector such that dot(skew_vec, other) == cross(vec, other)
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSVector2 Skew(ref this TSVector2 vector2)
        {
            return new TSVector2(-vector2.Y, vector2.X);
        }
    }
}