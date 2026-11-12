# NBody

A basic N-Body simulation with attempted correction to 1PN using Einstein-Infeld-Hoffmann equations.


Currently the calculation uses algebraic simplifications that cause deviation from real mechanics in favor of computational speed. Most notably, the term \[-(Gm\_cr\_AB^2) / 2r\_BC^3] is omitted completely from calculation. Complexity of the main branch solver algorithm is O(N^2). I will publish a branch with more exact calculations that should result in a complexity of O(N^3), but with a higher degree of accuracy for arbitrary N-Body systems.



I will also add runtime simulation control and visual clarity features moving forward.

