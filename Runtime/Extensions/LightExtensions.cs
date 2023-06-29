using System;
using Godot;

namespace Leipzig3DGodot.Scripts.Extensions;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
/// based on this blog post: https://tannerhelland.com/2012/09/18/convert-temperature-rgb-algorithm-code.html
/// and based on C# code of: https://gist.github.com/ibober/6b5a6e1dea888c01c0af175e71b15fa4
public static class LightExtensions
{
    /// <summary>
    /// Sets Color from ColorTemperature
    /// </summary>
    /// <param name="self"></param>
    /// <param name="temperature">Temperature must fit between 1000 and 40000 degrees.</param>
    public static Color ColorFromTemperature(float temperature)
    {
        // Temperature must fit between 1000 and 40000 degrees.
        temperature = Mathf.Clamp(temperature, 1000, 40000);

        // All calculations require temperature / 100, so only do the conversion once.
        temperature /= 100;

        // Compute each color in turn.
        float red, green, blue;

        if (temperature <= 66)
        {
            red = 255;
            // Note: the R-squared value for this approximation is 0.996.
            green = (float)(99.4708025861 * Math.Log(temperature) - 161.1195681661);
            if (temperature <= 19)
                blue = 0;
            else
            {
                // Note: the R-squared value for this approximation is 0.998.
                blue = (float)(138.5177312231 * Math.Log(temperature - 10) - 305.0447927307);
                blue = Mathf.Clamp(blue, 0, 255);
            }
        }
        else
        {
            // Note: the R-squared value for this approximation is 0.988.
            red = (float)(329.698727446 * Math.Pow(temperature - 60, -0.1332047592));
            // Note: the R-squared value for this approximation is 0.987.
            green = (float)(288.1221695283 * Math.Pow(temperature - 60, -0.0755148492));
            blue = 255;
        }

        red = Mathf.Clamp(red / 255f, 0, 1);
        green = Mathf.Clamp(green / 255f, 0, 1);
        blue = Mathf.Clamp(blue / 255f, 0, 1);

        return new Color(red, green, blue);
    }
}