using System.Collections.Generic;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.ViewModel;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
[Tool]
public partial class BuildViewModel : BaseViewModel
{
    //TODO make MVVMFunctionPlace, which knows function place and has reference to this to be able to remove Instance member
    private static BuildViewModel? _Instance;
    public List<ISelectable> CurrentSelected { get; } = new();
    public string UISelection { get; set; } = string.Empty;

    public static BuildViewModel Instance => _Instance ??= new BuildViewModel();

    public BuildViewModel()
    {
        if (_Instance != null)
        {
            GD.PrintErr("there should only be one instance of " + nameof(BuildViewModel));
        }

        _Instance = this;
    }

    [RelayCommand]
    public void LanternSelected(string lanternName)
    {
        UISelection = lanternName;
        GD.Print($"{MethodBase.GetCurrentMethod()}: {lanternName}");
    }
}