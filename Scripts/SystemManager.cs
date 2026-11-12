namespace NBody.Scripts;
using Godot;
using System;
using NBody.Objects;
using System.IO;

public partial class SystemManager : Node3D
{
    [ExportGroup("Simulation Settings")]
    [Export] public int SubStepsPerFrame = 100;
    [Export] public double TimeStep = 0.25;
    [Export] public double VisualScale = 10.0;
    [Export] public bool IsPaused = false;
    [Export(PropertyHint.File, "*.json")] public string StarSystemData { get; set; }

    [ExportGroup("Scene References")]
    [Export] public Body[] _bodies;
    private Vector3D[] _positions;
    private Vector3D[] _velocities;

    private Solver1PN _solver;
    private IntegratorRk4 _integrator;

    public double DaysElapsed { get; set; } = 0.0;
    public double YearsElapsed => DaysElapsed / 365.25;


    public override void _Ready()
    {
        int n = _bodies.Length;
        _positions = new Vector3D[n];
        _velocities = new Vector3D[n];

        InitBodies();

        for (int i = 0; i < n; i++)
        {
            _positions[i] = _bodies[i].InitPos;
            _velocities[i] = _bodies[i].InitVel;
        }

        _solver = new Solver1PN(_bodies);
        _integrator = new IntegratorRk4(_solver, n);

        SyncVisuals();
    }


    public void InitBodies()
    {
        JSONLoader.PopulateSystem(StarSystemData, _bodies);
    }


    public override void _PhysicsProcess(double delta)
    {
        if (IsPaused) return;

        for (int step = 0; step < SubStepsPerFrame; step++)
        {
            _integrator.Step(_positions, _velocities, TimeStep);
            DaysElapsed += TimeStep;
        }

        SyncVisuals();
    }


    private void SyncVisuals()
    {
        if (_bodies == null) return;
        for (int i = 0; i < _bodies.Length; i++)
        {
            if (_bodies[i] != null) _bodies[i].Position = _positions[i].ToVector3(VisualScale);
        }
    }
}
