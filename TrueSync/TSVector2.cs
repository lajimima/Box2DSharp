#region License

/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License

using System;
#if USEBATTLEDLL
#else
namespace TrueSync {

    [Serializable]
    public struct TSVector2 : IEquatable<TSVector2>
    {
#region Private Fields

        public static readonly TSVector2 Zero = new TSVector2(0, 0);
        public static readonly TSVector2 One = new TSVector2(1, 1);

        public static readonly TSVector2 Right = new TSVector2(1, 0);
        public static readonly TSVector2 Left = new TSVector2(-1, 0);

        public static readonly TSVector2 Up = new TSVector2(0, 1);
        public static readonly TSVector2 Down = new TSVector2(0, -1);
        #endregion Private Fields

        #region Public Fields

        public FP X;
        public FP Y;

#endregion Public Fields

#region Properties
        /*
        public static TSVector2 zero
        {
            get { return zeroVector; }
        }

        public static TSVector2 one
        {
            get { return oneVector; }
        }

        public static TSVector2 right
        {
            get { return rightVector; }
        }

        public static TSVector2 left {
            get { return leftVector; }
        }

        public static TSVector2 up
        {
            get { return upVector; }
        }

        public static TSVector2 down {
            get { return downVector; }
        }
        */
#endregion Properties

#region Constructors

