using System.ComponentModel;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

[Tool]
public partial class MVVMSlider : HSlider
{
    [Export]
    public Resource? DataModel { get; set; }

    private readonly Array<string> bindingFields = new(new[] { nameof(Visible), nameof(Value), nameof(MinValue), nameof(MaxValue) });
    private bool valueSetting;
    private Dictionary<StringName, MVVMBean>? bindingDict; //do not " = new()". important for serialisation
    public Dictionary<StringName, MVVMBean> BindingDict => bindingDict ??= new Dictionary<StringName, MVVMBean>();

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

        ValueChanged += OnValueChanged;

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnPropertyChanged;
        }
    }

    public override void _Ready()
    {
        MVVMUtil.OnReady(ViewModel, this, BindingDict);
    }

    private void OnValueChanged(double value)
    {
        // valueSetting = true;
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Value) + MVVMUtil.KEY_BINDING, value, BindingDict);
        // valueSetting = false;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // if (!valueSetting)
        MVVMUtil.OnPropertyChange(ViewModel, this, e.PropertyName, BindingDict);

        // if (ViewModel != null && e.PropertyName != null)
        // {
        //     string? binding = BindingDict.FirstOrDefault(p => p.Value == e.PropertyName).Value;
        //     if (!string.IsNullOrEmpty(binding))
        //     {
        //         object? val = ViewModel.GetType().GetProperty(e.PropertyName)?.GetValue(ViewModel);
        //         GetType().GetProperty(binding)?.SetValue(this, val);
        //     }
        // }
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