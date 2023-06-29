using Godot;

public partial class RayCastRenderer : Node3D
{
    private RayCast3D rayCast;
    private MeshInstance3D line;
    private ImmediateMesh lineMesh;

    public override void _Ready()
    {
        base._Ready();
        rayCast = GetNode<RayCast3D>("RayCast");
        line = GetNode<MeshInstance3D>("Line");
        lineMesh = line.Mesh as ImmediateMesh;
    }

    public override void _Process(double delta)
    {
        lineMesh.ClearSurfaces();
        if (rayCast.IsColliding())
        {
            // GD.Print("BAM");
            lineMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
            lineMesh.SurfaceAddVertex(ToLocal(rayCast.GlobalTransform.Origin));
            lineMesh.SurfaceAddVertex(ToLocal(rayCast.GetCollisionPoint()));
            lineMesh.SurfaceEnd();
        }
    }

    // public override void _PhysicsProcess(double delta)
    // {
    // if (GetParent() is Camera camera)
    // {
    //     var from = camera.ProjectRayOrigin(new Vector2(camera.HOffset / 2, camera.VOffset / 2));
    //     var to = from + camera.ProjectRayNormal(new Vector2(camera.HOffset / 2, camera.VOffset / 2)) * 1000f;
    //     var spaceState = GetWorld().DirectSpaceState;
    //     var result = spaceState.IntersectRay(from, to);
    //     if (result.Count > 0)
    //     {
    //         GD.Print("BAM");
    //     }
    // }
    // }
}