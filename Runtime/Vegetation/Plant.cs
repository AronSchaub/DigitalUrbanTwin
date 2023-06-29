using System;
using System.Collections.Generic;
using Godot;

namespace Leipzig3DGodot.Scripts.Vegetation;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public abstract partial class Plant : Node3D
{
    public abstract float Angle { get; set; }
    public abstract int Iterations { get; set; }

    protected Dictionary<char, string> rules;
    protected Dictionary<char, Action<Turtle>> turtleCommands;
}