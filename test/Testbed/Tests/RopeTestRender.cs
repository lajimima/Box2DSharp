using System.Numerics;
using FixedBox2D.Common;
using FixedBox2D.Ropes;
using ImGuiNET;
using Testbed.TestCases;

namespace Testbed.Tests
{
    [TestInherit]
    public class RopeTestRender : RopeTest
    {
        /// <inheritdoc />
        protected override void OnRender()
        {
            ImGui.SetNextWindowPos(new Vector2(10.0f, 100.0f));
            ImGui.SetNextWindowSize(new Vector2(200.0f, 700.0f));
            ImGui.Begin("Tuning", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

            ImGui.Separator();

            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);

            const ImGuiComboFlags comboFlags = 0;
            string[] bendModels = {"Spring", "PBD Ang", "XPBD Ang", "PBD Dist", "PBD Height", "PBD Triangle"};
            string[] stretchModels = {"PBD", "XPBD"};

            ImGui.Text("Rope 1");

            var bendModel1 = (int)Tuning1.BendingModel;
            if (ImGui.BeginCombo("Bend Model##1", bendModels[bendModel1], comboFlags))
            {
                for (var i = 0; i < bendModels.Length; ++i)
                {
                    var isSelected = bendModel1 == i;
                    if (ImGui.Selectable(bendModels[i], isSelected))
                    {
                        bendModel1 = i;
                        Tuning1.BendingModel = (BendingModel)i;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            float BendDamping = Tuning1.BendDamping.AsFloat();
            ImGui.SliderFloat("Damping##B1", ref BendDamping, 0.0f, 4.0f, "%.1f");
            Tuning1.BendDamping = BendDamping;

            float BendHertz = Tuning1.BendHertz.AsFloat();
            ImGui.SliderFloat("Hertz##B1", ref BendHertz, 0.0f, 60.0f, "%.0f");
            Tuning1.BendHertz = BendHertz;

            float BendStiffness = Tuning1.BendStiffness.AsFloat();
            ImGui.SliderFloat("Stiffness##B1", ref BendStiffness, 0.0f, 1.0f, "%.1f");
            Tuning1.BendStiffness = BendStiffness;

            ImGui.Checkbox("Isometric##1", ref Tuning1.Isometric);
            ImGui.Checkbox("Fixed Mass##1", ref Tuning1.FixedEffectiveMass);
            ImGui.Checkbox("Warm Start##1", ref Tuning1.WarmStart);

            var stretchModel1 = (int)Tuning1.StretchingModel;
            if (ImGui.BeginCombo("Stretch Model##1", stretchModels[stretchModel1], comboFlags))
            {
                for (var i = 0; i < stretchModels.Length; ++i)
                {
                    var isSelected = stretchModel1 == i;
                    if (ImGui.Selectable(stretchModels[i], isSelected))
                    {
                        stretchModel1 = i;
                        Tuning1.StretchingModel = (StretchingModel)i;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            float StretchDamping = Tuning1.StretchDamping.AsFloat();
            ImGui.SliderFloat("Damping##S1", ref StretchDamping, 0.0f, 4.0f, "%.1f");
            Tuning1.StretchDamping = StretchDamping;

            float StretchHertz = Tuning1.StretchHertz.AsFloat();
            ImGui.SliderFloat("Hertz##S1", ref StretchHertz, 0.0f, 60.0f, "%.0f");
            Tuning1.StretchHertz = StretchHertz;

            float StretchStiffness = Tuning1.StretchStiffness.AsFloat();
            ImGui.SliderFloat("Stiffness##S1", ref StretchStiffness, 0.0f, 1.0f, "%.1f");
            Tuning1.StretchStiffness = StretchStiffness;

            ImGui.SliderInt("Iterations##1", ref Iterations1, 1, 100, "%d");

            ImGui.Separator();

            ImGui.Text("Rope 2");

            var bendModel2 = (int)Tuning2.BendingModel;
            if (ImGui.BeginCombo("Bend Model##2", bendModels[bendModel2], comboFlags))
            {
                for (var i = 0; i < bendModels.Length; ++i)
                {
                    var isSelected = bendModel2 == i;
                    if (ImGui.Selectable(bendModels[i], isSelected))
                    {
                        bendModel2 = i;
                        Tuning2.BendingModel = (BendingModel)i;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            float BendDamping2 = Tuning2.BendDamping.AsFloat();
            ImGui.SliderFloat("Damping##", ref BendDamping2, 0.0f, 4.0f, "%.1f");
            Tuning2.BendDamping = BendDamping2;

            float BendHertz2 = Tuning2.BendHertz.AsFloat();
            ImGui.SliderFloat("Hertz##", ref BendHertz2, 0.0f, 60.0f, "%.0f");
            Tuning2.BendHertz = BendHertz2;

            float BendStiffness2 = Tuning2.BendStiffness.AsFloat();
            ImGui.SliderFloat("Stiffness##", ref BendStiffness2, 0.0f, 1.0f, "%.1f");
            Tuning2.BendStiffness = BendStiffness2;

            ImGui.Checkbox("Isometric##2", ref Tuning2.Isometric);
            ImGui.Checkbox("Fixed Mass##2", ref Tuning2.FixedEffectiveMass);
            ImGui.Checkbox("Warm Start##2", ref Tuning2.WarmStart);

            var stretchModel2 = (int)Tuning2.StretchingModel;
            if (ImGui.BeginCombo("Stretch Model##2", stretchModels[stretchModel2], comboFlags))
            {
                for (var i = 0; i < stretchModels.Length; ++i)
                {
                    var isSelected = stretchModel2 == i;
                    if (ImGui.Selectable(stretchModels[i], isSelected))
                    {
                        stretchModel2 = i;
                        Tuning2.StretchingModel = (StretchingModel)i;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            float StretchDamping2 = Tuning2.StretchDamping.AsFloat();
            ImGui.SliderFloat("Damping##S2", ref StretchDamping2, 0.0f, 4.0f, "%.1f");
            Tuning2.StretchDamping = StretchDamping2;

            float StretchHertz2 = Tuning2.StretchHertz.AsFloat();
            ImGui.SliderFloat("Hertz##S2", ref StretchHertz2, 0.0f, 60.0f, "%.0f");
            Tuning2.StretchHertz = StretchHertz2;

            float StretchStiffness2 = Tuning2.StretchStiffness.AsFloat();
            ImGui.SliderFloat("Stiffness##S2", ref StretchStiffness2, 0.0f, 1.0f, "%.1f");
            Tuning2.StretchStiffness = StretchStiffness2;

            ImGui.SliderInt("Iterations##2", ref Iterations2, 1, 100, "%d");

            ImGui.Separator();

            ImGui.SliderFloat("Speed", ref Speed, 10.0f, 100.0f, "%.0f");

            if (ImGui.Button("Reset"))
            {
                Position1.Set(-5.0f, 15.0f);
                Position2.Set(5.0f, 15.0f);
                Rope1.Reset(Position1);
                Rope2.Reset(Position2);
            }

            ImGui.PopItemWidth();

            ImGui.End();

            Rope1.Draw(Drawer);
            Rope2.Draw(Drawer);

            DrawString("Press comma and period to move left and right");
            base.OnRender();
        }
    }
}