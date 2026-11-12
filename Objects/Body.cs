namespace NBody.Objects;
using Godot;
using System;
using NBody.Scripts;

public partial class Body : Node3D
{
    [Export] public R_Body Res;
    [Export] public string BodyID;
    public double Mass { get; set; }
    public Vector3D InitPos { get; set; }
    public Vector3D InitVel { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        //if (BodyID == "Earth") GD.Print($"{BodyID} Position: {Position}");
    }
}