        /// <summary>
        /// Constructor foe standard 2D vector.
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="y">
        /// A <see cref="System.Single"/>
        /// </param>
        public TSVector2(FP x, FP y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructor for "square" vector.
        /// </summary>
        /// <param name="value">
        /// A <see cref="System.Single"/>
        /// </param>
        public TSVector2(FP value)
        {
            X = value;
            Y = value;
        }

        public void Set(FP x, FP y) {
            this.X = x;
            this.Y = y;
        }

#endregion Constructors

#region Public Methods

        public static void Reflect(ref TSVector2 vector, ref TSVector2 normal, out TSVector2 result)
        {
            FP dot = Dot(vector, normal);
            result.X = vector.X - ((2*dot)*normal.X);
            result.Y = vector.Y - ((2*dot)*normal.Y);
        }

        public static TSVector2 Reflect(TSVector2 vector, TSVector2 normal)
        {
            TSVector2 result;
            Reflect(ref vector, ref normal, out result);
            return result;
        }

        public static TSVector2 Add(TSVector2 value1, TSVector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static void Add(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static TSVector2 Barycentric(TSVector2 value1, TSVector2 value2, TSVector2 value3, FP amount1, FP amount2)
        {
            return new TSVector2(
                TSMath.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                TSMath.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static void Barycentric(ref TSVector2 value1, ref TSVector2 value2, ref TSVector2 value3, FP amount1,
                                       FP amount2, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                TSMath.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static TSVector2 CatmullRom(TSVector2 value1, TSVector2 value2, TSVector2 value3, TSVector2 value4, FP amount)
        {
            return new TSVector2(
                TSMath.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                TSMath.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static void CatmullRom(ref TSVector2 value1, ref TSVector2 value2, ref TSVector2 value3, ref TSVector2 value4,
                                      FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                TSMath.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static TSVector2 Clamp(TSVector2 value1, TSVector2 min, TSVector2 max)
        {
            return new TSVector2(
                FP.Clamp(value1.X, min.X, max.X),
                FP.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref TSVector2 value1, ref TSVector2 min, ref TSVector2 max, out TSVector2 result)
        {
            result = new TSVector2(
                FP.Clamp(value1.X, min.X, max.X),
                FP.Clamp(value1.Y, min.Y, max.Y));
        }

        /// <summary>
        /// Returns FP precison distanve between two vectors
        /// </summary>
        /// <param name="value1">
        /// A <see cref="TSVector2"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="TSVector2"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Single"/>
        /// </returns>
        public static FP Distance(TSVector2 value1, TSVector2 value2)
        {
            FP result;
            DistanceSquared(ref value1, ref value2, out result);
            return (FP) FP.Sqrt(result);
        }


        public static void Distance(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = (FP) FP.Sqrt(result);
        }

        public static FP DistanceSquared(TSVector2 value1, TSVector2 value2)
        {
            FP result;
            DistanceSquared(ref value1, ref value2, out result);
            return result;
        }

        public static void DistanceSquared(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            result = (value1.X - value2.X)*(value1.X - value2.X) + (value1.Y - value2.Y)*(value1.Y - value2.Y);
        }

        /// <summary>
        /// Devide first vector with the secund vector
        /// </summary>
        /// <param name="value1">
        /// A <see cref="TSVector2"/>
        /// </param>
        /// <param name="value2">
        /// A <see cref="TSVector2"/>
        /// </param>
        /// <returns>
        /// A <see cref="TSVector2"/>
        /// </returns>
        public static TSVector2 Divide(TSVector2 value1, TSVector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static void Divide(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = value1.X/value2.X;
            result.Y = value1.Y/value2.Y;
        }

        public static TSVector2 Divide(TSVector2 value1, FP divider)
        {
            FP factor = 1/divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        public static void Divide(ref TSVector2 value1, FP divider, out TSVector2 result)
        {
            FP factor = 1/divider;
            result.X = value1.X*factor;
            result.Y = value1.Y*factor;
        }

        public static FP Dot(TSVector2 value1, TSVector2 value2)
        {
            return value1.X*value2.X + value1.Y*value2.Y;
        }
        public static FP Dot(ref TSVector2 value1, ref TSVector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }
        public static void Dot(ref TSVector2 value1, ref TSVector2 value2, out FP result)
        {
            result = value1.X*value2.X + value1.Y*value2.Y;
        }

        public override bool Equals(object obj)
        {
            return (obj is TSVector2) ? this == ((TSVector2) obj) : false;
        }

        public bool Equals(TSVector2 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return (int) (X + Y);
        }

        public static TSVector2 Hermite(TSVector2 value1, TSVector2 tangent1, TSVector2 value2, TSVector2 tangent2, FP amount)
        {
            TSVector2 result = new TSVector2();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        public static void Hermite(ref TSVector2 value1, ref TSVector2 tangent1, ref TSVector2 value2, ref TSVector2 tangent2,
                                   FP amount, out TSVector2 result)
        {
            result.X = TSMath.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = TSMath.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        }

        public FP magnitude {
            get {
                return FP.Sqrt(X * X + Y * Y);
            }
        }

        public static TSVector2 ClampMagnitude(TSVector2 vector, FP maxLength) {
            return Normalize(vector) * maxLength;
        }
        public FP Length
        {
            get
            {
                return FP.Sqrt(X * X + Y * Y);
            }
        }
        public FP LengthSquared()
        {
           return  X  * X + Y * Y ;
        }

        public static TSVector2 Lerp(TSVector2 value1, TSVector2 value2, FP amount) {
            amount = FP.Clamp(amount, 0, 1);

            return new TSVector2(
                FP.Lerp(value1.X, value2.X, amount),
                FP.Lerp(value1.Y, value2.Y, amount));
        }

        public static TSVector2 LerpUnclamped(TSVector2 value1, TSVector2 value2, FP amount)
        {
            return new TSVector2(
                FP.Lerp(value1.X, value2.X, amount),
                FP.Lerp(value1.Y, value2.Y, amount));
        }

        public static void LerpUnclamped(ref TSVector2 value1, ref TSVector2 value2, FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                FP.Lerp(value1.X, value2.X, amount),
                FP.Lerp(value1.Y, value2.Y, amount));
        }

        public static TSVector2 Max(TSVector2 value1, TSVector2 value2)
        {
            return new TSVector2(
                TSMath.Max(value1.X, value2.X),
                TSMath.Max(value1.Y, value2.Y));
        }

        public static void Max(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = TSMath.Max(value1.X, value2.X);
            result.Y = TSMath.Max(value1.Y, value2.Y);
        }

        public static TSVector2 Min(TSVector2 value1, TSVector2 value2)
        {
            return new TSVector2(
                TSMath.Min(value1.X, value2.X),
                TSMath.Min(value1.Y, value2.Y));
        }

        public static void Min(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = TSMath.Min(value1.X, value2.X);
            result.Y = TSMath.Min(value1.Y, value2.Y);
        }

        public void Scale(TSVector2 other) {
            this.X = X * other.X;
            this.Y = Y * other.Y;
        }

        public static TSVector2 Scale(TSVector2 value1, TSVector2 value2) {
            TSVector2 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;

            return result;
        }

        public static TSVector2 Multiply(TSVector2 value1, TSVector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static TSVector2 Multiply(TSVector2 value1, FP scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref TSVector2 value1, FP scaleFactor, out TSVector2 result)
        {
            result.X = value1.X*scaleFactor;
            result.Y = value1.Y*scaleFactor;
        }

        public static void Multiply(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = value1.X*value2.X;
            result.Y = value1.Y*value2.Y;
        }

        public static TSVector2 Negate(TSVector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static void Negate(ref TSVector2 value, out TSVector2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        public void Normalize()
        {
            FP factor = X * X + Y * Y;
            if (factor > FP.Zero)
            {
                factor = FP.Sqrt(factor);
                X /= factor;
                Y /= factor;
            }
        }

        public static TSVector2 Normalize(TSVector2 value)
        {
            Normalize(ref value, out value);
            return value;
        }

        public TSVector2 normalized {
            get {
                TSVector2 result;
                TSVector2.Normalize(ref this, out result);

                return result;
            }
        }

        public static void Normalize(ref TSVector2 value, out TSVector2 result)
        {
            FP factor = value.X * value.X + value.Y * value.Y;
            if (factor > FP.Zero)
            {
                factor = FP.Sqrt(factor);
                result.X = value.X / factor;
                result.Y = value.Y / factor;
            }
            else
            {
                result.X = value.X;
                result.Y = value.Y;
            }
        }

        public static TSVector2 SmoothStep(TSVector2 value1, TSVector2 value2, FP amount)
        {
            return new TSVector2(
                TSMath.SmoothStep(value1.X, value2.X, amount),
                TSMath.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static void SmoothStep(ref TSVector2 value1, ref TSVector2 value2, FP amount, out TSVector2 result)
        {
            result = new TSVector2(
                TSMath.SmoothStep(value1.X, value2.X, amount),
                TSMath.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static TSVector2 Subtract(TSVector2 value1, TSVector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static void Subtract(ref TSVector2 value1, ref TSVector2 value2, out TSVector2 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        public static FP Angle(TSVector2 a, TSVector2 b) {
            return FP.Acos((a.sqrMagnitude == FP.Zero ? a : a.normalized) * (b.sqrMagnitude == FP.Zero ? b : b.normalized)) * FP.Rad2Deg;
        }


        public static TSVector2 RotateAround(  FP radius,ref TSVector2 point,FP angle )
        {   
            TSVector2 ret = TSVector2.Zero;
            ret.X = point.X + (radius * TSMath.Cos(angle));
            ret.Y = point.Y + (radius * TSMath.Sin(angle));
            return ret;
        }

        public static TSVector2 Eclipse(FP radius, FP radiusB, TSVector2 point, FP angle)
        {   
            TSVector2 ret = TSVector2.Zero;
            ret.X = point.X + (radius * TSMath.Cos(angle));
            ret.Y = point.Y + (radiusB * TSMath.Sin(angle));
            return ret;
        }   

        /// <summary>
        /// 转换为三维坐标， TSVector.z = TSVector2.y
        /// </summary>
        /// <returns></returns>
        public TSVector ToTSVector() {
            return new TSVector(this.X, FP.Zero, this.Y);
        }

        /// <summary>
        /// 转换为三维坐标， TSVector.z = TSVector2.y
        /// </summary>
        /// <returns></returns>
        public TSVector ToTSVector(FP y)
        {
            return new TSVector(this.X, y, this.Y);
        }


        public override string ToString() {
            return string.Format("({0:f1}, {1:f1})", X.AsFloat(), Y.AsFloat());
        }

#endregion Public Methods

#region Operators

        public static TSVector2 operator -(TSVector2 value)
        {
            value.X._serializedValue = -value.X._serializedValue;
            value.Y._serializedValue = -value.Y._serializedValue;
            return value;
        }


        public static bool operator ==(TSVector2 value1, TSVector2 value2)
        {
            return value1.X._serializedValue == value2.X._serializedValue && value1.Y._serializedValue == value2.Y._serializedValue;
        }


        public static bool operator !=(TSVector2 value1, TSVector2 value2)
        {
            return value1.X._serializedValue != value2.X._serializedValue || value1.Y._serializedValue != value2.Y._serializedValue;
        }


        public static TSVector2 operator +(TSVector2 value1, TSVector2 value2)
        {
            value1.X._serializedValue += value2.X._serializedValue;
            value1.Y._serializedValue += value2.Y._serializedValue;
            return value1;
        }


        public static TSVector2 operator -(TSVector2 value1, TSVector2 value2)
        {
            value1.X._serializedValue -= value2.X._serializedValue;
            value1.Y._serializedValue -= value2.Y._serializedValue;
            return value1;
        }


        public static FP operator *(TSVector2 value1, TSVector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }


        public static TSVector2 operator *(TSVector2 value, FP scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static TSVector2 operator *(FP scaleFactor, TSVector2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static TSVector2 operator /(TSVector2 value1, TSVector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }


        public static TSVector2 operator /(TSVector2 value1, FP divider)
        {
            FP factor = FP.One/divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

#endregion Operators

#region ChenPlus
        public FP sqrMagnitude
        {
            get
            {
                return ((this.X * this.X) + (this.Y * this.Y));
            }
        }

        public static explicit operator TSVector(TSVector2 value)
        {
            TSVector result;
            result.X = value.X;
            result.Y = value.Y;
            result.Z = FP.Zero;
            return result;
        }
#endregion

        public static TSVector2 Abs(TSVector2 vec2)
        {
            return new TSVector2 { X = FP.Abs(vec2.X), Y = FP.Abs(vec2.Y), };
        }
    }
}
#endif