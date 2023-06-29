using OSGeo.OSR;

namespace Leipzig3DASP.Extensions;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public static class SpatialReferenceExtensions
{
    public static SpatialReference FromWKTAndEPSG(string wkt, int epsg)
    {
        var sr = new SpatialReference(wkt);
        sr.ImportFromEPSG(epsg);
        return sr;
    }
}