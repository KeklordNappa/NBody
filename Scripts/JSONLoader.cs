namespace NBody.Scripts;
using Godot;
using System;
using NBody.Objects;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Godot.Collections;

public static class JSONLoader
{
    private record BodyRecord(double mass, double[] pos, double[] vel);

    public static void PopulateSystem(string jsonPath, Body[] bodies)
    {
        using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
        string jsonText = file.GetAsText();

        var records = JsonSerializer.Deserialize<Dictionary<string, R_Body>>(jsonText);
        foreach (Body body in bodies)
        {
            if (records != null && records.TryGetValue(body.BodyID, out var rec))
            {
                body.Mass = rec.mass;
                body.InitPos = new Vector3D(rec.pos[0], rec.pos[1], rec.pos[2]);
                body.Position = body.InitPos.ToVector3();
                body.InitVel = new Vector3D(rec.vel[0], rec.vel[1], rec.vel[2]);
                GD.Print($"{body.BodyID} Initial Mass: {body.Mass}. Initial Velocity: {rec.vel[0]},{rec.vel[1]},{rec.vel[2]}");
            } else { GD.PushError($"Key {body.BodyID} not found in {jsonPath}"); }
        }
    }
}
