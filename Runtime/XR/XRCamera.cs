using Godot;
using System;

public partial class XRCamera : XRCamera3D
{
    public override void _EnterTree()
    {
        XRHelpers.XRCamera = this;
    }
}