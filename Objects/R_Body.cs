namespace NBody.Objects;
using Godot;
using NBody.Scripts;
using System;

[GlobalClass]
public partial class R_Body : Resource
{
    public double mass { get; set; }
    public double[] pos { get; set; }
    public double[] vel { get; set; }
}
