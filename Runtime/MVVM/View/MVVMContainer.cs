using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

[Tool]
public partial class MVVMContainer : Control
{
    [Export]
    public Resource DataModel { get; set; }

    private readonly Array<string> bindingFields = new(new[] { nameof(Visible) });
    private Dictionary<StringName, MVVMBean>? bindingDict; //do not " = new()". important for serialisation
    public Dictionary<StringName, MVVMBean> BindingDict => bindingDict ??= new Dictionary<StringName, MVVMBean>();

    public BaseViewModel? ViewModel => DataModel as BaseViewModel;

    public override void _EnterTree()
    {
        if (ViewModel != null)
            ViewModel.PropertyChanged += OnPropertyChanged;
        VisibilityChanged += OnVisibilityChanged;
    }

    public override void _Ready()
    {
        MVVMUtil.OnReady(ViewModel, this, BindingDict);
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        return ViewModel == null
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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MVVMUtil.OnPropertyChange(ViewModel, this, e.PropertyName, BindingDict);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
        }
    }

    private void OnVisibilityChanged()
    {
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Visible) + MVVMUtil.KEY_BINDING, Visible, BindingDict);
    }
}