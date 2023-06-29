using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Leipzig3DGodot.Scripts.Vegetation;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class SimplePlant : Node3D
{
    private char axiom = 'F';

    private readonly Dictionary<char, string> rules = new()
    {
        { 'F', "FF+[+F-F-F-F]-[-F+F+F]" }
    };

    private readonly Dictionary<char, Action<Turtle>> turtleCommands = new()
    {
        { 'F', turtle => turtle.Translate(new Vector3(0, 0.1f, 0)) },
        { '+', turtle => turtle.Rotate(new Vector3(0, 0, 25)) },
        { '-', turtle => turtle.Rotate(new Vector3(0, 0, -25f)) },
        { '[', turtle => turtle.Push() },
        { ']', turtle => turtle.Pop() }
    };

    public override void _Ready()
    {
        var lSystem = new LSystem(axiom, rules, turtleCommands, Position);
        GD.Print(lSystem.GenerateSentence());
        GD.Print(lSystem.GenerateSentence());
        GD.Print(lSystem.GenerateSentence());
        var mi = new MeshInstance3D();
        mi.Mesh = lSystem.DrawSystem();
        AddChild(mi);
        mi.Owner = this;
    }
}