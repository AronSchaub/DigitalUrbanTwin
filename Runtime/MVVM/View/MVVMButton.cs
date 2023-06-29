using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class MVVMButton : Button
{
    [Export]
    public Resource DataModel { get; set; }

    const string buttonPressedName = nameof(ButtonPressed) + MVVMUtil.KEY_BINDING;

    private readonly Array<string> bindingFields = new(new[] { nameof(Visible), nameof(Text), nameof(ButtonPressed), nameof(Icon) });

    private Dictionary<StringName, MVVMBean>? bindingDict; //do not " = new()". important for serialisation
    public Dictionary<StringName, MVVMBean> BindingDict => bindingDict ??= new Dictionary<StringName, MVVMBean>();
    public BaseViewModel? ViewModel => DataModel as BaseViewModel;

    public override void _EnterTree()
    {
        base._EnterTree();

        if (ToggleMode)
            Toggled += OnButtonToggled;
        else
        {
            // ButtonDown += OnButtonDown;
            ButtonUp += OnButtonUp;
        }

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnPropertyChanged;
        }
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
        if (MVVMUtil.GetProperty(property, out var variant, bindingFields, BindingDict))
            return variant;
        else
            return base._Get(property);
    }

    public override bool _Set(StringName property, Variant value)
    {
        MVVMUtil.SetProperty(property, value, bindingFields, BindingDict);
        return base._Set(property, value);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
        }
    }

    private void OnButtonToggled(bool buttonPressed)
    {
        MVVMUtil.OnValueChanged(ViewModel, this, buttonPressedName, buttonPressed, BindingDict);
    }

    private void OnButtonDown()
    {
        MVVMUtil.OnValueChanged(ViewModel, this, buttonPressedName, true, BindingDict);
    }

    private void OnButtonUp()
    {
        MVVMUtil.OnValueChanged(ViewModel, this, buttonPressedName, false, BindingDict);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MVVMUtil.OnPropertyChange(ViewModel, this, e.PropertyName, BindingDict);
    }

    // public void PropertySet(object ownValue)
    // {
    //     const string buttonPressedName = nameof(ButtonPressed) + MVVMUtil.KEY_BINDING;
    //     if (BindingDict.ContainsKey(buttonPressedName))
    //     {
    //         PropertyInfo? propertyInfo = ViewModel?.GetType().GetProperty(BindingDict[buttonPressedName]);
    //         if (propertyInfo != null)
    //         {
    //             object? dataModelValue = propertyInfo.GetValue(ViewModel);
    //             if (dataModelValue != ownValue)
    //             {
    //                 if (dataModelValue is IRelayCommand relay)
    //                     relay.Execute(ownValue);
    //                 else
    //                     propertyInfo.SetValue(ViewModel, ownValue);
    //             }
    //         }
    //     }
    // }
    //
    // private void PropertyGet(string? ePropertyName)
    // {
    //     if (ViewModel != null && ePropertyName != null)
    //     {
    //         string? binding = BindingDict.FirstOrDefault(p => p.Value == ePropertyName).Value;
    //         if (!string.IsNullOrEmpty(binding))
    //         {
    //             object? dataModelValue = ViewModel.GetType().GetProperty(ePropertyName)?.GetValue(ViewModel);
    //             PropertyInfo? propertyInfo = GetType().GetProperty(binding);
    //             if (propertyInfo != null)
    //             {
    //                 object? ownValue = propertyInfo.GetValue(this);
    //                 if (dataModelValue != ownValue)
    //                     propertyInfo.SetValue(this, dataModelValue);
    //             }
    //         }
    //     }
    // }
}