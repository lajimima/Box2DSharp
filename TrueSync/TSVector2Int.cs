using System;

namespace TrueSync
{
    [Serializable]
    public struct TSVector2Int : IEquatable<TSVector2Int>
    {
        public int x;
        public int y;

        public TSVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public long sqrMagnitudeLong
        {
            get
            {
                return (long)x * (long)x + (long)y * (long)y;
            }
        }

        public static TSVector2Int operator +(TSVector2Int a, TSVector2Int b)
        {
            return new TSVector2Int(a.x + b.x, a.y + b.y);
        }

        public static TSVector2Int operator -(TSVector2Int a, TSVector2Int b)
        {
            return new TSVector2Int(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(TSVector2Int a, TSVector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(TSVector2Int a, TSVector2Int b)
        {
            return a.x != b.x || a.y != b.y;
        }

        /// <summary>Dot product of the two coordinates</summary>
        public static long DotLong(TSVector2Int a, TSVector2Int b)
        {
            return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
        }

        public bool Equals(TSVector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null) return false;
            var rhs = (TSVector2Int)o;

            return x == rhs.x && y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x * 49157 + y * 98317;
        }

        public static TSVector2Int Min(TSVector2Int a, TSVector2Int b)
        {
            return new TSVector2Int(System.Math.Min(a.x, b.x), System.Math.Min(a.y, b.y));
        }

        public static TSVector2Int Max(TSVector2Int a, TSVector2Int b)
        {
            return new TSVector2Int(System.Math.Max(a.x, b.x), System.Math.Max(a.y, b.y));
        }

        public static TSVector2Int FromInt3XZ(TSVectorInt o)
        {
            return new TSVector2Int(o.x, o.z);
        }

        public static TSVectorInt ToInt3XZ(TSVector2Int o)
        {
            return new TSVectorInt(o.x, 0, o.y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}
