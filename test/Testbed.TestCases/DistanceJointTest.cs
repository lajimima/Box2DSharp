using TrueSync;
using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using Testbed.Abstractions;

namespace Testbed.TestCases
{
    [TestCase("Joints", "Distance Joint")]
    public class DistanceJointTest : TestBase
    {
        public DistanceJoint m_joint;

        public FP m_length;

        public FP m_minLength;

        public FP m_maxLength;

        public FP m_hertz;

        public FP m_dampingRatio;

        public DistanceJointTest()
        {
            Body ground = null;
            {
                BodyDef bd = new BodyDef();
                ground = World.CreateBody(bd);

                EdgeShape shape = new EdgeShape();
                shape.SetTwoSided(new TSVector2(-40.0f, FP.Zero), new TSVector2(40.0f, FP.Zero));
                ground.CreateFixture(shape, FP.Zero);
            }
            {
                BodyDef bd = new BodyDef();
                bd.BodyType = BodyType.DynamicBody;
                bd.AngularDamping = FP.EN1;

                bd.Position.Set(FP.Zero, 5.0f);
                Body body = World.CreateBody(bd);

                PolygonShape shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);
                body.CreateFixture(shape, 5.0f);

                m_hertz = FP.One;
                m_dampingRatio = 0.7f;

                DistanceJointDef jd = new DistanceJointDef();
                jd.Initialize(ground, body, new TSVector2(FP.Zero, 15.0f), bd.Position);
                jd.CollideConnected = true;
                m_length = jd.Length;
                m_minLength = m_length;
                m_maxLength = m_length;
                JointUtils.LinearStiffness(out jd.Stiffness, out jd.Damping, m_hertz, m_dampingRatio, jd.BodyA, jd.BodyB);
                m_joint = (DistanceJoint)World.CreateJoint(jd);
            }
        }

        /// <inheritdoc />
        protected override void OnRender()
        {
            DrawString("This demonstrates a soft distance joint.");
            DrawString("Press: (b) to delete a Body, (j) to delete a joint");
        }
    }
}