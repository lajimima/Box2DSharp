using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Examples", "Web")]
    public class WebTest : TestBase
    {
        public WebTest()
        {
            Body ground;
            {
                BodyDef bd = new BodyDef();
                ground = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }

            {
                PolygonShape shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);

                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;

                bd.Position.Set(-5.0f, 5.0f);
                m_bodies[0] = World.CreateBody(bd);
                m_bodies[0].CreateFixture(shape, 5.0f);

                bd.Position.Set(5.0f, 5.0f);
                m_bodies[1] = World.CreateBody(bd);
                m_bodies[1].CreateFixture(shape, 5.0f);

                bd.Position.Set(5.0f, 15.0f);
                m_bodies[2] = World.CreateBody(bd);
                m_bodies[2].CreateFixture(shape, 5.0f);

                bd.Position.Set(-5.0f, 15.0f);
                m_bodies[3] = World.CreateBody(bd);
                m_bodies[3].CreateFixture(shape, 5.0f);

                DistanceJointDef jd = new DistanceJointDef();
                TSVector2 p1;
                TSVector2 p2;
                TSVector2 d;

                FP frequencyHz = FP.Two;
                FP dampingRatio = FP.Zero;

                jd.BodyA = ground;
                jd.BodyB = m_bodies[0];
                jd.LocalAnchorA.Set(-10.0f, FP.Zero);
                jd.LocalAnchorB.Set(-0.5f, -0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[0] = World.CreateJoint(jd);

                jd.BodyA = ground;
                jd.BodyB = m_bodies[1];
                jd.LocalAnchorA.Set(10.0f, FP.Zero);
                jd.LocalAnchorB.Set(0.5f, -0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[1] = World.CreateJoint(jd);

                jd.BodyA = ground;
                jd.BodyB = m_bodies[2];
                jd.LocalAnchorA.Set(10.0f, 20.0f);
                jd.LocalAnchorB.Set(0.5f, 0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[2] = World.CreateJoint(jd);

                jd.BodyA = ground;
                jd.BodyB = m_bodies[3];
                jd.LocalAnchorA.Set(-10.0f, 20.0f);
                jd.LocalAnchorB.Set(-0.5f, 0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[3] = World.CreateJoint(jd);

                jd.BodyA = m_bodies[0];
                jd.BodyB = m_bodies[1];
                jd.LocalAnchorA.Set(0.5f, FP.Zero);
                jd.LocalAnchorB.Set(-0.5f, FP.Zero);
                ;
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[4] = World.CreateJoint(jd);

                jd.BodyA = m_bodies[1];
                jd.BodyB = m_bodies[2];
                jd.LocalAnchorA.Set(FP.Zero, 0.5f);
                jd.LocalAnchorB.Set(FP.Zero, -0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[5] = World.CreateJoint(jd);

                jd.BodyA = m_bodies[2];
                jd.BodyB = m_bodies[3];
                jd.LocalAnchorA.Set(-0.5f, FP.Zero);
                jd.LocalAnchorB.Set(0.5f, FP.Zero);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[6] = World.CreateJoint(jd);

                jd.BodyA = m_bodies[3];
                jd.BodyB = m_bodies[0];
                jd.LocalAnchorA.Set(FP.Zero, -0.5f);
                jd.LocalAnchorB.Set(FP.Zero, 0.5f);
                p1 = jd.BodyA.GetWorldPoint(jd.LocalAnchorA);
                p2 = jd.BodyB.GetWorldPoint(jd.LocalAnchorB);
                d = p2 - p1;
                jd.Length = d.magnitude;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, frequencyHz, dampingRatio, jd.BodyA, jd.BodyB);
                m_joints[7] = World.CreateJoint(jd);
            }
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            switch (keyInput.Key)
            {
            case KeyCodes.B:
                for (var i = 0; i < 4; ++i)
                {
                    if (m_bodies[i] != null)
                    {
                        World.DestroyBody(m_bodies[i]);
                        m_bodies[i] = null;
                        break;
                    }
                }

                break;

            case KeyCodes.J:
                for (var i = 0; i < 8; ++i)
                {
                    if (m_joints[i] != null)
                    {
                        World.DestroyJoint(m_joints[i]);
                        m_joints[i] = null;
                        break;
                    }
                }

                break;
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            base.OnRender();
            DrawString("Press: (b) to delete a body, (j) to delete a joint");
        }

        public override void JointDestroyed(Joint joint)
        {
            for (var i = 0; i < 8; ++i)
            {
                if (m_joints[i] == joint)
                {
                    m_joints[i] = null;
                    break;
                }
            }
        }

        Body[] m_bodies = new Body[4];

        Joint[] m_joints = new Joint[8];
    }
}