﻿using TrueSync;
using ImGuiNET;
using Testbed.TestCases;

namespace Testbed.Tests
{
    [TestInherit]
    public class RevoluteJointTestRender : RevoluteJointTest
    {
        /// <inheritdoc />
        protected override void OnRender()
        {
            ImGui.SetNextWindowPos(new TSVector2(10.0f, 100.0f));
            ImGui.SetNextWindowSize(new TSVector2(200.0f, 100.0f));
            ImGui.Begin("Joint Controls", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);

            if (ImGui.Checkbox("Limit", ref EnableLimit))
            {
                Joint1.EnableLimit(EnableLimit);
            }

            if (ImGui.Checkbox("Motor", ref EnableMotor))
            {
                Joint1.EnableMotor(EnableMotor);
            }

            if (ImGui.SliderFloat("Speed", ref MotorSpeed, -20.0f, 20.0f, "%.0f"))
            {
                Joint1.SetMotorSpeed(MotorSpeed);
            }

            ImGui.End();
        }
    }
}