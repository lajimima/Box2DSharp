using System;
using TrueSync;

namespace FixedBox2D.Common
{
    public struct Transform : IFormattable
    {
        public TSVector2 Position;

        public Rotation Rotation;

        /// Initialize using a position vector and a rotation.
        public Transform(in TSVector2 position, in Rotation rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Transform(in TSVector2 position, FP angle)
        {
            Position = position;
            Rotation = new Rotation(angle);
        }

        /// Set this to the identity transform.
        public void SetIdentity()
        {
            Position = TSVector2.Zero;
            Rotation.SetIdentity();
        }

        /// Set this based on the position and angle.
        public void Set(in TSVector2 position, FP angle)
        {
            Position = position;
            Rotation.Set(angle);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }

        public new string ToString()
        {
            return $"({Position.X},{Position.Y}), Cos:{Rotation.Cos}, Sin:{Rotation.Sin})";
        }
    }
}