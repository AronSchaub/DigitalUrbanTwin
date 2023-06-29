using System.ComponentModel;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class MVVMTimeControl : Node
{
    private TimeController? timeControl;
    private Dictionary<StringName, MVVMBean>? bindingDict; //do not " = new()". important for serialisation

    [Export]
    public TimeController TimeControl
    {
        get => timeControl ??= GetParent<TimeController>();
        set => timeControl = value;
    }

    public int Year
    {
        get => TimeControl.Year;
        set => TimeControl.Year = value;
    }

    public int Month
    {
        get => TimeControl.Month;
        set => TimeControl.Month = value;
    }

    public int Day
    {
        get => TimeControl.Day;
        set => TimeControl.Day = value;
    }

    public int Hour
    {
        get => TimeControl.Hour;
        set => TimeControl.Hour = value;
    }

    public int Minute
    {
        get => TimeControl.Minutes;
        set => TimeControl.Minutes = value;
    }

    public int Second
    {
        get => TimeControl.Seconds;
        set => TimeControl.Seconds = value;
    }

    public int Millisecond
    {
        get => TimeControl.Milliseconds;
        set => TimeControl.Milliseconds = value;
    }

    public float Multiplier
    {
        get => TimeControl.Multiplier;
        set => TimeControl.Multiplier = value;
    }

    [Export]
    public Resource? DataModel { get; set; }

    private readonly Array<string> bindingFields = new(new[] { nameof(Year), nameof(Month), nameof(Day), nameof(Hour), nameof(Minute), nameof(Second), nameof(Millisecond), nameof(Multiplier) });
    private bool valueSetting;
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

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnPropertyChanged;
        }
    }

    public override void _Ready()
    {
        if (ViewModel is TimeViewModel timeViewModel)
        {
            timeViewModel.DateTime = TimeControl.DateTime;
            timeViewModel.Speed = TimeControl.Multiplier;
        }

        MVVMUtil.OnReady(ViewModel, this, BindingDict);
        TimeControl.DateTime.DateTimeChanged += OnDateTimeChanged;
    }

    private void OnDateTimeChanged(DateTimeResource value)
    {
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Year) + MVVMUtil.KEY_BINDING, Year, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Month) + MVVMUtil.KEY_BINDING, Month, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Day) + MVVMUtil.KEY_BINDING, Day, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Hour) + MVVMUtil.KEY_BINDING, Hour, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Minute) + MVVMUtil.KEY_BINDING, Minute, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Second) + MVVMUtil.KEY_BINDING, Second, BindingDict);
        MVVMUtil.OnValueChanged(ViewModel, this, nameof(Millisecond) + MVVMUtil.KEY_BINDING, Millisecond, BindingDict);
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
}