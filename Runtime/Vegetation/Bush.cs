using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Leipzig3DGodot.Scripts.Vegetation;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class Bush : Plant
{
    [Export]
    public override float Angle { get; set; } = 25.7f;
    [Export]
    public override int Iterations { get; set; } = 1;

    protected char axiom = 'Y';

    public override void _Ready()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "X[-FFF][+FFF]FX" },
            { 'Y', "YFX[+Y][-Y]" }
        };
        turtleCommands = new Dictionary<char, Action<Turtle>>
        {
            { 'F', turtle => turtle.Translate(new Vector3(0, 0.1f, 0)) },
            { '+', turtle => turtle.Rotate(new Vector3(0, 0, Angle)) },
            { '-', turtle => turtle.Rotate(new Vector3(0, 0, -Angle)) },
            { '[', turtle => turtle.Push() },
            { ']', turtle => turtle.Pop() }
        };
        var lSystem = new LSystem(axiom, rules, turtleCommands, Position);
        for (var i = 0; i < Iterations; i++)
        {
            lSystem.GenerateSentence();
        }
        var mi = new MeshInstance3D();
        mi.Mesh = lSystem.DrawSystem();
        AddChild(mi);
        mi.Owner = this;
    }
}