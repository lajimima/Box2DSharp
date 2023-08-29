using FixedBox2D.Collision.Shapes;
using FixedBox2D.Common;
using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Joints;
using System;
using System.IO;
using Testbed.Abstractions;
using TrueSync;

namespace Testbed.TestCases
{
    [TestCase("Examples", "BodyTypes")]
    public class BodyTypes : TestBase
    {
        const int PixelWidth = 1920;
        const int PixelHeight = 1080;
        const int Pixel2Meter = 32;
        const int MeterWidth = PixelWidth / Pixel2Meter * 3;
        ET.BattleMapConfig config = null;
        public BodyTypes()
        {
            var content = File.ReadAllText("BattleMapConfig.json");
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<ET.BattleMapConfig>(content);

            Body ground;
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.StaticBody;
                bd.Position = new TSVector2(FP.Zero, -FP.One);
                bd.Bullet = true;
                ground = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(MeterWidth, FP.Two);

                //var shape = new EdgeShape();
                //shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));

                var fd = new FixtureDef();
                fd.Friction = FP.One;
                fd.Shape = shape;

                ground.CreateFixture(fd);
            }

            CreateBox();
            //CreateBox2();

            //CreateBox31();
            //CreateBox32();

            CreateBox41();
        }


        private void CreateBox()
        {
            Body box;
            {
                var bd = new BodyDef();
                bd.Position = new TSVector2(FP.One, 5);
                bd.BodyType = BodyType.DynamicBody;
                bd.FixedRotation = true;
                box = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, FP.One);

                var fd = new FixtureDef();
                fd.Density = 1;
                fd.Friction = FP.One;
                fd.Shape = shape;

                box.CreateFixture(fd);
            }
        }

        private void CreateBox2()
        {
            Body ground;
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.StaticBody;
                bd.Position = new TSVector2(-FP.One, FP.Zero);
                ground = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.Two, 50);

                //var shape = new EdgeShape();
                //shape.SetTwoSided(new TSVector2(-20.0f, FP.Zero), new TSVector2(20.0f, FP.Zero));

                var fd = new FixtureDef();
                fd.Friction = FP.One;
                fd.Shape = shape;

                ground.CreateFixture(fd);
            }

        }

        private void CreateBox31()
        {
            Body ground;
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.StaticBody;
                bd.Position = new TSVector2(FP.One, FP.Zero);
                ground = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, 30);

                var fd = new FixtureDef();
                fd.Friction = FP.One;
                fd.Shape = shape;

                ground.CreateFixture(fd);
            }
        }

        private void CreateBox32()
        {
            Body ground;
            {
                var bd = new BodyDef();
                bd.BodyType = BodyType.StaticBody;
                bd.Position = new TSVector2(FP.Three, FP.Zero);
                ground = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(FP.One, 30);

                var fd = new FixtureDef();
                fd.Friction = FP.One;
                fd.Shape = shape;

                ground.CreateFixture(fd);
            }
        }

        Body DownGround = null;
        private void CreateBox41()
        {
            Body box;
            {
                var bd = new BodyDef();
                bd.Position = new TSVector2(10, 10);
                bd.BodyType = BodyType.DynamicBody;
                bd.FixedRotation = true;
                box = World.CreateBody(bd);

                var shape = new PolygonShape();
                shape.SetAsBox(10, FP.One);

                var fd = new FixtureDef();
                fd.Density = 0;
                fd.Friction = FP.One;
                fd.Shape = shape;

                box.CreateFixture(fd);
            }
            DownGround = box;
        }

        protected override void OnRender()
        {
            DrawString("Keys: (d) dynamic, (s) static, (k) kinematic");
        }

        protected override void PreStep()
        {
        }

        /// <inheritdoc />
        public override void OnKeyDown(KeyInputEventArgs keyInput)
        {
            switch (keyInput.Key)
            {
                case KeyCodes.D:
                    Console.WriteLine(World.IsLocked);
                    Console.WriteLine(DownGround.IsSleepingAllowed);

                    DownGround.FixtureList[0].Density = 1;
                    DownGround.FixtureList[0].GetMassData(out var massData);
                    DownGround.SetMassData(massData);
                    //DownGround.ResetMassData();
                    DownGround.IsSleepingAllowed = false;
                    break;
            }
        }
    }
}