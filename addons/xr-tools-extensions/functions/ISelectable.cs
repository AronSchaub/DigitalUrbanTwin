using Godot;

namespace Leipzig3DGodot.Scripts;

public interface ISelectable
{
    void Hover();
    void UnHover();
    void ToggleSelect();
    void Select();
    void DeSelect();
    bool IsSelected();
}