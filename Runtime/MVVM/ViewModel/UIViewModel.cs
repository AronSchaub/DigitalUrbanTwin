using System;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using Godot;

namespace Leipzig3DGodot.Scripts.MVVM.ViewModel;

[Tool]
public partial class UIViewModel : BaseViewModel
{
    private static UIViewModel? _Instance;

    private bool menuModeBuildToggle;
    private bool menuModeLightSettingsToggle;
    private bool menuModeTimeSettingsToggle;

    public static UIViewModel Instance => _Instance ??= new UIViewModel();

    public UIViewModel()
    {
        if (_Instance != null)
        {
            GD.PrintErr("there should only be one instance of " + nameof(UIViewModel));
        }

        _Instance = this;
    }

    [Export]
    public bool MenuModeBuildToggle
    {
        get => menuModeBuildToggle;
        set => SetProperty(ref menuModeBuildToggle, value);
    }

    [Export]
    public bool MenuModeLightSettingsToggle
    {
        get => menuModeLightSettingsToggle;
        set => SetProperty(ref menuModeLightSettingsToggle, value);
    }

    [Export]
    public bool MenuModeTimeSettingsToggle
    {
        get => menuModeTimeSettingsToggle;
        set => SetProperty(ref menuModeTimeSettingsToggle, value);
    }

    [RelayCommand]
    public void LanternSelectAll(Node source)
    {
        GD.Print(MethodBase.GetCurrentMethod()?.Name);
        BuildViewModel.Instance.CurrentSelected.AddRange(source.GetTree().Root.GetChildren<Lantern>(true));
    }

    [RelayCommand]
    public void LanternDelete(Node source)
    {
        GD.Print(MethodBase.GetCurrentMethod()?.Name);
        foreach (var node in BuildViewModel.Instance.CurrentSelected.Select(selectable => selectable as Node))
        {
            node.Owner = null;
            node.GetParent().RemoveChild(node);
        }
    }

    [RelayCommand]
    public void Screenshot(Node source)
    {
        GD.Print(MethodBase.GetCurrentMethod()?.Name);
        var error = source.GetTree().Root.GetChild<XRCamera3D>(true).GetViewport().GetTexture().GetImage().SavePng($"../{DateTime.Now:HHmmss}.png");
        GD.PrintErr(error);
    }
}