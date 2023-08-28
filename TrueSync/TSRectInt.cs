using System;
namespace TrueSync
{
    [Serializable]
    public struct TSRectInt : IEquatable<TSRectInt>
    {
        public int x, y, width, height;

        public TSVector2 center { get { return new TSVector2(x + width / 2f, y + height / 2f); } }
        public TSVector2Int min { get { return new TSVector2Int(xMin, yMin); } set { xMin = value.x; yMin = value.y; } }
        public TSVector2Int max { get { return new TSVector2Int(xMax, yMax); } set { xMax = value.x; yMax = value.y; } }

        public int xMin { get { return System.Math.Min(x, x + width); } set { int oldxmax = xMax; x = value; width = oldxmax - x; } }
        public int yMin { get { return System.Math.Min(y, y + height); } set { int oldymax = yMax; y = value; height = oldymax - y; } }
        public int xMax { get { return System.Math.Max(x, x + width); } set { width = value - x; } }
        public int yMax { get { return System.Math.Max(y, y + height); } set { height = value - y; } }

        public TSVector2Int position { get { return new TSVector2Int(x, y); } set { x = value.x; y = value.y; } }
        public TSVector2Int size { get { return new TSVector2Int(width, height); } set { width = value.x; height = value.y; } }

        public void SetMinMax(TSVector2Int minPosition, TSVector2Int maxPosition)
        {
            min = minPosition;
            max = maxPosition;
        }

        public TSRectInt(int xMin, int yMin, int width, int height)
        {
            x = xMin;
            y = yMin;
            this.width = width;
            this.height = height;
        }

        public TSRectInt(TSVector2Int position, TSVector2Int size)
        {
            x = position.x;
            y = position.y;
            width = size.x;
            height = size.y;
        }

        public void ClampToBounds(TSRectInt bounds)
        {
            position = new TSVector2Int(
                System.Math.Max(System.Math.Min(bounds.xMax, position.x), bounds.xMin),
                System.Math.Max(System.Math.Min(bounds.yMax, position.y), bounds.yMin)
            );
            size = new TSVector2Int(
                System.Math.Min(bounds.xMax - position.x, size.x),
                System.Math.Min(bounds.yMax - position.y, size.y)
            );
        }

        public bool Contains(TSVector2Int position)
        {
            return position.x >= xMin
                && position.y >= yMin
                && position.x < xMax
                && position.y < yMax;
        }

        public bool Overlaps(TSRectInt other)
        {
            return other.xMin < xMax
                && other.xMax > xMin
                && other.yMin < yMax
                && other.yMax > yMin;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string format)
        {
            return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", x.ToString(format), y.ToString(format), width.ToString(format), height.ToString(format));
        }

        public bool Equals(TSRectInt other)
        {
            return x == other.x &&
                y == other.y &&
                width == other.width &&
                height == other.height;
        }
    }
}
