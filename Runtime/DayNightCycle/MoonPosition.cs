using System;

namespace Leipzig3DGodot.Scripts;

/// <summary>
/// The following source came from this blog:
/// TODO
/// http://guideving.blogspot.co.uk/2010/08/sun-position-in-c.html
/// </summary>
public static class MoonPosition
{
    private const double DEG2_RAD = Math.PI / 180.0;
    //private const double Rad2Deg = 180.0 / Math.PI;

    /// <summary>
    /// Calculates the sun light. 
    /// CalcSunPosition calculates the suns "position" based on a
    /// given date and time in local time, latitude and longitude
    /// expressed in decimal degrees. It is based on the method
    /// found here:
    /// http://www.astro.uio.no/~bgranslo/aares/calculate.html
    /// The calculation is only satisfiable correct for dates in
    /// the range March 1 1900 to February 28 2100. 
    /// </summary>
    /// <param name="dateTime">Time and date in local time. </param>
    /// <param name="latitude">Latitude expressed in decimal degrees. </param>
    /// <param name="longitude">Longitude expressed in decimal degrees. </param>
    /// <param name="outAzimuth"></param>
    /// <param name="outAltitude"></param>
    public static void CalculatePosition(
        DateTime dateTime, double latitude, double longitude, out double outAzimuth, out double outAltitude)
    {
        // Convert to UTC  
        dateTime = dateTime.ToUniversalTime();

        // Number of days from J2000.0.  
        double julianDate = 367 * dateTime.Year -
            (int)(7.0 / 4.0 * (dateTime.Year +
                               (int)((dateTime.Month + 9.0) / 12.0))) +
            (int)(275.0 * dateTime.Month / 9.0) +
            dateTime.Day - 730531.5;

        double julianCenturies = julianDate / 36525.0;

        // Sidereal Time  
        double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;

        double siderealTimeUt = siderealTimeHours +
                                366.2422 / 365.2422 * dateTime.TimeOfDay.TotalHours;

        double siderealTime = siderealTimeUt * 15 + longitude;

        // Refine to number of days (fractional) to specific time.  
        julianDate += dateTime.TimeOfDay.TotalHours / 24.0;
        julianCenturies = julianDate / 36525.0;

        // Solar Coordinates  
        double meanLongitude = CorrectAngle(DEG2_RAD *
                                            (280.466 + 36000.77 * julianCenturies));

        double meanAnomaly = CorrectAngle(DEG2_RAD *
                                          (357.529 + 35999.05 * julianCenturies));

        double equationOfCenter = DEG2_RAD * ((1.915 - 0.005 * julianCenturies) *
            Math.Sin(meanAnomaly) + 0.02 * Math.Sin(2 * meanAnomaly));

        double elipticalLongitude =
            CorrectAngle(meanLongitude + equationOfCenter);

        double obliquity = (23.439 - 0.013 * julianCenturies) * DEG2_RAD;

        // Right Ascension  
        double rightAscension = Math.Atan2(
            Math.Cos(obliquity) * Math.Sin(elipticalLongitude),
            Math.Cos(elipticalLongitude));

        double declination = Math.Asin(
            Math.Sin(rightAscension) * Math.Sin(obliquity));

        // Horizontal Coordinates  
        double hourAngle = CorrectAngle(siderealTime * DEG2_RAD) - rightAscension;

        if (hourAngle > Math.PI)
        {
            hourAngle -= 2 * Math.PI;
        }

        double altitude = Math.Asin(Math.Sin(latitude * DEG2_RAD) *
            Math.Sin(declination) + Math.Cos(latitude * DEG2_RAD) *
            Math.Cos(declination) * Math.Cos(hourAngle));

        // Nominator and denominator for calculating Azimuth  
        // angle. Needed to test which quadrant the angle is in.  
        double aziNom = -Math.Sin(hourAngle);
        double aziDenom =
            Math.Tan(declination) * Math.Cos(latitude * DEG2_RAD) -
            Math.Sin(latitude * DEG2_RAD) * Math.Cos(hourAngle);

        double azimuth = Math.Atan(aziNom / aziDenom);

        if (aziDenom < 0) // In 2nd or 3rd quadrant  
        {
            azimuth += Math.PI;
        }
        else if (aziNom < 0) // In 4th quadrant  
        {
            azimuth += 2 * Math.PI;
        }

        outAltitude = altitude;
        outAzimuth = azimuth;
    }

    /// <summary>
    /// Corrects an angle. 
    /// </summary>
    /// <param name="angleInRadians">An angle expressed in radians. </param>
    /// <returns>An angle in the range 0 to 2*PI. </returns>
    private static double CorrectAngle(double angleInRadians)
    {
        if (angleInRadians < 0)
        {
            return 2 * Math.PI - Math.Abs(angleInRadians) % (2 * Math.PI);
        }

        if (angleInRadians > 2 * Math.PI)
        {
            return angleInRadians % (2 * Math.PI);
        }

        return angleInRadians;
    }
}