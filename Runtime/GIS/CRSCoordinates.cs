using System;
using Godot;
using Leipzig3DASP.Extensions;
using OSGeo.OGR;
using OSGeo.OSR;
using Vector2 = System.Numerics.Vector2;

namespace Leipzig3DGodot.Scripts.GIS;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
/// convertible coordinate reference system
public readonly record struct CRSCoordinates(float x, float y, SpatialReference current)
{
    private const double MeterToAngle = 1 / 111319.491667;

    public CRSCoordinates As(SpatialReference target)
    {
        var wkbPoint = new Geometry(wkbGeometryType.wkbPoint);
        wkbPoint.AssignSpatialReference(current);
        wkbPoint.AddPoint_2D(x, y);
        int result = wkbPoint.TransformTo(target);
        if (result == 0)
        {
            return new CRSCoordinates((float)wkbPoint.GetX(0), (float)wkbPoint.GetY(0), target);
        }

        throw new Exception("wkbPoint.TransformTo returned: " + result);
    }

    public CRSCoordinates AsGPS()
    {
        return As(SpatialReferenceExtensions.FromWKTAndEPSG(Osr.SRS_WKT_WGS84_LAT_LONG, 25833));
    }

    public CRSCoordinates AsUTM_33N()
    {
        return As(SpatialReferenceExtensions.FromWKTAndEPSG(Osr.SRS_WKT_WGS84_LAT_LONG, 4326));
    }
}