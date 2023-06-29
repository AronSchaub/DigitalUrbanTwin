using Godot;
using System;

public partial class ChildrenDemo : Node
{
	public override void _Ready()
	{
		foreach (var child in this.GetChildren<Node>(true))
		{
			GD.Print(child.Name);
		}

		GD.Print("Done");
	}

	public override void _Process(double delta)
	{
	}
}
