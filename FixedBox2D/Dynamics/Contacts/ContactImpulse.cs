using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Contacts
{
    /// Contact impulses for reporting. Impulses are used instead of forces because
    /// sub-step forces may approach infinity for rigid body collisions. These
    /// match up one-to-one with the contact points in b2Manifold.
    public struct ContactImpulse
    {
        public FixedArray2<float> NormalImpulses;

        public FixedArray2<float> TangentImpulses;

        public int Count;
    }
}