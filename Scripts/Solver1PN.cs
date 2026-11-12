namespace NBody.Scripts;
using System;
using NBody.Objects;

// Physics solver to apply first-order post-Newtonian expansion (1PN) using Einstein-Infeld-Hoffman (EIH) Equations.
public partial class Solver1PN
{
    private const double G = 2.959122082855911e-4; // AU^3 / (M_sun * day^2)
    private const double C = 173.14463267424034;   // AU / day
    private const double INVC2 = 1.0 / (C * C);    // Inverse of C^2

    private readonly int _n;            
    private readonly double[] _phi;     // Scalar Newtonian gravitational potential per body.
    private readonly Vector3D[] _aNewt; // Pure Newtonian acceleration. Will be used to reduce computational complexity.

    /// <summary>
    /// Array of bodies to be considered in the gravity simulation.
    /// </summary>
    public Body[] Bodies;

    /// <summary>
    /// Constructor. Initializes solver and pre-allocates buffers for force calculation output.
    /// </summary>
    /// <param name="bodies">Array of bodies to be considered in the gravity simulation.</param>
    public Solver1PN(Body[] bodies)
    {
        Bodies = bodies;
        _n = Bodies.Length;
        _phi = new double[_n];
        _aNewt = new Vector3D[_n];
    }

    /// <summary>
    /// Computes Newtonian accelerations and 1PN corrections for all simulated bodies
    /// at an instant in coordinate time. O(N^2) time.
    /// </summary>
    /// <param name="p">Current 64-bit barycentric positions (AU) for all considered bodies.</param>
    /// <param name="v">Current 64-bit barycentric velocities (AU/day) for all considered bodies.</param>
    /// <param name="outAccel">Pre-allocated output array for total accelerations.</param>
    public void ComputeAccelerations(Vector3D[] p, Vector3D[] v, Vector3D[] outAccel) //(position, velocity, output acceleration container)
    {
        Array.Clear(_phi, 0, _n);
        Array.Clear(_aNewt, 0, _n);

        // Initial pass to compute raw Newtonian acceleration and potentials
        for (int a = 0; a < _n; a++)
        {
            for (int b = a + 1; b < _n; b++)
            {
                // Displacement vector from body B to body A
                Vector3D rAB = p[a] - p[b];

                // Reciprocal distance and inverse cubed distance
                double invDist = 1.0/rAB.Length;
                double invDist3 = invDist * invDist * invDist;

                // Accumulate potentials
                // These will be reused to simplify evaluating 3-body cross-terms. This eliminates the need for a third pass.
                _phi[a] += G * Bodies[b].Mass * invDist;
                _phi[b] += G * Bodies[a].Mass * invDist;

                // Apply Newtonian accelerations
                Vector3D fAB = (G * invDist3) * rAB;
                _aNewt[a] -= Bodies[b].Mass * fAB;
                _aNewt[b] += Bodies[a].Mass * fAB;
            }
        }

        // Apply 1PN corrections
        for (int a = 0; (a < _n); a++) 
        {
            Vector3D a1PN = default;
            double vASq = v[a].LengthSquared;

            for (int b = 0; b < _n; b++)
            {
                if (a == b) continue;

                Vector3D rAB = p[a] - p[b];
                double invDist = 1.0 / rAB.Length;
                Vector3D nAB = rAB * invDist; // Unit vector for direction from B to A

                // Velocity and acceleration projections along unit direction
                double vBSq = v[b].LengthSquared;
                double vAdotvB = Vector3D.Dot(v[a], v[b]);
                double nDotvA = Vector3D.Dot(nAB, v[a]);
                double nDotvB = Vector3D.Dot(nAB, v[b]);
                double rDotaB = Vector3D.Dot(rAB, _aNewt[b]);

                // Scalar coefficient along nAB
                double radCoeff =
                    -vASq                                     // Kinetic energy corrections
                    - 2.0 * vBSq
                    + 4.0 * vAdotvB                           // Frame-dragging/Cross-coupling
                    + 1.5 * (nDotvB * nDotvB)                 // Lorentz length-contraction of B gravitational field along unit vector
                    + 4.0 * _phi[a]                           // Spacetime curvature at body A caused by all other masses (contribute 4*G*m_B/r_AB + 3-body terms)
                    + _phi[b]                                 // As above but at body B (contribute 1*G*m_A/r_AB + 3-body terms)
                    + (4.0 * G * Bodies[a].Mass) * invDist    // Remainder of nonlinear self-energy
                    + 0.5 * rDotaB;                           //Tidal / frame-acceleration distortion from third bodies acting upon body B

                // Scalar coefficient along relative velocity
                double vCoeff = 4.0 * nDotvA - 3.0 * nDotvB;

                // Prefactor: (G * m_B) / (c^2 * r_AB^2)
                double prefactor = (G * Bodies[b].Mass * INVC2) * (invDist * invDist);

                // Aggregate 1PN acceleration vector for body A
                a1PN += prefactor * (radCoeff * nAB + vCoeff * (v[a] - v[b]));
            }
            // Combine Newtonian acceleration with 1PN correction acceleration
            outAccel[a] = _aNewt[a] + a1PN;
        }
    }
}
