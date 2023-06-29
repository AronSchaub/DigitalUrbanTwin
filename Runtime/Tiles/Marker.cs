using Godot;
using System;

public partial class Marker : Node3D
{
	[Export]
	public Label3D? Label { get; set; }

	public string Text
	{
		get => Label?.Text ?? string.Empty;
		set
		{
			if (Label != null) Label.Text = value;
		}
	}

	public override void _Ready()
	{
		Label ??= this.GetChild<Label3D>(true);
	}

	public override void _Process(double delta)
	{
	}
}
