namespace NBody.Objects;
using Godot;
using System;
using NBody.Scripts;

public partial class Body : Node3D
{
    [ExportGroup("Simulation Settings")]
    [Export] public R_Body Res;
    [Export] public string BodyID;


    [ExportGroup("Visual Settings")]
    [Export] public Color TrailColor = new(0.4f, 0.8f, 1.0f, 0.8f);
    /// <summary>Length of the orbital trail in number of points.</summary>
    [Export] public int TrailLength = 400;


    public double Mass { get; set; }
    public Vector3D InitPos { get; set; }
    public Vector3D InitVel { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        //if (BodyID == "Earth") GD.Print($"{BodyID} Position: {Position}");
    }
}
