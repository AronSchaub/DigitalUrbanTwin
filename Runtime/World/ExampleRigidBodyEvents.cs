using Godot;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class ExampleRigidBodyEvents : RigidBody3D
{
    public override void _Ready()
    {
        BodyEntered += ThisBodyEntered;
    }

    private void ThisBodyEntered(Node body)
    {
        GD.Print(body);
    }
}