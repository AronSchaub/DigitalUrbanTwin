using System.ComponentModel;
using Godot;
using Leipzig3DGodot.Scripts;
using Leipzig3DGodot.Scripts.Extensions;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

[Tool]
public partial class Lantern : Node3D, ISelectable
{
    private bool enableDebug;
    private StaticBody3D? staticBody;
    private const string ALBEDO_COLOR = "shader_parameter/albedo_color";

    [Export]
    public bool EnableDebug
    {
        get => enableDebug;
        set
        {
            enableDebug = value;
            if (Light != null)
                Light.Visible = !enableDebug;
            if (LightHull != null)
                LightHull.Visible = enableDebug;
            if (Light2 != null)
                Light2.Visible = !enableDebug;
            if (LightHull2 != null)
                LightHull2.Visible = enableDebug;
        }
    }

    [Export(PropertyHint.Range, "800,12000,100")]
    public float LightTemperature
    {
        get
        {
            if (Light != null)
                return Light.LightTemperature;
            if (Light2 != null)
                return Light2.LightTemperature;
            return -1;
        }
        set
        {
            var color = LightExtensions.ColorFromTemperature(value);

            if (Light != null)
            {
                Light.LightTemperature = value;
                if (LightHull is CsgSphere3D sphere)
                {
                    color.A = sphere.Material.Get(ALBEDO_COLOR).AsColor().A;
                    sphere.Material.Set(ALBEDO_COLOR, color);
                }
            }

            if (Light2 != null)
            {
                Light2.LightTemperature = value;
                if (LightHull2 is CsgSphere3D sphere)
                {
                    color.A = sphere.Material.Get(ALBEDO_COLOR).AsColor().A;
                    sphere.Material.Set(ALBEDO_COLOR, color);
                }
            }
        }
    }

    [Export(PropertyHint.Range, "0.01,180")]
    public float LightAngle
    {
        get
        {
            return Light switch
            {
                OmniLight3D omniLight3D => omniLight3D.OmniRange,
                SpotLight3D spotLight3D => spotLight3D.SpotAngle,
                _ => 0
            };
        }
        set
        {
            if (value == 0)
                return;
            switch (Light)
            {
                case OmniLight3D omniLight3D:
                    omniLight3D.OmniRange = value;
                    break;
                case SpotLight3D spotLight3D:
                    spotLight3D.SpotAngle = value;
                    break;
            }

            switch (Light2)
            {
                case OmniLight3D omniLight3D:
                    omniLight3D.OmniRange = value;
                    break;
                case SpotLight3D spotLight3D:
                    spotLight3D.SpotAngle = value;
                    break;
            }

            switch (LightHull)
            {
                case CsgSphere3D omniLight3D:
                    omniLight3D.Radius = value;
                    break;
                case CsgCylinder3D spotLight3D:
                    spotLight3D.Radius = value;
                    break;
            }

            switch (LightHull2)
            {
                case CsgSphere3D omniLight3D:
                    omniLight3D.Radius = value;
                    break;
                case CsgCylinder3D spotLight3D:
                    spotLight3D.Radius = value;
                    break;
            }
        }
    }

    [Export(PropertyHint.Range, "0,10")]
    public float LightIntensity
    {
        get
        {
            return Light switch
            {
                OmniLight3D omniLight3D => 10f - omniLight3D.OmniAttenuation,
                SpotLight3D spotLight3D => 10f - spotLight3D.SpotAttenuation,
                _ => 0
            };
        }
        set
        {
            switch (Light)
            {
                case OmniLight3D omniLight3D:
                    omniLight3D.OmniAttenuation = 10f - value;
                    break;
                case SpotLight3D spotLight3D:
                    spotLight3D.SpotAttenuation = 10f - value;
                    break;
            }

            switch (Light2)
            {
                case OmniLight3D omniLight3D:
                    omniLight3D.OmniAttenuation = 10f - value;
                    break;
                case SpotLight3D spotLight3D:
                    spotLight3D.SpotAttenuation = 10f - value;
                    break;
            }

            if (LightHull is CsgSphere3D sphere && Light != null)
            {
                var c = sphere.Material.Get(ALBEDO_COLOR).AsColor();
                c.A = value / 10f;
                sphere.Material.Set(ALBEDO_COLOR, c);
            }

            if (LightHull2 is CsgSphere3D sphere2 && Light2 != null)
            {
                var c = sphere2.Material.Get(ALBEDO_COLOR).AsColor();
                c.A = value / 10f;
                sphere2.Material.Set(ALBEDO_COLOR, c);
            }
        }
    }

    [Export]
    public StaticBody3D StaticBody
    {
        get => staticBody ??= this.GetChild<StaticBody3D>(true);
        set => staticBody = value;
    }

    [Export]
    public Light3D? Light { get; set; }

    [Export]
    public Light3D? Light2 { get; set; }

    [Export]
    public CsgPrimitive3D? LightHull { get; set; }

    [Export]
    public CsgPrimitive3D? LightHull2 { get; set; }

    [Export]
    public Outline? Outline { get; set; }

    public bool Collision
    {
        get => !StaticBody.GetChild<CollisionShape3D>().Disabled;
        set => StaticBody.GetChild<CollisionShape3D>().Disabled = !value;
    }

    public override void _Ready()
    {
        Light ??= this.GetChild<Light3D>(true);
        LightHull ??= this.GetChild<CsgPrimitive3D>(true);
        Outline ??= this.GetChild<Outline>(true);
        if (Light != null)
            Light.Visible = !enableDebug;
        if (LightHull != null)
            LightHull.Visible = enableDebug;
    }

    public override void _Process(double delta)
    {
    }

    public void Hover()
    {
        if (Outline != null)
            if (Outline.State != Outline.States.Selected)
                Outline.State = Outline.States.Hover;
    }

    public void UnHover()
    {
        if (Outline != null)
        {
            if (Outline.State != Outline.States.Selected)
                Outline.State = Outline.States.None;
        }
    }

    public void ToggleSelect()
    {
        if (Outline != null)
            Outline.State = Outline.State == Outline.States.Selected
                ? Outline.States.None
                : Outline.States.Selected;
    }

    public void Select()
    {
        if (Outline != null)
        {
            foreach (var selectable in BuildViewModel.Instance.CurrentSelected)
                selectable.DeSelect();

            BuildViewModel.Instance.CurrentSelected.Clear();
            Outline.State = Outline.States.Selected;
            BuildViewModel.Instance.CurrentSelected.Add(this);
        }
    }

    public void DeSelect()
    {
        if (Outline != null)
        {
            Outline.State = Outline.States.None; //todo set back to hovered if it currently is hovered
        }
    }

    public bool IsSelected()
    {
        return Outline != null && Outline.State == Outline.States.Selected;
    }
}