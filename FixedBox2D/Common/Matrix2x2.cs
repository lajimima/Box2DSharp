using System.Runtime.CompilerServices;
using TrueSync;

namespace FixedBox2D.Common
{
    public struct Matrix2x2
    {
        public TSVector2 Ex;

        public TSVector2 Ey;

        /// The default constructor does nothing (for performance).
        /// Construct this matrix using columns.
        public Matrix2x2(in TSVector2 c1, in TSVector2 c2)
        {
            Ex = c1;
            Ey = c2;
        }

        /// Construct this matrix using scalars.
        public Matrix2x2(FP a11, FP a12, FP a21, FP a22)
        {
            Ex.X = a11;
            Ex.Y = a21;
            Ey.X = a12;
            Ey.Y = a22;
        }

        /// Initialize this matrix using columns.
        public void Set(in TSVector2 c1, in TSVector2 c2)
        {
            Ex = c1;
            Ey = c2;
        }

        /// Set this to the identity matrix.
        public void SetIdentity()
        {
            Ex.X = FP.One;
            Ey.X = FP.Zero;
            Ex.Y = FP.Zero;
            Ey.Y = FP.One;
        }

        /// Set this matrix to all zeros.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetZero()
        {
            Ex.X = FP.Zero;
            Ey.X = FP.Zero;
            Ex.Y = FP.Zero;
            Ey.Y = FP.Zero;
        }

        public Matrix2x2 GetInverse()
        {
            var a = Ex.X;
            var b = Ey.X;
            var c = Ex.Y;
            var d = Ey.Y;

            var det = a * d - b * c;
            if (det != FP.Zero)
            {
                det = FP.One / det;
            }

            var B = new Matrix2x2();
            B.Ex.X = det * d;
            B.Ey.X = -det * b;
            B.Ex.Y = -det * c;
            B.Ey.Y = det * a;
            return B;
        }

        /// Solve A * x = b, where b is a column vector. This is more efficient
        /// than computing the inverse in one-shot cases.
        public TSVector2 Solve(in TSVector2 b)
        {
            var a11 = Ex.X;
            var a12 = Ey.X;
            var a21 = Ex.Y;
            var a22 = Ey.Y;
            var det = a11 * a22 - a12 * a21;
            if (det != FP.Zero)
            {
                det = FP.One / det;
            }

            var x = new TSVector2 {X = det * (a22 * b.X - a12 * b.Y), Y = det * (a11 * b.Y - a21 * b.X)};
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2x2 operator +(in Matrix2x2 A, in Matrix2x2 B)
        {
            return new Matrix2x2(A.Ex + B.Ex, A.Ey + B.Ey);
        }
    }
}