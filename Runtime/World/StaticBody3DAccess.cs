using System.Linq;
using Godot;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class StaticBody3DAccess : StaticBody3D
{
    

    public PhysicsDirectBodyState3D State { get; private set; }
}