using Godot;
using System;

[Tool]
public partial class Outline : MeshInstance3D
{
    private const string EDGE_COLOR = "shader_parameter/outline_color";
    private const string EDGE_WIDTH = "shader_parameter/outline_width";
    private States state;

    [Export(PropertyHint.Range, "0,25")]
    public float Width
    {
        get
        {
            for (var i = 0; i < GetSurfaceOverrideMaterialCount();)
                return GetSurfaceOverrideMaterial(i).Get(EDGE_WIDTH).AsSingle();

            return 5;
        }
        set
        {
            for (var i = 0; i < GetSurfaceOverrideMaterialCount(); i++)
                GetSurfaceOverrideMaterial(i).Set(EDGE_WIDTH, value);
        }
    }

    [Export(PropertyHint.Enum, "Inactive,Hover,Selected,Active,Error,-None-")]
    public States State
    {
        get => state;
        set
        {
            var color = value switch
            {
                States.Inactive => InactiveColor,
                States.Hover => HoverColor,
                States.Selected => SelectedColor,
                States.Active => ActiveColor,
                States.Error => ErrorColor,
                _ => Colors.Transparent
            };
            Visible = color != Colors.Transparent;
            for (var i = 0; i < GetSurfaceOverrideMaterialCount(); i++)
            {
                GetSurfaceOverrideMaterial(i).Set(EDGE_COLOR, color);
            }

            GD.Print(value);
            state = value;
        }
    }

    [Export]
    public Color HoverColor { get; set; } = Colors.Yellow;

    [Export]
    public Color SelectedColor { get; set; } = Colors.Green;

    [Export]
    public Color ActiveColor { get; set; } = Colors.Blue;

    [Export]
    public Color InactiveColor { get; set; } = Colors.Black;

    [Export]
    public Color ErrorColor { get; set; } = Colors.Red;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public enum States
    {
        Inactive = 0,
        Hover = 1,
        Selected = 2,
        Active = 3,
        Error = 4,
        None = -1
    }
}