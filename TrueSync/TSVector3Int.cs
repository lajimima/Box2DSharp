using System;

namespace TrueSync
{
    public struct TSVectorInt : IEquatable<TSVectorInt>
    {
        public int x;
        public int y;
        public int z;

        //These should be set to the same value (only PrecisionFactor should be 1 divided by Precision)

        /// <summary>
        /// Precision for the integer coordinates.
        /// One world unit is divided into [value] pieces. A value of 1000 would mean millimeter precision, a value of 1 would mean meter precision (assuming 1 world unit = 1 meter).
        /// This value affects the maximum coordinates for nodes as well as how large the cost values are for moving between two nodes.
        /// A higher value means that you also have to set all penalty values to a higher value to compensate since the normal cost of moving will be higher.
        /// </summary>

        public static readonly TSVectorInt zero = new TSVectorInt(0, 0, 0);

        public TSVectorInt(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static bool operator ==(TSVectorInt lhs, TSVectorInt rhs)
        {
            return lhs.x == rhs.x &&
                   lhs.y == rhs.y &&
                   lhs.z == rhs.z;
        }

        public static bool operator !=(TSVectorInt lhs, TSVectorInt rhs)
        {
            return lhs.x != rhs.x ||
                   lhs.y != rhs.y ||
                   lhs.z != rhs.z;
        }

        public static TSVectorInt operator -(TSVectorInt lhs, TSVectorInt rhs)
        {
            lhs.x -= rhs.x;
            lhs.y -= rhs.y;
            lhs.z -= rhs.z;
            return lhs;
        }

        public static TSVectorInt operator -(TSVectorInt lhs)
        {
            lhs.x = -lhs.x;
            lhs.y = -lhs.y;
            lhs.z = -lhs.z;
            return lhs;
        }

        public static TSVectorInt operator +(TSVectorInt lhs, TSVectorInt rhs)
        {
            lhs.x += rhs.x;
            lhs.y += rhs.y;
            lhs.z += rhs.z;
            return lhs;
        }

        public static TSVectorInt operator *(TSVectorInt lhs, int rhs)
        {
            lhs.x *= rhs;
            lhs.y *= rhs;
            lhs.z *= rhs;

            return lhs;
        }

        public static TSVectorInt operator *(TSVectorInt lhs, FP rhs)
        {
            lhs.x = (int)TSMath.Round(lhs.x * rhs);
            lhs.y = (int)TSMath.Round(lhs.y * rhs);
            lhs.z = (int)TSMath.Round(lhs.z * rhs);

            return lhs;
        }

        public static TSVectorInt operator /(TSVectorInt lhs, FP rhs)
        {
            lhs.x = (int)TSMath.Round(lhs.x / rhs);
            lhs.y = (int)TSMath.Round(lhs.y / rhs);
            lhs.z = (int)TSMath.Round(lhs.z / rhs);
            return lhs;
        }

        public int this[int i]
        {
            get
            {
                return i == 0 ? x : (i == 1 ? y : z);
            }
            set
            {
                if (i == 0) x = value;
                else if (i == 1) y = value;
                else z = value;
            }
        }

        /// <summary>Angle between the vectors in radians</summary>
        public static FP Angle(TSVectorInt lhs, TSVectorInt rhs)
        {
            FP cos = Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude);

            cos = cos < -1 ? -1 : (cos > 1 ? 1 : cos);
            return TSMath.Acos(cos);
        }

        public static int Dot(TSVectorInt lhs, TSVectorInt rhs)
        {
            return
                lhs.x * rhs.x +
                lhs.y * rhs.y +
                lhs.z * rhs.z;
        }

        public static long DotLong(TSVectorInt lhs, TSVectorInt rhs)
        {
            return
                (long)lhs.x * (long)rhs.x +
                (long)lhs.y * (long)rhs.y +
                (long)lhs.z * (long)rhs.z;
        }

        /// <summary>
        /// Normal in 2D space (XZ).
        /// Equivalent to Cross(this, Int3(0,1,0) )
        /// except that the Y coordinate is left unchanged with this operation.
        /// </summary>
        public TSVectorInt Normal2D()
        {
            return new TSVectorInt(z, y, -x);
        }

        /// <summary>
        /// Returns the magnitude of the vector. The magnitude is the 'length' of the vector from 0,0,0 to this point. Can be used for distance calculations:
        /// <code> Debug.Log ("Distance between 3,4,5 and 6,7,8 is: "+(new Int3(3,4,5) - new Int3(6,7,8)).magnitude); </code>
        /// </summary>
        public FP magnitude
        {
            get
            {
                return TSMath.Sqrt(x * x + y * y + z * z);
            }
        }

        /// <summary>
        /// Magnitude used for the cost between two nodes. The default cost between two nodes can be calculated like this:
        /// <code> int cost = (node1.position-node2.position).costMagnitude; </code>
        ///
        /// This is simply the magnitude, rounded to the nearest integer
        /// </summary>
        public int costMagnitude
        {
            get
            {
                return (int)TSMath.Round(magnitude);
            }
        }

        /// <summary>The squared magnitude of the vector</summary>
        public FP sqrMagnitude
        {
            get
            {
                return (x * x + y * y + z * z);
            }
        }

        /// <summary>The squared magnitude of the vector</summary>
        public long sqrMagnitudeLong
        {
            get
            {
                long _x = x;
                long _y = y;
                long _z = z;
                return (_x * _x + _y * _y + _z * _z);
            }
        }

        public static implicit operator string(TSVectorInt obj)
        {
            return obj.ToString();
        }

        /// <summary>Returns a nicely formatted string representing the vector</summary>
        public override string ToString()
        {
            return "( " + x + ", " + y + ", " + z + ")";
        }

        public override int GetHashCode()
        {
            return x * 73856093 ^ y * 19349663 ^ z * 83492791;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TSVectorInt)) return false;
            TSVectorInt other = (TSVectorInt)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }

        public bool Equals(TSVectorInt other)
        {
            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }
    }
}
