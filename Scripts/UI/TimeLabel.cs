namespace NBody.Scripts.UI;
using Godot;
using System;

public partial class TimeLabel : RichTextLabel
{
    [Export] public SystemManager Sim;

    public override void _PhysicsProcess(double delta)
    {
        Text = $"Time Elapsed: {(int)Sim.YearsElapsed} years, {(int)(Sim.DaysElapsed % 365.25)} days";
    }
}
