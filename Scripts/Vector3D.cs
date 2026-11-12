namespace NBody.Scripts;
using Godot;
using System;

// Custom implementation of Vector3D to introduce double precision
// and to convert the ecliptic and AU scale into Godot's standard 3D space without doing so in the math itself.
public readonly struct Vector3D
{
    public readonly double X, Y, Z;

    public Vector3D(double x, double y, double z) => (X, Y, Z) = (x, y, z);

    public double LengthSquared => X * X + Y * Y + Z * Z;
    public double Length => Math.Sqrt(LengthSquared);

    public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3D operator *(Vector3D a, double s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3D operator *(double s, Vector3D a) => a * s;
    public static double Dot(Vector3D a, Vector3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public Godot.Vector3 ToVector3(double scale = 10.0)
        => new((float)(X*scale), (float)(Z*scale), (float)(Y*scale));
}