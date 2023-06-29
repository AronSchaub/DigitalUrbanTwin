using System.Linq;
using Godot;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class RigidBody3DAccess : RigidBody3D
{
    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        base._IntegrateForces(state);
        State = state;
        // if (state.GetContactCount() > 0)
        // {
        //     GD.Print(string.Join(',', Enumerable.Range(0, state.GetContactCount()).Select(state.GetContactColliderObject).OfType<Node>().Select(n => n.Name)));
        // }
    }

    public PhysicsDirectBodyState3D State { get; private set; }
}