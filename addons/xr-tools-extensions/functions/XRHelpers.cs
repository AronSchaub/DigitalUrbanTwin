using Godot;

public class XRHelpers
{
    private static XROrigin3D origin;
    private static XRCamera3D camera;

    // public static XROrigin3D GetXROrigin(Node node, NodePath? path = null)
    // {
    //     if (origin == null)
    //     {
    //         if (path != null)
    //         {
    //             origin = node.GetNodeOrNull<XROrigin3D>(path);
    //             if (origin != null)
    //                 return origin;
    //         }
    //     }
    //
    //     return origin;
    // }
    //
    // public static void SetXROrigin(XROrigin3D value)
    // {
    //     origin = value;
    // }
    public static XROrigin3D XROrigin
    {
        get { return origin; }
        set { origin = value; }
    }

    public static XRCamera3D XRCamera
    {
        get { return camera; }
        set { camera = value; }
    }
}