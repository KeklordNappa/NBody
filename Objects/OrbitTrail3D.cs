namespace NBody.Objects;
using Godot;
using System;

[GlobalClass]
public partial class OrbitTrail3D : MeshInstance3D
{
    public Body TgtBody { get; set; }
    public int MaxPoints { get; set; } = 400;
    public float MinPointDist { get; set; } = 0.03f;
    public Color TrailColor { get; set; } = new(0.4f, 0.8f, 1.0f, 0.8f);

    private ImmediateMesh _mesh;
    private Vector3[] _points;
    private int _head = 0;
    private int _count = 0;
    private Vector3 _lastPos;

    public override void _Ready()
    {
        TgtBody ??= GetParentOrNull<Body>();
        if (TgtBody != null)
        {
            TrailColor = TgtBody.TrailColor;
            MaxPoints = TgtBody.TrailLength;
        }

        _mesh = new ImmediateMesh();
        _points = new Vector3[MaxPoints];
        Mesh = _mesh;

        MaterialOverride = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };
        TopLevel = true; // stay anchored to origin
        GlobalPosition = Vector3.Zero;
    }


    public override void _PhysicsProcess(double delta)
    {
        if(TgtBody == null) return;

        GlobalTransform = Transform3D.Identity;
        Vector3 cPos = TgtBody.GlobalPosition;
        if (_count == 0 || cPos.DistanceSquaredTo(_lastPos) >= MinPointDist) DrawPoint(cPos);
        RebuildMesh(cPos);
    }

    private void DrawPoint(Vector3 p)
    {
        _points[_head] = p;
        _head = (_head + 1) % MaxPoints;
        if (_count < MaxPoints) _count++;
        _lastPos = p;
    }

    private void RebuildMesh(Vector3 p)
    {
        _mesh.ClearSurfaces();
        if (_count < 1 ) return;

        _mesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
        int startIdx = (_head - _count + MaxPoints) % MaxPoints;

        for (int i = 0; i < _count; i++)
        {
            int idx = (startIdx + i) % MaxPoints;
            float alphaMod = (float)i / _count;

            _mesh.SurfaceSetColor(new Color(TrailColor.R, TrailColor.G, TrailColor.B, alphaMod));
            _mesh.SurfaceAddVertex(_points[idx]);
        }
        _mesh.SurfaceSetColor(TrailColor);
        _mesh.SurfaceAddVertex(p);

        _mesh.SurfaceEnd();
    }


    public void ClearTrail()
    {
        _head = 0;
        _count = 0;
        _mesh?.ClearSurfaces();
    }
}
