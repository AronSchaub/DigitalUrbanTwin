using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Leipzig3DGodot.Scripts.Vegetation;

public partial class LSystem
{
    private readonly Dictionary<char, string> rules;
    private readonly Dictionary<char, Action<Turtle>> turtleCommands;

    private string sentence;
    private Turtle turtle;


    public LSystem(char axiom, Dictionary<char, string> rules, Dictionary<char, Action<Turtle>> turtleCommands, Vector3 initialPosition)
    {
        this.rules = rules;
        this.sentence = axiom + "";
        this.turtleCommands = turtleCommands;
        turtle = new Turtle(initialPosition);
    }

    public ImmediateMesh DrawSystem()
    {
        turtle.StartDraw();
        foreach (char instruction in sentence)
            if (turtleCommands.TryGetValue(instruction, out Action<Turtle>? command))
                command.Invoke(turtle);

        turtle.EndDraw();
        return turtle.Mesh;
    }

    public string GenerateSentence()
    {
        sentence = IterateSentence(sentence);
        return sentence;
    }

    private string IterateSentence(string oldSentence)
    {
        return oldSentence.Aggregate("", (current, c) =>
        {
            if (rules.TryGetValue(c, out string? n))
                return current + n;
            return current + c;
        });
    }

    public override string ToString()
    {
        return sentence;
    }
}