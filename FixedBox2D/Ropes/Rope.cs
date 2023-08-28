using System;
using System.Diagnostics;
using TrueSync;
using FixedBox2D.Common;

namespace FixedBox2D.Ropes
{
    public class Rope
    {
        private RopeBend[] _bendConstraints;

        private int _bendCount;

        private TSVector2[] _bindPositions;

        private int _count;

        private TSVector2 _gravity;

        private FP[] _invMasses;

        private TSVector2[] _p0s;

        private TSVector2 _position;

        private TSVector2[] _ps;

        private RopeStretch[] _stretchConstraints;

        private int _stretchCount;

        private RopeTuning _tuning;

        private TSVector2[] _vs;

        public void Create(in RopeDef def)
        {
            Debug.Assert(def.Count >= 3);
            _position = def.Position;
            _count = def.Count;
            _bindPositions = new TSVector2[_count];
            _ps = new TSVector2[_count];
            _p0s = new TSVector2[_count];
            _vs = new TSVector2[_count];
            _invMasses = new FP[_count];

            for (var i = 0; i < _count; ++i)
            {
                _bindPositions[i] = def.Vertices[i];
                _ps[i] = def.Vertices[i] + _position;
                _p0s[i] = def.Vertices[i] + _position;
                _vs[i].SetZero();

                var m = def.Masses[i];
                if (m > FP.Zero)
                {
                    _invMasses[i] = FP.One / m;
                }
                else
                {
                    _invMasses[i] = FP.Zero;
                }
            }

            _stretchCount = _count - 1;
            _bendCount = _count - 2;

            _stretchConstraints = new RopeStretch[_stretchCount];
            _bendConstraints = new RopeBend[_bendCount];

            for (var i = 0; i < _stretchCount; ++i)
            {
                ref var c = ref _stretchConstraints[i];
                var p1 = _ps[i];
                var p2 = _ps[i + 1];

                c.I1 = i;
                c.I2 = i + 1;
                c.L = TSVector2.Distance(p1, p2);
                c.InvMass1 = _invMasses[i];
                c.InvMass2 = _invMasses[i + 1];
                c.Lambda = FP.Zero;
                c.Damper = FP.Zero;
                c.Spring = FP.Zero;
            }

            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var p1 = _ps[i];
                var p2 = _ps[i + 1];
                var p3 = _ps[i + 2];

                c.i1 = i;
                c.i2 = i + 1;
                c.i3 = i + 2;
                c.invMass1 = _invMasses[i];
                c.invMass2 = _invMasses[i + 1];
                c.invMass3 = _invMasses[i + 2];
                c.invEffectiveMass = FP.Zero;
                c.L1 = TSVector2.Distance(p1, p2);
                c.L2 = TSVector2.Distance(p2, p3);
                c.lambda = FP.Zero;

                // Pre-compute effective mass (TODO use flattened config)
                var e1 = p2 - p1;
                var e2 = p3 - p2;
                var l1Sqr = e1.LengthSquared();
                var l2Sqr = e2.LengthSquared();

                if ((l1Sqr * l2Sqr).Equals(0))
                {
                    continue;
                }

                var jd1 = -FP.One / l1Sqr * e1.Skew();
                var jd2 = FP.One / l2Sqr * e2.Skew();

                var j1 = -jd1;
                var j2 = jd1 - jd2;
                var j3 = jd2;

                c.invEffectiveMass = c.invMass1 * TSVector2.Dot(j1, j1) + c.invMass2 * TSVector2.Dot(j2, j2) + c.invMass3 * TSVector2.Dot(j3, j3);

                var r = p3 - p1;

                var rr = r.LengthSquared();
                if (rr.Equals(0))
                {
                    continue;
                }

                // a1 = h2 / (h1 + h2)
                // a2 = h1 / (h1 + h2)
                c.alpha1 = TSVector2.Dot(e2, r) / rr;
                c.alpha2 = TSVector2.Dot(e1, r) / rr;
            }

            _gravity = def.Gravity;

            SetTuning(def.Tuning);
        }

