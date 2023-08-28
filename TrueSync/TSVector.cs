/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
*
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution.
*  3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Runtime.CompilerServices;
#if USEBATTLEDLL
#else
namespace TrueSync
{
    /// <summary>
    /// A vector structure.
    /// </summary>
    [Serializable]
    public struct TSVector
    {

        private static FP ZeroEpsilonSq = FP.Epsilon;
        internal static TSVector InternalZero;
        internal static TSVector Arbitrary;

        /// <summary>The X component of the vector.</summary>
        public FP X;
        /// <summary>The Y component of the vector.</summary>
        public FP Y;
        /// <summary>The Z component of the vector.</summary>
        public FP Z;

        #region Static readonly variables
        public static readonly TSVector initPos;
        /// <summary>
        /// A vector with components (0,0,0);
        /// </summary>
        public static readonly TSVector zero;
        /// <summary>
        /// A vector with components (-1,0,0);
        /// </summary>
        public static readonly TSVector left;
        /// <summary>
        /// A vector with components (1,0,0);
        /// </summary>
        public static readonly TSVector right;
        /// <summary>
        /// A vector with components (0,1,0);
        /// </summary>
        public static readonly TSVector up;
        /// <summary>
        /// A vector with components (0,-1,0);
        /// </summary>
        public static readonly TSVector down;
        /// <summary>
        /// A vector with components (0,0,-1);
        /// </summary>
        public static readonly TSVector back;
        /// <summary>
        /// A vector with components (0,0,1);
        /// </summary>
        public static readonly TSVector forward;
        /// <summary>
        /// A vector with components (1,1,1);
        /// </summary>
        public static readonly TSVector one;
        /// <summary>
        /// A vector with components
        /// (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly TSVector MinValue;
        /// <summary>
        /// A vector with components
        /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly TSVector MaxValue;
        #endregion

        #region Private static constructor
        static TSVector()
        {
            initPos = new TSVector(-1, -1, -1) * 1000;
            one = new TSVector(1, 1, 1);
            zero = new TSVector(0, 0, 0);
            left = new TSVector(-1, 0, 0);
            right = new TSVector(1, 0, 0);
            up = new TSVector(0, 1, 0);
            down = new TSVector(0, -1, 0);
            back = new TSVector(0, 0, -1);
            forward = new TSVector(0, 0, 1);
            MinValue = new TSVector(FP.MinValue);
            MaxValue = new TSVector(FP.MaxValue);
            Arbitrary = new TSVector(1, 1, 1);
            InternalZero = zero;
        }
        #endregion

