namespace NBody.Scripts;
using Godot;
using System;
using NBody.Objects;

public partial class IntegratorRk4 : Node
{
    private static readonly double[] StageSteps = { 0.0, 0.5, 0.5, 1.0 };
    private static readonly double[] Weights = { 1.0 / 6.0, 1.0 / 3.0, 1.0 / 3.0, 1.0 / 6.0 };

    private readonly int _n;
    private readonly Solver1PN _solver;

    private readonly Vector3D[] _pStage;
    private readonly Vector3D[] _vStage;
    private readonly Vector3D[] _aStage;

    private readonly Vector3D[] _pos;
    private readonly Vector3D[] _vel;


    public IntegratorRk4(Solver1PN s, int n)
    {
        _solver = s;
        _n = n;

        _pStage = new Vector3D[_n];
        _vStage = new Vector3D[_n];
        _aStage = new Vector3D[_n];
        _pos = new Vector3D[_n];
        _vel = new Vector3D[_n];
    }

    public void Step(Vector3D[] p, Vector3D[] v, double dt)
    {
        Array.Clear(_pos, 0, _n);
        Array.Clear(_vel, 0, _n);

        for (int i = 0; i < 4; i++)
        {
            double stepDt = StageSteps[i] * dt;
            double weightDt = Weights[i] * dt;

            if (i == 0)
            {
                Array.Copy(p, _pStage, _n);
                Array.Copy(v, _vStage, _n);
            }
            else
            {
                for (int j = 0; j < _n; j++)
                {
                    _pStage[j] = p[j] + stepDt * _vStage[j];
                    _vStage[j] = v[j] + stepDt * _aStage[j];
                }
            }
            _solver.ComputeAccelerations(_pStage, _vStage, _aStage);
            for (int j = 0; j < _n; j++)
            {
                _pos[j] += weightDt * _vStage[j];
                _vel[j] += weightDt * _aStage[j];
            }
        }
        for (int k = 0; k < _n; k++)
        {
            p[k] += _pos[k];
            v[k] += _vel[k];
        }
    }
}