        public void SetTuning(RopeTuning tuning)
        {
            _tuning = tuning;

            // Pre-compute spring and damper values based on tuning

            var bendOmega = FP.Two * Settings.Pi * _tuning.BendHertz;

            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var l1Sqr = c.L1 * c.L1;
                var l2Sqr = c.L2 * c.L2;

                if ((l1Sqr * l2Sqr).Equals(0))
                {
                    c.spring = FP.Zero;
                    c.damper = FP.Zero;
                    continue;
                }

                // Flatten the triangle formed by the two edges
                var j2 = FP.One / c.L1 + FP.One / c.L2;
                var sum = c.invMass1 / l1Sqr + c.invMass2 * j2 * j2 + c.invMass3 / l2Sqr;
                if (sum.Equals(0))
                {
                    c.spring = FP.Zero;
                    c.damper = FP.Zero;
                    continue;
                }

                var mass = FP.One / sum;

                c.spring = mass * bendOmega * bendOmega;
                c.damper = FP.Two * mass * _tuning.BendDamping * bendOmega;
            }

            var stretchOmega = FP.Two * Settings.Pi * _tuning.StretchHertz;

            for (var i = 0; i < _stretchCount; ++i)
            {
                ref var c = ref _stretchConstraints[i];

                var sum = c.InvMass1 + c.InvMass2;
                if (sum.Equals(0))
                {
                    continue;
                }

                var mass = FP.One / sum;

                c.Spring = mass * stretchOmega * stretchOmega;
                c.Damper = FP.Two * mass * _tuning.StretchDamping * stretchOmega;
            }
        }

        public void Step(FP dt, int iterations, TSVector2 position)
        {
            if (dt.Equals(0))
            {
                return;
            }

            var invDt = FP.One / dt;

            var d = TSMath.Exp(-dt * _tuning.Damping);

            // Apply gravity and damping
            for (var i = 0; i < _count; ++i)
            {
                if (_invMasses[i] > FP.Zero)
                {
                    _vs[i] *= d;
                    _vs[i] += dt * _gravity;
                }
                else
                {
                    _vs[i] = invDt * (_bindPositions[i] + position - _p0s[i]);
                }
            }

            // Apply bending spring
            if (_tuning.BendingModel == BendingModel.SpringAngleBendingModel)
            {
                ApplyBendForces(dt);
            }

            for (var i = 0; i < _bendCount; ++i)
            {
                _bendConstraints[i].lambda = FP.Zero;
            }

            for (var i = 0; i < _stretchCount; ++i)
            {
                _stretchConstraints[i].Lambda = FP.Zero;
            }

            // Update position
            for (var i = 0; i < _count; ++i)
            {
                _ps[i] += dt * _vs[i];
            }

            // Solve constraints
            for (var i = 0; i < iterations; ++i)
            {
                switch (_tuning.BendingModel)
                {
                case BendingModel.PbdAngleBendingModel:
                    SolveBend_PBD_Angle();
                    break;
                case BendingModel.XpdAngleBendingModel:
                    SolveBend_XPBD_Angle(dt);
                    break;
                case BendingModel.PbdDistanceBendingModel:
                    SolveBend_PBD_Distance();
                    break;
                case BendingModel.PbdHeightBendingModel:
                    SolveBend_PBD_Height();
                    break;
                case BendingModel.PbdTriangleBendingModel:
                    SolveBend_PBD_Triangle();
                    break;
                }

                switch (_tuning.StretchingModel)
                {
                case StretchingModel.PbdStretchingModel:
                    SolveStretch_PBD();
                    break;
                case StretchingModel.XpbdStretchingModel:
                    SolveStretch_XPBD(dt);
                    break;
                }
            }

            // Constrain velocity
            for (var i = 0; i < _count; ++i)
            {
                _vs[i] = invDt * (_ps[i] - _p0s[i]);
                _p0s[i] = _ps[i];
            }
        }

        public void Reset(TSVector2 position)
        {
            _position = position;

            for (var i = 0; i < _count; ++i)
            {
                _ps[i] = _bindPositions[i] + _position;
                _p0s[i] = _bindPositions[i] + _position;
                _vs[i].SetZero();
            }

            for (var i = 0; i < _bendCount; ++i)
            {
                _bendConstraints[i].lambda = FP.Zero;
            }

            for (var i = 0; i < _stretchCount; ++i)
            {
                _stretchConstraints[i].Lambda = FP.Zero;
            }
        }

        private void SolveStretch_PBD()
        {
            var stiffness = _tuning.StretchStiffness;
            for (var i = 0; i < _stretchCount; ++i)
            {
                ref var c = ref _stretchConstraints[i];

                var p1 = _ps[c.I1];
                var p2 = _ps[c.I2];

                var d = p2 - p1;
                var l = MathExtensions.Normalize(d);

                var sum = c.InvMass1 + c.InvMass2;
                if (sum.Equals(0))
                {
                    continue;
                }

                var s1 = c.InvMass1 / sum;
                var s2 = c.InvMass2 / sum;

                p1 -= stiffness * s1 * (c.L - l) * d;
                p2 += stiffness * s2 * (c.L - l) * d;

                _ps[c.I1] = p1;
                _ps[c.I2] = p2;
            }
        }

        private void SolveStretch_XPBD(FP dt)
        {
            Debug.Assert(dt > FP.Zero);
            for (var i = 0; i < _stretchCount; ++i)
            {
                ref var ropeStretch = ref _stretchConstraints[i];

                var p1 = _ps[ropeStretch.I1];
                var p2 = _ps[ropeStretch.I2];

                var dp1 = p1 - _p0s[ropeStretch.I1];
                var dp2 = p2 - _p0s[ropeStretch.I2];

                var u = p2 - p1;
                var l = MathExtensions.Normalize(u);

                var j1 = -u;
                var j2 = u;

                var sum = ropeStretch.InvMass1 + ropeStretch.InvMass2;
                if (sum.Equals(0))
                {
                    continue;
                }

                var alpha = FP.One / (ropeStretch.Spring * dt * dt); // 1 / kg
                var beta = dt * dt * ropeStretch.Damper;           // kg * s
                var sigma = alpha * beta / dt;                     // non-dimensional
                var stretchL = l - ropeStretch.L;

                // This is using the initial velocities
                var cDot = TSVector2.Dot(j1, dp1) + TSVector2.Dot(j2, dp2);

                var b = stretchL + alpha * ropeStretch.Lambda + sigma * cDot;
                var sum2 = (FP.One + sigma) * sum + alpha;

                var impulse = -b / sum2;

                p1 += ropeStretch.InvMass1 * impulse * j1;
                p2 += ropeStretch.InvMass2 * impulse * j2;

                _ps[ropeStretch.I1] = p1;
                _ps[ropeStretch.I2] = p2;
                ropeStretch.Lambda += impulse;
            }
        }

        private void SolveBend_PBD_Angle()
        {
            var stiffness = _tuning.BendStiffness;
            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var p1 = _ps[c.i1];
                var p2 = _ps[c.i2];
                var p3 = _ps[c.i3];

                var d1 = p2 - p1;
                var d2 = p3 - p2;
                var a = MathUtils.Cross(d1, d2);
                var b = TSVector2.Dot(d1, d2);

                var angle = FP.Atan2(a, b);

                FP L1sqr, L2sqr;

                if (_tuning.Isometric)
                {
                    L1sqr = c.L1 * c.L1;
                    L2sqr = c.L2 * c.L2;
                }
                else
                {
                    L1sqr = d1.LengthSquared();
                    L2sqr = d2.LengthSquared();
                }

                if ((L1sqr * L2sqr).Equals(0))
                {
                    continue;
                }

                var Jd1 = -FP.One / L1sqr * d1.Skew();
                var Jd2 = FP.One / L2sqr * d2.Skew();

                var J1 = -Jd1;
                var J2 = Jd1 - Jd2;
                var J3 = Jd2;

                FP sum;
                if (_tuning.FixedEffectiveMass)
                {
                    sum = c.invEffectiveMass;
                }
                else
                {
                    sum = c.invMass1 * TSVector2.Dot(J1, J1) + c.invMass2 * TSVector2.Dot(J2, J2) + c.invMass3 * TSVector2.Dot(J3, J3);
                }

                if (sum.Equals(0))
                {
                    sum = c.invEffectiveMass;
                }

                var impulse = -stiffness * angle / sum;

                p1 += c.invMass1 * impulse * J1;
                p2 += c.invMass2 * impulse * J2;
                p3 += c.invMass3 * impulse * J3;

                _ps[c.i1] = p1;
                _ps[c.i2] = p2;
                _ps[c.i3] = p3;
            }
        }

        private void SolveBend_XPBD_Angle(FP dt)
        {
            Debug.Assert(dt > FP.Zero);
            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var p1 = _ps[c.i1];
                var p2 = _ps[c.i2];
                var p3 = _ps[c.i3];

                var dp1 = p1 - _p0s[c.i1];
                var dp2 = p2 - _p0s[c.i2];
                var dp3 = p3 - _p0s[c.i3];

                var d1 = p2 - p1;
                var d2 = p3 - p2;

                FP L1sqr, L2sqr;

                if (_tuning.Isometric)
                {
                    L1sqr = c.L1 * c.L1;
                    L2sqr = c.L2 * c.L2;
                }
                else
                {
                    L1sqr = d1.LengthSquared();
                    L2sqr = d2.LengthSquared();
                }

                if ((L1sqr * L2sqr).Equals(0))
                {
                    continue;
                }

                var a = MathUtils.Cross(d1, d2);
                var b = TSVector2.Dot(d1, d2);

                var angle = FP.Atan2(a, b);

                var Jd1 = -FP.One / L1sqr * d1.Skew();
                var Jd2 = FP.One / L2sqr * d2.Skew();

                var J1 = -Jd1;
                var J2 = Jd1 - Jd2;
                var J3 = Jd2;

                FP sum;
                if (_tuning.FixedEffectiveMass)
                {
                    sum = c.invEffectiveMass;
                }
                else
                {
                    sum = c.invMass1 * TSVector2.Dot(J1, J1) + c.invMass2 * TSVector2.Dot(J2, J2) + c.invMass3 * TSVector2.Dot(J3, J3);
                }

                if (sum.Equals(0))
                {
                    continue;
                }

                var alpha = FP.One / (c.spring * dt * dt);
                var beta = dt * dt * c.damper;
                var sigma = alpha * beta / dt;
                var C = angle;

                // This is using the initial velocities
                var Cdot = TSVector2.Dot(J1, dp1) + TSVector2.Dot(J2, dp2) + TSVector2.Dot(J3, dp3);

                var B = C + alpha * c.lambda + sigma * Cdot;
                var sum2 = (FP.One + sigma) * sum + alpha;

                var impulse = -B / sum2;

                p1 += c.invMass1 * impulse * J1;
                p2 += c.invMass2 * impulse * J2;
                p3 += c.invMass3 * impulse * J3;

                _ps[c.i1] = p1;
                _ps[c.i2] = p2;
                _ps[c.i3] = p3;
                c.lambda += impulse;
            }
        }

        private void ApplyBendForces(FP dt)
        {
            // omega = 2 * pi * hz
            var omega = FP.Two * Settings.Pi * _tuning.BendHertz;
            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var p1 = _ps[c.i1];
                var p2 = _ps[c.i2];
                var p3 = _ps[c.i3];

                var v1 = _vs[c.i1];
                var v2 = _vs[c.i2];
                var v3 = _vs[c.i3];

                var d1 = p2 - p1;
                var d2 = p3 - p2;

                FP L1sqr, L2sqr;

                if (_tuning.Isometric)
                {
                    L1sqr = c.L1 * c.L1;
                    L2sqr = c.L2 * c.L2;
                }
                else
                {
                    L1sqr = d1.LengthSquared();
                    L2sqr = d2.LengthSquared();
                }

                if ((L1sqr * L2sqr).Equals(0))
                {
                    continue;
                }

                var a = MathUtils.Cross(d1, d2);
                var b = TSVector2.Dot(d1, d2);

                var angle = FP.Atan2(a, b);

                var Jd1 = -FP.One / L1sqr * d1.Skew();
                var Jd2 = FP.One / L2sqr * d2.Skew();

                var J1 = -Jd1;
                var J2 = Jd1 - Jd2;
                var J3 = Jd2;

                FP sum;
                if (_tuning.FixedEffectiveMass)
                {
                    sum = c.invEffectiveMass;
                }
                else
                {
                    sum = c.invMass1 * TSVector2.Dot(J1, J1) + c.invMass2 * TSVector2.Dot(J2, J2) + c.invMass3 * TSVector2.Dot(J3, J3);
                }

                if (sum.Equals(0))
                {
                    continue;
                }

                var mass = FP.One / sum;

                var spring = mass * omega * omega;
                var damper = FP.Two * mass * _tuning.BendDamping * omega;

                var C = angle;
                var Cdot = TSVector2.Dot(J1, v1) + TSVector2.Dot(J2, v2) + TSVector2.Dot(J3, v3);

                var impulse = -dt * (spring * C + damper * Cdot);

                _vs[c.i1] += c.invMass1 * impulse * J1;
                _vs[c.i2] += c.invMass2 * impulse * J2;
                _vs[c.i3] += c.invMass3 * impulse * J3;
            }
        }

        private void SolveBend_PBD_Distance()
        {
            var stiffness = _tuning.BendStiffness;
            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var i1 = c.i1;
                var i2 = c.i3;

                var p1 = _ps[i1];
                var p2 = _ps[i2];

                var d = p2 - p1;
                var L = MathExtensions.Normalize(d);

                var sum = c.invMass1 + c.invMass3;
                if (sum.Equals(0))
                {
                    continue;
                }

                var s1 = c.invMass1 / sum;
                var s2 = c.invMass3 / sum;

                p1 -= stiffness * s1 * (c.L1 + c.L2 - L) * d;
                p2 += stiffness * s2 * (c.L1 + c.L2 - L) * d;

                _ps[i1] = p1;
                _ps[i2] = p2;
            }
        }

        // Constraint based implementation of:
        // P. Volino: Simple Linear Bending Stiffness in Particle Systems
        private void SolveBend_PBD_Height()
        {
            var stiffness = _tuning.BendStiffness;
            for (var i = 0; i < _bendCount; ++i)
            {
                ref var c = ref _bendConstraints[i];

                var p1 = _ps[c.i1];
                var p2 = _ps[c.i2];
                var p3 = _ps[c.i3];

                // Barycentric coordinates are held constant
                var d = c.alpha1 * p1 + c.alpha2 * p3 - p2;
                var dLen = d.magnitude;

                if (dLen.Equals(0))
                {
                    continue;
                }

                var dHat = FP.One / dLen * d;

                var J1 = c.alpha1 * dHat;
                var J2 = -dHat;
                var J3 = c.alpha2 * dHat;

                var sum = c.invMass1 * c.alpha1 * c.alpha1 + c.invMass2 + c.invMass3 * c.alpha2 * c.alpha2;

                if (sum.Equals(0))
                {
                    continue;
                }

                var C = dLen;
                var mass = FP.One / sum;
                var impulse = -stiffness * mass * C;

                p1 += c.invMass1 * impulse * J1;
                p2 += c.invMass2 * impulse * J2;
                p3 += c.invMass3 * impulse * J3;

                _ps[c.i1] = p1;
                _ps[c.i2] = p2;
                _ps[c.i3] = p3;
            }
        }

        // M. Kelager: A Triangle Bending Constraint Model for PBD
        private void SolveBend_PBD_Triangle()
        {
            var stiffness = _tuning.BendStiffness;

            for (var i = 0; i < _bendCount; ++i)
            {
                var c = _bendConstraints[i];

                var b0 = _ps[c.i1];
                var v = _ps[c.i2];
                var b1 = _ps[c.i3];

                var wb0 = c.invMass1;
                var wv = c.invMass2;
                var wb1 = c.invMass3;

                var W = wb0 + wb1 + FP.Two * wv;
                var invW = stiffness / W;

                var d = v - (FP.One / 3.0f) * (b0 + v + b1);

                var db0 = FP.Two * wb0 * invW * d;
                var dv = -4.0f * wv * invW * d;
                var db1 = FP.Two * wb1 * invW * d;

                b0 += db0;
                v += dv;
                b1 += db1;

                _ps[c.i1] = b0;
                _ps[c.i2] = v;
                _ps[c.i3] = b1;
            }
        }

        public void Draw(IDrawer draw)
        {
            var c = Color.FromArgb(0.4f, FP.Half, 0.7f);

            var pg = Color.FromArgb(0.1f, 0.8f, 0.1f);

            var pd = Color.FromArgb(0.7f, 0.2f, 0.4f);
            for (var i = 0; i < _count - 1; ++i)
            {
                draw.DrawSegment(_ps[i], _ps[i + 1], c);

                var pc = _invMasses[i] > FP.Zero ? pd : pg;
                draw.DrawPoint(_ps[i], 5.0f, pc);
            }

            {
                var pc = _invMasses[_count - 1] > FP.Zero ? pd : pg;
                draw.DrawPoint(_ps[_count - 1], 5.0f, pc);
            }
        }
    }
}