using CommunityToolkit.Mvvm.Input;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.ViewModel;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class LightViewModel : BaseViewModel
{
    private int lightTemperature;
    private int lightAngle;
    private float lightIntensity;
    private bool enableDebug;

    [Export]
    public int LightTemperature
    {
        get => lightTemperature;
        set => SetProperty(ref lightTemperature, value);
    }

    [Export]
    public int LightAngle
    {
        get => lightAngle;
        set => SetProperty(ref lightAngle, value);
    }

    [Export]
    public float LightIntensity
    {
        get => lightIntensity;
        set => SetProperty(ref lightIntensity, value);
    }

    [Export]
    public bool EnableDebug
    {
        get => enableDebug;
        set => SetProperty(ref enableDebug, value);
    }

    [RelayCommand]
    public void SetTemperature(string targetTemp)
    {
        if (int.TryParse(targetTemp, out int lightTemp))
        {
            LightTemperature = lightTemp;
        }
    }
}