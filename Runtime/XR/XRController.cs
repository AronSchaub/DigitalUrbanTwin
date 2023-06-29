using Godot;
using System;

public partial class XRController : XROrigin3D
{
    public override void _EnterTree()
    {
        XRHelpers.XROrigin = this;
        base._EnterTree();
    }

    public override void _Ready()
    {
        var xrInterface = XRServer.FindInterface("OpenXR");
        if (xrInterface != null && xrInterface.IsInitialized())
        {
            var vp = GetViewport();
            if (vp != null) vp.UseXR = true;
        }
    }

    public override void _Process(double delta)
    {
    }
}