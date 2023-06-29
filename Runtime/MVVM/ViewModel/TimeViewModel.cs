using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.ViewModel;

[Tool]
public partial class TimeViewModel : BaseViewModel
{
    private DateTimeResource? dateTime;
    private float speed;
    private DateTimeResource DateTimeResource => (DateTime as DateTimeResource)!;

    [Export]
    public Resource DateTime
    {
        get
        {
            dateTime ??= new DateTimeResource();
            dateTime.DateTimeChanged += OnDateTimeChanged;
            return dateTime;
        }
        set
        {
            if (dateTime != null)
            {
                dateTime.DateTimeChanged -= OnDateTimeChanged;
            }

            if (value is DateTimeResource dateTimeResource)
            {
                SetProperty(ref dateTime, dateTimeResource);
                dateTimeResource.DateTimeChanged += OnDateTimeChanged;
            }
        }
    }

    private void OnDateTimeChanged(DateTimeResource dateTimeResource)
    {
        OnPropertyChanged(nameof(DateTime));
    }

    [Export]
    public float Speed
    {
        get => speed;
        set => SetProperty(ref speed, value);
    }

    [RelayCommand]
    public void SetTime(string minutesOfDay)
    {
        string[] time = minutesOfDay.Split(":");
        DateTimeResource.Hour = int.Parse(time[0]);
        DateTimeResource.Minute = int.Parse(time[1]);
    }

    [RelayCommand]
    public void SetMidnight()
    {
        DateTimeResource.Hour = 22;
        DateTimeResource.Minute = 0;
    }

    [RelayCommand]
    public void SetDawn()
    {
        DateTimeResource.Hour = 6;
        DateTimeResource.Minute = 0;
    }

    [RelayCommand]
    public void SetMidday()
    {
        DateTimeResource.Hour = 12;
        DateTimeResource.Minute = 0;
    }

    [RelayCommand]
    public void SetDusk()
    {
        DateTimeResource.Hour = 18;
        DateTimeResource.Minute = 0;
    }


    [RelayCommand]
    public void Pause()
    {
        Speed = 0;
    }

    [RelayCommand]
    public void Play()
    {
        Speed = 1;
    }

    [RelayCommand]
    public void Forward()
    {
        Speed = 60;
    }

    [RelayCommand]
    public void FastForward()
    {
        Speed = 600;
    }
}