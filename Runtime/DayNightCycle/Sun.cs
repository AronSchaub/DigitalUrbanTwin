using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Environment = Godot.Environment;

namespace Leipzig3DGodot.Scripts;

[Tool]
public partial class Sun : DirectionalLight3D
{
    [Export(PropertyHint.Range, "-180, 180")]
    private double longitude = 12.3730747F;

    [Export(PropertyHint.Range, "-90, 90")]
    private double latitude = 51.3396955F;

    private bool refresh;

    [Export]
    public bool Refresh
    {
        get => refresh;
        set
        {
            // SetPosition(DateTime.Now);
            // SetPosition(DateTime.Today.AddHours(5).AddMinutes(2));
            // SetPosition(DateTime.Today.AddHours(21).AddMinutes(13));
            SetPosition(DateTime.Today.AddHours(13).AddMinutes(7));
            refresh = value;
        }
    }

    private Vector3 angles;
    private EnvironmentController? environmentController;
    private DateTime localTime;

    private EnvironmentController EnvironmentController
    {
        get { return environmentController ??= EnvironmentController.Instance; }
    }


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

    public void SetPosition(DateTime other)
    {
        localTime = other;
        SunPosition.CalculatePosition(localTime, latitude, longitude, out double azi, out double alt);
        // angles.X = Mathf.Rad2deg *(float)alt);
        // angles.Y = Mathf.Rad2deg *(float)azi);
        angles.X = (float)alt;
        angles.Y = (float)azi;
    }

    /// <summary>
    /// Sets the position of the sun
    /// </summary>
    public void SetPosition(int year, int month, int day, int hour, int minute, int second)
    {
        SetPosition(new DateTime(year, month, day, hour, minute, second));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Engine.IsEditorHint())
        {
            if (!refresh) return;
            refresh = false;
        }

        Rotation = angles;
        // var skyMaterial = environmentController.Sky.SkyMaterial as ProceduralSkyMaterial;
        // sky.SunLongitude = Mathf.Rad2deg *angles.Y) - 180F;
        // sky.SunLatitude = Mathf.Rad2deg *angles.X);
        LightEnergy = Mathf.Min(0.7f, Mathf.Max(angles.X, 0));
        ShadowEnabled = LightEnergy > 0;
        if (EnvironmentController.Environment != null)
        {
            EnvironmentController.Environment.AmbientLightEnergy = Mathf.Min(0.5f, Mathf.Max(angles.X, 0) / 30f);
            EnvironmentController.Environment.FogLightColor = LightColor; //ShadowEnabled ? LightColor : ShadowColor;
            // environment.FogSunColor = ShadowEnabled ? LightColor : ShadowColor;
            // environment.FogSunAmount = Mathf.Min(0.1f, Mathf.Max(sky.SunLatitude, 0) / 30f);
        }

        // GD.Print($"{localTime}, ({rad2deg * Rotation.X},{rad2deg * Rotation.Y}), ({LightEnergy})");
    }
}