using Godot;

namespace Leipzig3DGodot.Scripts;

public partial class FloatingOrigin : Node3D
{
    [Export]
    public float Threshold = 100.0f;

    public ChunkController LayoutGenerator;

    [Export]
    public Camera3D Camera;

    public override void _Ready()
    {
        base._Ready();
        if (LayoutGenerator == null)
            LayoutGenerator = ChunkController.Instance;
        LayoutGenerator.LevelSize = Mathf.CeilToInt(Threshold);
        if (Camera == null)
            Camera = GetNode<Camera3D>("Spawn/Camera");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Camera != null && Camera.GlobalTransform.Origin.Length() > Threshold)
        {
            var cameraPosition = Camera.GlobalTransform.Origin;
            cameraPosition.Y = 0f;

            var globalTransform = GlobalTransform;
            globalTransform.Origin -= cameraPosition;
            Camera.Call("_on_floating_origin_set", cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            // Camera.new_translation -= cameraPosition;
            GlobalTransform = globalTransform;

            var originDelta = Vector3.Zero - cameraPosition;
            LayoutGenerator.UpdateSpawnOrigin(originDelta);
            GD.Print("recenter origin delta = " + originDelta);
        }
    }
}