        public static TSVector Abs(TSVector other)
        {
            return Abs(ref other);
        }
        public static TSVector Abs(ref TSVector other)
        {
            return new TSVector(FP.Abs(other.X), FP.Abs(other.Y), FP.Abs(other.Z));
        }

        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public FP sqrMagnitude
        {
            get
            {
                return (((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z));
            }
        }
        public TSVector Normalized()
        {
            return normalized;
        }
        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        /// <returns>Returns the length of the vector.</returns>
        public FP magnitude
        {
            get
            {
                return FP.Sqrt(((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z));
            }
        }

        public FP Length()
        {
            return magnitude;
        }

        //>
        public bool IsLengthMoreThan(FP value)
        {
            return sqrMagnitude > value * value;

            if (value < FP.Zero)
            {
                return true;
            }

            return FP.FastAbs(X) > value || FP.FastAbs(Y) > value || FP.FastAbs(Z) > value || (sqrMagnitude > value * value);
        }

        //<
        public bool IsLengthLessThan(FP value)
        {
            return sqrMagnitude < value * value;

            if (value < FP.Zero)
            {
                return false;
            }

            return FP.FastAbs(X) < value && FP.FastAbs(Y) < value && FP.FastAbs(Z) < value && (sqrMagnitude < value * value);

        }

        public bool IsLengthLessEqualThan(FP value)
        {
            return sqrMagnitude <= value * value;

            if (value < FP.Zero)
            {
                return false;
            }

            return FP.FastAbs(X) <= value && FP.FastAbs(Y) <= value && FP.FastAbs(Z) <= value && (sqrMagnitude <= value * value);

        }
        public static TSVector ClampMagnitude(TSVector vector, FP maxLength)
        {
            return Normalize(ref vector) * maxLength;
        }

        /// <summary>
        /// Gets a normalized version of the vector.
        /// </summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public TSVector normalized
        {
            get {
                TSVector result = new TSVector(this.X, this.Y, this.Z);
                result.Normalize();

                return result;
            }
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>

        public TSVector(int x, int y, int z)
        {
            this.X = (FP)x;
            this.Y = (FP)y;
            this.Z = (FP)z;
        }

        public TSVector(FP x, FP y, FP z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public TSVector(TSVector copy)
        {
            this.X = copy.X;
            this.Y = copy.Y;
            this.Z = copy.Z;
        }

        public static TSVector MoveTowards(TSVector current, TSVector target, FP maxDistanceDelta)
        {
            TSVector a = target - current;
            FP magnitude = a.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == FP.Zero)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public void Scale(TSVector other)
        {
            this.X = X * other.X;
            this.Y = Y * other.Y;
            this.Z = Z * other.Z;
        }

        /// <summary>
        /// Sets all vector component to specific values.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public void Set(FP x, FP y, FP z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="xyz">All components of the vector are set to xyz</param>
        public TSVector(FP xyz)
        {
            this.X = xyz;
            this.Y = xyz;
            this.Z = xyz;
        }

        public TSVector(FP xyz, FP v)
        {
            this.X = xyz;
            this.Y = v;
            this.Z = FP.Zero;
        }

        public static TSVector Lerp(TSVector from, TSVector to, FP percent)
        {
            return Lerp(ref from, ref to, percent);
        }

        public static TSVector Lerp(ref TSVector from, ref TSVector to, FP percent)
        {
            return from + (to - from) * FP.Clamp01(percent);
        }

        /// <summary>
        /// Tests if an object is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns true if they are euqal, otherwise false.</returns>
        #region public override bool Equals(object obj)
        public override bool Equals(object obj)
        {
            if (!(obj is TSVector))
            {
                return false;
            }
            TSVector other = (TSVector)obj;

            return (((X == other.X) && (Y == other.Y)) && (Z == other.Z));
        }
        #endregion

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public static TSVector Scale(TSVector vecA, TSVector vecB)
        {
            TSVector result;
            result.X = vecA.X * vecB.X;
            result.Y = vecA.Y * vecB.Y;
            result.Z = vecA.Z * vecB.Z;

            return result;
        }

        /// <summary>
        /// Tests if two JVector are equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns true if both values are equal, otherwise false.</returns>
        #region public static bool operator ==(JVector value1, JVector value2)
        public static bool operator ==(TSVector value1, TSVector value2)
        {
            return (((value1.X._serializedValue == value2.X._serializedValue) && (value1.Y._serializedValue == value2.Y._serializedValue)) && (value1.Z._serializedValue == value2.Z._serializedValue));
        }
        #endregion

        /// <summary>
        /// Tests if two JVector are not equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns false if both values are equal, otherwise true.</returns>
        #region public static bool operator !=(JVector value1, JVector value2)
        public static bool operator !=(TSVector value1, TSVector value2)
        {
            if ((value1.X._serializedValue == value2.X._serializedValue) && (value1.Y._serializedValue == value2.Y._serializedValue))
            {
                return (value1.Z._serializedValue != value2.Z._serializedValue);
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        #region public static JVector Min(JVector value1, JVector value2)

        public static TSVector Min(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Min(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
        public static void Min(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = (value1.X < value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            result.Z = (value1.Z < value2.Z) ? value1.Z : value2.Z;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        #region public static JVector Max(JVector value1, JVector value2)
        public static TSVector Max(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Max(ref value1, ref value2, out result);
            return result;
        }

        public static FP Distance(TSVector v1, TSVector v2)
        {
            return FP.Sqrt ((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y) + (v1.Z - v2.Z) * (v1.Z - v2.Z));
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
        public static void Max(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = (value1.X > value2.X) ? value1.X : value2.X;
            result.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            result.Z = (value1.Z > value2.Z) ? value1.Z : value2.Z;
        }
        #endregion

        /// <summary>
        /// Sets the length of the vector to zero.
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            X = FP.Zero;
            Y = FP.Zero;
            Z = FP.Zero;
        }
        #endregion

        /// <summary>
        /// Checks if the length of the vector is zero.
        /// </summary>
        /// <returns>Returns true if the vector is zero, otherwise false.</returns>
        #region public bool IsZero()
        public bool IsZero()
        {
            return (this.sqrMagnitude == FP.Zero);
        }

        /// <summary>
        /// Checks if the length of the vector is nearly zero.
        /// </summary>
        /// <returns>Returns true if the vector is nearly zero, otherwise false.</returns>
        public bool IsNearlyZero()
        {
            return (this.sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The transformed vector.</returns>
        #region public static JVector Transform(JVector position, JMatrix matrix)
        public static TSVector Transform(TSVector position, TSMatrix matrix)
        {
            TSVector result;
            TSVector.Transform(ref position, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void Transform(ref TSVector position, ref TSMatrix matrix, out TSVector result)
        {
            FP num0 = ((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31);
            FP num1 = ((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32);
            FP num2 = ((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33);

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }

        /// <summary>
        /// Transforms a vector by the transposed of the given Matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void TransposedTransform(ref TSVector position, ref TSMatrix matrix, out TSVector result)
        {
            FP num0 = ((position.X * matrix.M11) + (position.Y * matrix.M12)) + (position.Z * matrix.M13);
            FP num1 = ((position.X * matrix.M21) + (position.Y * matrix.M22)) + (position.Z * matrix.M23);
            FP num2 = ((position.X * matrix.M31) + (position.Y * matrix.M32)) + (position.Z * matrix.M33);

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        #region public static FP Dot(JVector vector1, JVector vector2)

        public FP Dot(ref TSVector v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public FP Dot(TSVector vector2)
        {
            return ((X * vector2.X) + (Y * vector2.Y)) + (Z * vector2.Z);
        }

        public static FP Dot(TSVector vector1, TSVector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) + (vector1.Z * vector2.Z);
        }
        /// <summary>
        /// Calculates the dot product of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static FP Dot(ref TSVector vector1, ref TSVector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) + (vector1.Z * vector2.Z);
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static void Add(JVector value1, JVector value2)
        public static TSVector Add(TSVector value1, TSVector value2)
        {
            TSVector result;
            TSVector.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// Adds to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The sum of both vectors.</param>
        public static void Add(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static TSVector Divide(TSVector value1, FP scaleFactor)
        {
            TSVector result;
            TSVector.Divide(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the scaled vector.</param>
        public static void Divide(ref TSVector value1, FP scaleFactor, out TSVector result)
        {
            result.X = value1.X / scaleFactor;
            result.Y = value1.Y / scaleFactor;
            result.Z = value1.Z / scaleFactor;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        public static TSVector Subtract(TSVector value1, TSVector value2)
        {
            return Subtract(ref value1, ref value2);
        }
        public static TSVector Subtract(ref TSVector value1, ref TSVector value2)
        {
            TSVector result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }
        /// <summary>
        /// Subtracts to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The difference of both vectors.</param>
        public static void Subtract(ref TSVector value1, ref TSVector value2, out TSVector result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }


        #endregion

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        #region public static JVector Cross(JVector vector1, JVector vector2)
        public static TSVector Cross(TSVector vector1, TSVector vector2)
        {
            TSVector result;
            result.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            result.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            result.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);

            return result;
        }
        public static TSVector Cross(ref TSVector vector1, ref TSVector vector2)
        {
            TSVector result;
            result.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            result.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            result.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
            return result;
        }

        public TSVector Cross(TSVector vector2)
        {
            TSVector result;

            result.X = (Y * vector2.Z) - (Z * vector2.Y);
            result.Y = (Z * vector2.X) - (X * vector2.Z);
            result.Z = (X * vector2.Y) - (Y * vector2.X);

            return result;
        }
        public TSVector Cross(ref TSVector vector2)
        {
            TSVector result;
            result.X = (Y * vector2.Z) - (Z * vector2.Y);
            result.Y = (Z * vector2.X) - (X * vector2.Z);
            result.Z = (X * vector2.Y) - (Y * vector2.X);

            return result;
        }
        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of both vectors.</param>
        public static void Cross(ref TSVector vector1, ref TSVector vector2, out TSVector result)
        {
            result.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            result.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            result.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
        }
        #endregion

        /// <summary>
        /// Gets the hashcode of the vector.
        /// </summary>
        /// <returns>Returns the hashcode of the vector.</returns>
        #region public override int GetHashCode()
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Inverses the direction of the vector.
        /// </summary>
        #region public static JVector Negate(JVector value)
        public void Negate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <returns>The negated vector.</returns>
        public static TSVector Negate(TSVector value)
        {
            TSVector result;
            TSVector.Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <param name="result">The negated vector.</param>
        public static void Negate(ref TSVector value, out TSVector result)
        {
            FP num0 = -value.X;
            FP num1 = -value.Y;
            FP num2 = -value.Z;

            result.X = num0;
            result.Y = num1;
            result.Z = num2;
        }
        #endregion

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        #region public static JVector Normalize(JVector value)
        public static TSVector Normalize(TSVector value)
        {
            return Normalize(ref value);
        }
        public static TSVector Normalize(ref TSVector value)
        {
            TSVector result;
            TSVector.Normalize(ref value, out result);
            return result;
        }
        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            //待优化，特别是x,y,z都很小的情况
            FP num2 = ((this.X * this.X) + (this.Y * this.Y)) + (this.Z * this.Z);
            if (num2 > FP.Zero)
            {
                //1/FP.Sqrt(num2) may be zero
                FP num =  FP.Sqrt(num2);
                num = FP.One / num;
                this.X *= num;
                this.Y *= num;
                this.Z *= num;
            }
            else
            {
                this.X = FP.Zero;
                this.Y = FP.Zero;
                this.Z = FP.Zero;
            }

        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="result">A normalized vector.</param>
        public static void Normalize(ref TSVector value, out TSVector result)
        {
            FP num2 = ((value.X * value.X) + (value.Y * value.Y)) + (value.Z * value.Z);
            if (num2 > FP.Zero)
            {
                //1/FP.Sqrt(num2) may be zero
                FP num = FP.Sqrt(num2);
                num = FP.One / num;
                result.X = value.X * num;
                result.Y = value.Y * num;
                result.Z = value.Z * num;
            }
            else
            {
                result.X = FP.Zero;
                result.Y = FP.Zero;
                result.Z = FP.Zero;
            }
        }
        #endregion

        #region public static void Swap(ref JVector vector1, ref JVector vector2)

        /// <summary>
        /// Swaps the components of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector to swap with the second.</param>
        /// <param name="vector2">The second vector to swap with the first.</param>
        public static void Swap(ref TSVector vector1, ref TSVector vector2)
        {
            FP temp;

            temp = vector1.X;
            vector1.X = vector2.X;
            vector2.X = temp;

            temp = vector1.Y;
            vector1.Y = vector2.Y;
            vector2.Y = temp;

            temp = vector1.Z;
            vector1.Z = vector2.Z;
            vector2.Z = temp;
        }
        #endregion

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        #region public static JVector Multiply(JVector value1, FP scaleFactor)
        public static TSVector Multiply(TSVector value1, FP scaleFactor)
        {
            value1.X = value1.X * scaleFactor;
            value1.Y = value1.Y * scaleFactor;
            value1.Z = value1.Z * scaleFactor;
            return value1;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the multiplied vector.</param>
        public static void Multiply(ref TSVector value1, FP scaleFactor, out TSVector result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        #endregion

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the cross product of both.</returns>
        #region public static JVector operator %(JVector value1, JVector value2)
        public static TSVector operator %(TSVector vector1, TSVector vector2)
        {
            TSVector result;
            FP num3 = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            FP num2 = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            FP num = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
            result.X = num3;
            result.Y = num2;
            result.Z = num;

            return result;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the dot product of both.</returns>
        #region public static FP operator *(JVector value1, JVector value2)
        public static FP operator *(TSVector vector1, TSVector vector2)
        {
            return ((vector1.X * vector2.X) + (vector1.Y * vector2.Y)) + (vector1.Z * vector2.Z);
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value1">The vector to scale.</param>
        /// <param name="value2">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(JVector value1, FP value2)
        public static TSVector operator *(TSVector value1, FP scaleFactor)
        {
            value1.X = value1.X * scaleFactor;
            value1.Y = value1.Y * scaleFactor;
            value1.Z = value1.Z * scaleFactor;

            return value1;
        }

        public static TSVector operator *(TSVector value1, int scaleFactor)
        {
            value1.X = value1.X * scaleFactor;
            value1.Y = value1.Y * scaleFactor;
            value1.Z = value1.Z * scaleFactor;

            return value1;
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value2">The vector to scale.</param>
        /// <param name="value1">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(FP value1, JVector value2)
        public static TSVector operator *(FP scaleFactor, TSVector value2)
        {
            value2.X = value2.X * scaleFactor;
            value2.Y = value2.Y * scaleFactor;
            value2.Z = value2.Z * scaleFactor;

            return value2;
        }

        public static TSVector operator *(int scaleFactor, TSVector value2)
        {
            value2.X = value2.X * scaleFactor;
            value2.Y = value2.Y * scaleFactor;
            value2.Z = value2.Z * scaleFactor;

            return value2;
        }
        #endregion

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector operator -(JVector value1, JVector value2)
        public static TSVector operator -(TSVector value1, TSVector value2)
        {
            value1.X._serializedValue = value1.X._serializedValue - value2.X._serializedValue;
            value1.Y._serializedValue = value1.Y._serializedValue - value2.Y._serializedValue;
            value1.Z._serializedValue = value1.Z._serializedValue - value2.Z._serializedValue;

            return value1;
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static JVector operator +(JVector value1, JVector value2)
        public static TSVector operator +(TSVector value1, TSVector value2)
        {
            value1.X._serializedValue = value1.X._serializedValue + value2.X._serializedValue;
            value1.Y._serializedValue = value1.Y._serializedValue + value2.Y._serializedValue;
            value1.Z._serializedValue = value1.Z._serializedValue + value2.Z._serializedValue;
            return value1;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static TSVector operator /(TSVector value1, FP scaleFactor)
        {
            //TSVector result;
            //TSVector.Divide(ref value1, value2, out result);

            value1.X = value1.X / scaleFactor;
            value1.Y = value1.Y / scaleFactor;
            value1.Z = value1.Z / scaleFactor;

            return value1;
        }

        public static FP Angle(TSVector a, TSVector b)
        {
            return FP.Acos((a.sqrMagnitude == FP.Zero ? a : a.normalized) * (b.sqrMagnitude == FP.Zero ? b : b.normalized)) * FP.Rad2Deg;
        }

        public static FP GetSignedAngle(TSVector vectorA, TSVector vectorB, TSVector up)
        {
            var angle = TSVector.Angle(vectorA, vectorB);
            TSVector cross = TSVector.Cross(vectorA, vectorB);
            if (cross.Y < FP.Zero)
            {
                angle = -angle;
            }
            return angle;
        }

        public static TSVector Rotate(TSVector p, FP angle)
        {
            var qr = TSQuaternion.Euler(0, angle, 0);
            TSMatrix4x4 m = TSMatrix4x4.TRS(TSVector.zero, qr, TSVector.one);
            var r = m.MultiplyPoint3x4(p);
            return r;
        }

        public TSVector2 ToTSVector2()
        {
            return new TSVector2(this.X, this.Z);
        }

        #region ChenPlus
        public static TSVector operator -(TSVector value1)
        {
            value1.X._serializedValue = -value1.X._serializedValue;
            value1.Y._serializedValue = -value1.Y._serializedValue;
            value1.Z._serializedValue = -value1.Z._serializedValue;

            return value1;
        }
        public static TSVector Project(TSVector vector1, TSVector vector2)
        {
            return Dot(ref vector1, ref vector2) / vector2.sqrMagnitude * vector2.normalized;
        }
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.X.AsFloat(), this.Y.AsFloat(), this.Z.AsFloat());
        }
        public int costMagnitude
        {
            get
            {
                return (int)FP.Round(magnitude);
            }
        }

        public static explicit operator TSVector2(TSVector value)
        {
            TSVector2 result;
            result.X = value.X;
            result.Y = value.Y;
            return result;
        }

        public FP this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index:" + index);
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.X = value;
                        break;
                    case 1:
                        this.Y = value;
                        break;
                    case 2:
                        this.Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index:" + index);
                }
            }
        }

        public TSVector Abs()
        {
            TSVector result;
            result.X._serializedValue = X._serializedValue < FP.Zero._serializedValue ? -X._serializedValue : X._serializedValue;
            result.Y._serializedValue = Y._serializedValue < FP.Zero._serializedValue ? -Y._serializedValue : Y._serializedValue;
            result.Z._serializedValue = Z._serializedValue < FP.Zero._serializedValue ? -Z._serializedValue : Z._serializedValue;
            return result;
        }

        public void abs()
        {
            X._serializedValue = X._serializedValue < FP.Zero._serializedValue ? -X._serializedValue : X._serializedValue;
            Y._serializedValue = Y._serializedValue < FP.Zero._serializedValue ? -Y._serializedValue : Y._serializedValue;
            Z._serializedValue = Z._serializedValue < FP.Zero._serializedValue ? -Z._serializedValue : Z._serializedValue;
        }
        public TSVector Absolute()
        {
            return new TSVector(FP.FastAbs(X), FP.FastAbs(Y), FP.FastAbs(Z));
        }

        public int MaxAxis()
        {
            return X < Y ? (Y < Z ? 2 : 1) : (X < Z ? 2 : 0);
        }

        public int MinAxis()
        {
            return X < Y ? (X < Z ? 0 : 2) : (Y < Z ? 1 : 2);
        }
        public FP Triple(ref TSVector b, ref TSVector c)
        {
            return X * (b.Y * c.Z - b.Z * c.Y) +
                   Y * (b.Z * c.X - b.X * c.Z) +
                   Z * (b.X * c.Y - b.Y * c.X);
        }

        public void SetMin(ref TSVector v)
        {
            if (v.X < X)
            {
                X = v.X;
            }
            if (v.Y < Y)
            {
                Y = v.Y;
            }
            if (v.Z < Z)
            {
                Z = v.Z;
            }
        }


        public void SetMax(ref TSVector v)
        {
            if (v.X > X)
            {
                X = v.X;
            }
            if (v.Y > Y)
            {
                Y = v.Y;
            }
            if (v.Z > Z)
            {
                Z = v.Z;
            }
        }

        #endregion

        /// Set this matrix to all zeros.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetZero()
        {
            X = FP.Zero;
            Y = FP.Zero;
            Z = FP.Zero;
        }
    }

}
#endif