using System;
using Godot;

namespace Leipzig3DGodot.Scripts;

public partial class Moon : DirectionalLight3D
{
    [Export(PropertyHint.Range, "-180, 180")]
    private float longitude;

    [Export(PropertyHint.Range, "-90, 90")]
    private float latitude;


    /// <summary>
    /// Sets the location on earth
    /// </summary>
    /// <param name="longitude">Longitude of the location</param>
    /// <param name="latitude">Latitude of the location</param>
    public void SetLocation(float longitude, float latitude)
    {
        this.longitude = longitude;
        this.latitude = latitude;
    }

    /// <summary>
    /// Sets the position of the sun
    /// </summary>
    public void SetPosition(DateTime time)
    {
        var angles = new Vector3();
        MoonPosition.CalculatePosition(time, latitude, longitude, out double azi, out double alt);
        angles.X = Mathf.RadToDeg((float)alt);
        angles.Y = Mathf.RadToDeg((float)azi);
        //UnityEngine.Debug.Log(angles);
        Rotation = angles;
        //light.intensity = Mathf.InverseLerp(-12, 0, angles.X);
    }
}