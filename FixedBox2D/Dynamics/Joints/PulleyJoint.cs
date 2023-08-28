using System;
using System.Diagnostics;
using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Dynamics.Joints
{
    /// The pulley joint is connected to two bodies and two fixed ground points.
    /// The pulley supports a ratio such that:
    /// length1 + ratio * length2 <= constant
    /// Yes, the force transmitted is scaled by the ratio.
    /// Warning: the pulley joint can get a bit squirrelly by itself. They often
    /// work better when combined with prismatic joints. You should also cover the
    /// the anchor points with static shapes to prevent one side from going to
    /// zero length.
    public class PulleyJoint : Joint
    {
        private readonly FP _constant;

        private readonly FP _lengthA;

        private readonly FP _lengthB;

        // Solver shared
        private readonly TSVector2 _localAnchorA;

        private readonly TSVector2 _localAnchorB;

        private readonly FP _ratio;

        private TSVector2 _groundAnchorA;

        private TSVector2 _groundAnchorB;

        private FP _impulse;

        // Solver temp
        private int _indexA;

        private int _indexB;

        private FP _invIa;

        private FP _invIb;

        private FP _invMassA;

        private FP _invMassB;

        private TSVector2 _localCenterA;

        private TSVector2 _localCenterB;

        private FP _mass;

        private TSVector2 _rA;

        private TSVector2 _rB;

        private TSVector2 _uA;

        private TSVector2 _uB;

        public PulleyJoint(PulleyJointDef def) : base(def)
        {
            _groundAnchorA = def.GroundAnchorA;
            _groundAnchorB = def.GroundAnchorB;
            _localAnchorA = def.LocalAnchorA;
            _localAnchorB = def.LocalAnchorB;

            _lengthA = def.LengthA;
            _lengthB = def.LengthB;

            Debug.Assert(!def.Ratio.Equals(FP.Zero));
            _ratio = def.Ratio;

            _constant = def.LengthA + _ratio * def.LengthB;

            _impulse = FP.Zero;
        }

        /// Get the first ground anchor.
        public TSVector2 GetGroundAnchorA()
        {
            return _groundAnchorA;
        }

        /// Get the second ground anchor.
        public TSVector2 GetGroundAnchorB()
        {
            return _groundAnchorB;
        }

        /// Get the current length of the segment attached to bodyA.
        public FP GetLengthA()
        {
            return _lengthA;
        }

        /// Get the current length of the segment attached to bodyB.
        public FP GetLengthB()
        {
            return _lengthB;
        }

        /// Get the pulley ratio.
        public FP GetRatio()
        {
            return _ratio;
        }

        /// Get the current length of the segment attached to bodyA.
        public FP GetCurrentLengthA()
        {
            var p = BodyA.GetWorldPoint(_localAnchorA);
            var s = _groundAnchorA;
            var d = p - s;
            return d.magnitude;
        }

        /// Get the current length of the segment attached to bodyB.
        public FP GetCurrentLengthB()
        {
            var p = BodyB.GetWorldPoint(_localAnchorB);
            var s = _groundAnchorB;
            var d = p - s;
            return d.magnitude;
        }

        /// <inheritdoc />
        public override TSVector2 GetAnchorA()
        {
            return BodyA.GetWorldPoint(_localAnchorA);
        }

        /// <inheritdoc />
        public override TSVector2 GetAnchorB()
        {
            return BodyB.GetWorldPoint(_localAnchorB);
        }

        /// <inheritdoc />
        public override TSVector2 GetReactionForce(FP inv_dt)
        {
            var P = _impulse * _uB;
            return inv_dt * P;
        }

        /// <inheritdoc />
        public override FP GetReactionTorque(FP inv_dt)
        {
            return FP.Zero;
        }

        /// <inheritdoc />
        internal override void InitVelocityConstraints(in SolverData data)
        {
            _indexA = BodyA.IslandIndex;
            _indexB = BodyB.IslandIndex;
            _localCenterA = BodyA.Sweep.LocalCenter;
            _localCenterB = BodyB.Sweep.LocalCenter;
            _invMassA = BodyA.InvMass;
            _invMassB = BodyB.InvMass;
            _invIa = BodyA.InverseInertia;
            _invIb = BodyB.InverseInertia;

            var cA = data.Positions[_indexA].Center;
            var aA = data.Positions[_indexA].Angle;
            var vA = data.Velocities[_indexA].V;
            var wA = data.Velocities[_indexA].W;

            var cB = data.Positions[_indexB].Center;
            var aB = data.Positions[_indexB].Angle;
            var vB = data.Velocities[_indexB].V;
            var wB = data.Velocities[_indexB].W;

            var qA = new Rotation(aA);
            var qB = new Rotation(aB);

            _rA = MathUtils.Mul(qA, _localAnchorA - _localCenterA);
            _rB = MathUtils.Mul(qB, _localAnchorB - _localCenterB);

            // Get the pulley axes.
            _uA = cA + _rA - _groundAnchorA;
            _uB = cB + _rB - _groundAnchorB;

            var lengthA = _uA.magnitude;
            var lengthB = _uB.magnitude;

            if (lengthA > 10.0f * Settings.LinearSlop)
            {
                _uA *= FP.One / lengthA;
            }
            else
            {
                _uA.SetZero();
            }

            if (lengthB > 10.0f * Settings.LinearSlop)
            {
                _uB *= FP.One / lengthB;
            }
            else
            {
                _uB.SetZero();
            }

            // Compute effective mass.
            var ruA = MathUtils.Cross(_rA, _uA);
            var ruB = MathUtils.Cross(_rB, _uB);

            var mA = _invMassA + _invIa * ruA * ruA;
            var mB = _invMassB + _invIb * ruB * ruB;

            _mass = mA + _ratio * _ratio * mB;

            if (_mass > FP.Zero)
            {
                _mass = FP.One / _mass;
            }

            if (data.Step.WarmStarting)
            {
                // Scale impulses to support variable time steps.
                _impulse *= data.Step.DtRatio;

                // Warm starting.
                var PA = -_impulse * _uA;
                var PB = -_ratio * _impulse * _uB;

                vA += _invMassA * PA;
                wA += _invIa * MathUtils.Cross(_rA, PA);
                vB += _invMassB * PB;
                wB += _invIb * MathUtils.Cross(_rB, PB);
            }
            else
            {
                _impulse = FP.Zero;
            }

            data.Velocities[_indexA].V = vA;
            data.Velocities[_indexA].W = wA;
            data.Velocities[_indexB].V = vB;
            data.Velocities[_indexB].W = wB;
        }

        /// <inheritdoc />
        internal override void SolveVelocityConstraints(in SolverData data)
        {
            var vA = data.Velocities[_indexA].V;
            var wA = data.Velocities[_indexA].W;
            var vB = data.Velocities[_indexB].V;
            var wB = data.Velocities[_indexB].W;

            var vpA = vA + MathUtils.Cross(wA, _rA);
            var vpB = vB + MathUtils.Cross(wB, _rB);

            var Cdot = -TSVector2.Dot(_uA, vpA) - _ratio * TSVector2.Dot(_uB, vpB);
            var impulse = -_mass * Cdot;
            _impulse += impulse;

            var PA = -impulse * _uA;
            var PB = -_ratio * impulse * _uB;
            vA += _invMassA * PA;
            wA += _invIa * MathUtils.Cross(_rA, PA);
            vB += _invMassB * PB;
            wB += _invIb * MathUtils.Cross(_rB, PB);

            data.Velocities[_indexA].V = vA;
            data.Velocities[_indexA].W = wA;
            data.Velocities[_indexB].V = vB;
            data.Velocities[_indexB].W = wB;
        }

        /// <inheritdoc />
        internal override bool SolvePositionConstraints(in SolverData data)
        {
            var cA = data.Positions[_indexA].Center;
            var aA = data.Positions[_indexA].Angle;
            var cB = data.Positions[_indexB].Center;
            var aB = data.Positions[_indexB].Angle;

            var qA = new Rotation(aA);
            var qB = new Rotation(aB);

            var rA = MathUtils.Mul(qA, _localAnchorA - _localCenterA);
            var rB = MathUtils.Mul(qB, _localAnchorB - _localCenterB);

            // Get the pulley axes.
            var uA = cA + rA - _groundAnchorA;
            var uB = cB + rB - _groundAnchorB;

            var lengthA = uA.magnitude;
            var lengthB = uB.magnitude;

            if (lengthA > 10.0f * Settings.LinearSlop)
            {
                uA *= FP.One / lengthA;
            }
            else
            {
                uA.SetZero();
            }

            if (lengthB > 10.0f * Settings.LinearSlop)
            {
                uB *= FP.One / lengthB;
            }
            else
            {
                uB.SetZero();
            }

            // Compute effective mass.
            var ruA = MathUtils.Cross(rA, uA);
            var ruB = MathUtils.Cross(rB, uB);

            var mA = _invMassA + _invIa * ruA * ruA;
            var mB = _invMassB + _invIb * ruB * ruB;

            var mass = mA + _ratio * _ratio * mB;

            if (mass > FP.Zero)
            {
                mass = FP.One / mass;
            }

            var C = _constant - lengthA - _ratio * lengthB;
            var linearError = FP.Abs(C);

            var impulse = -mass * C;

            var PA = -impulse * uA;
            var PB = -_ratio * impulse * uB;

            cA += _invMassA * PA;
            aA += _invIa * MathUtils.Cross(rA, PA);
            cB += _invMassB * PB;
            aB += _invIb * MathUtils.Cross(rB, PB);

            data.Positions[_indexA].Center = cA;
            data.Positions[_indexA].Angle = aA;
            data.Positions[_indexB].Center = cB;
            data.Positions[_indexB].Angle = aB;

            return linearError < Settings.LinearSlop;
        }

        /// <inheritdoc />
        public override void Dump()
        {
            var indexA = BodyA.IslandIndex;
            var indexB = BodyB.IslandIndex;

            DumpLogger.Log("  b2PulleyJointDef jd;");
            DumpLogger.Log($"  jd.bodyA = bodies[{indexA}];");
            DumpLogger.Log($"  jd.bodyB = bodies[{indexB}];");
            DumpLogger.Log($"  jd.collideConnected = bool({CollideConnected});");
            DumpLogger.Log($"  jd.groundAnchorA.Set({_groundAnchorA.X}, {_groundAnchorA.Y});");
            DumpLogger.Log($"  jd.groundAnchorB.Set({_groundAnchorB.X}, {_groundAnchorB.Y});");
            DumpLogger.Log($"  jd.localAnchorA.Set({_localAnchorA.X}, {_localAnchorA.Y});");
            DumpLogger.Log($"  jd.localAnchorB.Set({_localAnchorB.X}, {_localAnchorB.Y});");
            DumpLogger.Log($"  jd.lengthA = {_lengthA};");
            DumpLogger.Log($"  jd.lengthB = {_lengthB};");
            DumpLogger.Log($"  jd.ratio = {_ratio};");
            DumpLogger.Log($"  joints[{Index}] = m_world.CreateJoint(&jd);");
        }

        /// <inheritdoc />
        public override void ShiftOrigin(in TSVector2 newOrigin)
        {
            _groundAnchorA -= newOrigin;
            _groundAnchorB -= newOrigin;
        }
    }
}