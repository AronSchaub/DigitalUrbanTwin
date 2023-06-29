using Godot;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de">Aron Schaub</author>
[Tool]
public partial class Player : CharacterBody3D
{
    private bool refresh;

    [Export]
    public bool Refresh
    {
        get => refresh;
        set => refresh = value;
    }

    public override void _Ready()
    {
        base._Ready();
        // ContactMonitor = true;
        // ContactsReported = 5;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!refresh) return;
        refresh = false;
        GD.Print("Edit Mode");
        // GD.Print(this.GetComponent<Camera>());
        // GD.Print(this.GetComponentInParent<DirectionalLight>());
    }
}

public static class Tags
{
    public const string CHUNK = "Chunk";
}