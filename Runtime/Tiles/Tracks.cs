using Godot;
using Leipzig3DGodot.Scripts.Tiles;

[Tool]
public partial class Tracks : Street
{
    private float distanceBetweenPlanks = .6f;
    private bool isDirty;

    [Export]
    public float DistanceBetweenPlanks
    {
        get => distanceBetweenPlanks;
        set
        {
            distanceBetweenPlanks = value;
            isDirty = true;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (isDirty)
        {
            UpdateMultiMesh();
            isDirty = false;
        }
    }

    private void UpdateMultiMesh()
    {
        float pathLength = Curve.GetBakedLength();
        var count = (int)Mathf.Floor(pathLength / DistanceBetweenPlanks);

        var mm = GetNode<MultiMeshInstance3D>("RailroadCrosstie").Multimesh;
        mm.InstanceCount = count;
        float offset = DistanceBetweenPlanks / 2.0f;
        for (var i = 0; i < count; i++)
        {
            float curveDistance = offset + DistanceBetweenPlanks * i;
            var position = Curve.SampleBaked(curveDistance, true);

            var basis = new Basis();

            var up = Curve.SampleBakedUpVector(curveDistance, true);
            var forward = position.DirectionTo(Curve.SampleBaked(curveDistance + 1f, true));

            basis.X = forward.Cross(up).Normalized();
            basis.Y = up;
            basis.Z = -forward;
            if(i==20)
                GD.Print(basis);

            var transform = new Transform3D(basis, position);
            mm.SetInstanceTransform(i, transform);
        }
    }

    public void OnCurveChanged()
    {
        isDirty = true;
    }
}