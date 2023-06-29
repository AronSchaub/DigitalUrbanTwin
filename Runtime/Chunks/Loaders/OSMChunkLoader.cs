using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Leipzig3DASP.Extensions;
using Leipzig3DGodot.Scripts.GIS;
using Leipzig3DGodot.Scripts.OSM;
using log4net;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OSGeo.OGR;
using OSGeo.OSR;
using OsmSharp.Db;
using OsmSharp.Db.Impl;
using OsmSharp.Geo;
using OsmSharp.Geo.Streams;
using OsmSharp.Streams;
using OsmSharp.Streams.Complete;
using Geometry = OSGeo.OGR.Geometry;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class OSMChunkLoader : AbstractChunkLoader
{
    [Export(PropertyHint.ResourceType, nameof(OsmConfig))]
    private Resource config;

    [ExportCategory("Templates")]
    [Export]
    private PackedScene? StreetTemplate { get; set; }

    [Export]
    private PackedScene? MarkerTemplate { get; set; }

    private static readonly ILog Log = LogManager.GetLogger(typeof(OSMChunkLoader));

    // private double latitude = 51.3396200;
    // private double longitude = 12.3712900;
    // private const double MeterToAngle = 1 / 111319.491667;
    private const string ATTRIBUTE_NAME = "name";
    private const string ATTRIBUTE_ID = "id";
    public static readonly string CHUNK_BASE_DIR = "res://Scenes/OSM/";
    public static readonly string LEVEL_STORE_BASE_DIR = "res://Resource/Scenes/OSM/";
    private OsmCompleteStreamSource source;
    private ISnapshotDb db;

    //https://gis.stackexchange.com/questions/403264/transforming-epsg25831-to-epsg4326-in-decimal-degrees-format
    //https://gis.stackexchange.com/questions/142866/converting-latitude-longitude-epsg4326-into-epsg3857

    public override async Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates crs)
    {
        // var inSR = new SpatialReference(Osr.SRS_WKT_WGS84);
        // GD.Print(inSR.ImportFromEPSG(25833));

        var outSR = new SpatialReference(Osr.SRS_WKT_WGS84_LAT_LONG);
        GD.Print(outSR.ImportFromEPSG(4326));


        var wkbPoint = new Geometry(wkbGeometryType.wkbLineString);
        wkbPoint.AddPoint_2D((globalOrigin.X + spawnPosition.X * chunkSize) + crs.x, (globalOrigin.Z + spawnPosition.Z * chunkSize) + crs.y);
        // wkbPoint.AddPoint_2D((globalOrigin.X + spawnPosition.X * chunkSize) + crs.X + chunkSize, (globalOrigin.Z + spawnPosition.Z * chunkSize) + crs.Y);
        // wkbPoint.AddPoint_2D((globalOrigin.X + spawnPosition.X * chunkSize) + crs.X, (globalOrigin.Z + spawnPosition.Z * chunkSize) + crs.Y + chunkSize);
        wkbPoint.AddPoint_2D((globalOrigin.X + spawnPosition.X * chunkSize) + crs.x + chunkSize, (globalOrigin.Z + spawnPosition.Z * chunkSize) + crs.y + chunkSize);
        wkbPoint.AssignSpatialReference(crs.current);


        // float realX = (float)(MeterToAngle * (globalOrigin.X + spawnPosition.X * chunkSize) + crs.Latitude);
        // float realZ = (float)(MeterToAngle * (globalOrigin.Z + spawnPosition.Z * chunkSize) + crs.Longitude);

        // wkbPoint.AddPoint_2D(realX, realZ);
        // wkbPoint.AssignSpatialReference(inSR);
        int transformResult = wkbPoint.TransformTo(outSR);
        if (transformResult == 0)
        {
            var minX = (float)wkbPoint.GetX(0);
            var minY = (float)wkbPoint.GetY(0);
            var maxX = (float)wkbPoint.GetX(1);
            var maxY = (float)wkbPoint.GetY(1);
            GD.Print(minX);
            GD.Print(minY);

            // float chunkSizeInDeg = (float)(chunkSize * MeterToAngle);
            Log.Debug(MethodBase.GetCurrentMethod()?.Name + ": " + new Rect2(minX, minY, maxX, maxY));

            var features = db.Get(minX, minY, maxX, maxY).ToFeatureSource();
            foreach (var feature in features)
            {
                Log.Info((feature.Attributes.Exists(ATTRIBUTE_NAME) ? feature.Attributes[ATTRIBUTE_NAME] : feature.Attributes[ATTRIBUTE_ID]) + ", " + feature.Geometry);
                if (feature.Attributes.Exists(ATTRIBUTE_NAME))
                {
                    switch (feature.Geometry)
                    {
                        case MultiLineString multiLineString:
                            break;
                        case MultiPoint multiPoint:
                            break;
                        case MultiPolygon multiPolygon:
                            break;
                        case GeometryCollection geometryCollection:
                            break;
                        case LinearRing linearRing:
                            break;
                        case LineString lineString:
                            break;
                        case Point point:
                            if (MarkersEnabled)
                            {
                                var marker = MarkerTemplate?.Instantiate<Marker>();
                                if (marker != null)
                                {
                                    marker.Name = new StringName(feature.Attributes[ATTRIBUTE_ID] + "");
                                    marker.Text = "";
                                    foreach (string? attributeName in feature.Attributes.GetNames())
                                    {
                                        marker.Text += $"{attributeName}: {feature.Attributes[attributeName]}\n";
                                        switch (attributeName)
                                        {
                                            case "memorial":
                                                Log.Debug($"{marker.Name} is a {attributeName}: {feature.Attributes[attributeName]}");
                                                break;
                                            case "amenity":
                                                Log.Debug($"{marker.Name} is a {attributeName}: {feature.Attributes[attributeName]}");
                                                break;
                                            case "tourism":
                                                Log.Debug($"{marker.Name} is a {attributeName}: {feature.Attributes[attributeName]}");
                                                break;
                                            case "highway":
                                                Log.Debug($"{marker.Name} is a {attributeName}: {feature.Attributes[attributeName]}");
                                                break;
                                        }
                                    }

                                    marker.Position = new Vector3((float)(point.Y - minX), 0, (float)point.X - minY);
                                    // marker.Position /= (float)MeterToAngle; //TODO: readd somehow
                                    nodes.Enqueue(marker);
                                }
                            }

                            break;
                        case NetTopologySuite.Geometries.Polygon polygon:
                            break;
                    }
                }
            }
        }

        Log.Error("transform result was: " + transformResult);
    }

    public override void NotifyLevelChunkDataRemoved(LevelChunkData value)
    {
    }

    public override string LevelStoreBaseDir()
    {
        return LEVEL_STORE_BASE_DIR;
    }

    public override async Task Initialise(CRSCoordinates crs)
    {
        if (config is not OsmConfig)
            config = new OsmConfig();

        if (config is not OsmConfig osmConfig) return;

        await using var fileStream = File.OpenRead(osmConfig.SourcePath);
        db = new MemorySnapshotDb(new XmlOsmStreamSource(fileStream)).CreateSnapshotDb();

        Log.Debug("done: " + MethodBase.GetCurrentMethod());
    }
}