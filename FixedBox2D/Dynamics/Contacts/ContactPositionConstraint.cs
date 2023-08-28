using TrueSync;
using FixedBox2D.Collision.Collider;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Contacts
{
    public struct ContactPositionConstraint
    {
        /// <summary>
        /// Size <see cref="Settings.MaxManifoldPoints"/>
        /// </summary>
        public FixedArray2<TSVector2> LocalPoints;

        public int IndexA;

        public int IndexB;

        public FP InvIa, InvIb;

        public FP InvMassA, InvMassB;

        public TSVector2 LocalCenterA, LocalCenterB;

        public TSVector2 LocalNormal;

        public TSVector2 LocalPoint;

        public int PointCount;

        public FP RadiusA, RadiusB;

        public ManifoldType Type;
    }
}