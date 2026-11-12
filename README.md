# NBody
A 64-bit precision N-body simulation built in Godot .NET (C#), incorporating First-Order Post-Newtonian ($1\text{PN}$) relativistic corrections via the Einstein–Infeld–Hoffmann (EIH) equations.

## Physics & Architecture
All physics calculations run in C# using custom 64-bit `double` vectors (`AU`, `days`, and `Solar Masses`) and a zero-allocation 4th-order Runge-Kutta (`RK4`) integrator before scaling down to Godot's 32-bit scene graph for rendering.

The `main` branch uses an $`O(N^2)`$ two-pass formulation that caches Newtonian potentials and accelerations to approximate three-body cross-terms. This prioritizes performance while remaining accurate for most planetary systems. A separate branch will implement the unapproximated $`O(N^3)`$ EIH equations to provide better accuracy for arbitrary, comparable-mass multi-body systems (such as triple-star or compact binary setups).

## Roadmap
* Runtime simulation controls (time-warp scaling, pause/step, and system presets).
* Visual clarity improvements (3D orbital trail rendering and scaled body visibility).
* Orbital telemetry HUD to track perihelion precession.
* Custom simulation editing.
