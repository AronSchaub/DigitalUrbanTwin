using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class MVVMLantern : Node
{
    private Lantern? lantern;
    private Godot.Collections.Dictionary<StringName, MVVMBean>? bindingDict; //do not " = new()". important for serialisation

    [Export]
    public Lantern Lantern
    {
        get => lantern ??= GetParent<Lantern>();
        set => lantern = value;
    }

    public bool EnableDebug
    {
        get => Lantern.EnableDebug;
        set => Lantern.EnableDebug = value;
    }

    public float LightTemperature
    {
        get => Lantern.LightTemperature;
        set => Lantern.LightTemperature = value;
    }

    public float LightAngle
    {
        get => Lantern.LightAngle;
        set => Lantern.LightAngle = value;
    }

    public float LightIntensity
    {
        get => Lantern.LightIntensity;
        set => Lantern.LightIntensity = value;
    }
    
    [Export]
    public Resource? DataModel { get; set; }

    private readonly Array<string> bindingFields = new(new[] { nameof(EnableDebug), nameof(LightTemperature), nameof(LightAngle), nameof(LightIntensity) });
    private bool valueSetting;
    public Godot.Collections.Dictionary<StringName, MVVMBean> BindingDict => bindingDict ??= new Godot.Collections.Dictionary<StringName, MVVMBean>();
    public BaseViewModel? ViewModel => DataModel as BaseViewModel;

    public override Array<Dictionary> _GetPropertyList()
    {
        return DataModel == null
            ? base._GetPropertyList()
            : MVVMUtil.PropertyListFor(ViewModel, bindingFields);
    }

    public override Variant _Get(StringName property)
    {
        return MVVMUtil.GetProperty(property, out var variant, bindingFields, BindingDict)
            ? variant
            : base._Get(property);
    }

    public override bool _Set(StringName property, Variant value)
    {
        MVVMUtil.SetProperty(property, value, bindingFields, BindingDict);
        return base._Set(property, value);
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnPropertyChanged;
        }
    }

    public override void _Ready()
    {
        if (ViewModel is LightViewModel timeViewModel)
        {
            timeViewModel.EnableDebug = Lantern.EnableDebug;
        }

        MVVMUtil.OnReady(ViewModel, this, BindingDict);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MVVMUtil.OnPropertyChange(ViewModel, this, e.PropertyName, BindingDict);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (GetParentOrNull<Lantern>() == null)
            warnings.Add("Parent of this node needs to be of type " + nameof(Lantern));

        return warnings.ToArray();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
        }
    }
}