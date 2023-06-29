using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BackgroundTask.Demo;
using CityJson.Base;
using Godot;
using Leipzig3DASP.Extensions;
using Leipzig3DGodot.Scripts.Tiles;
using log4net;
using OSGeo.OGR;

namespace Leipzig3DGodot.Scripts;

public class GISChunkBuilder
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(GISChunkBuilder));
    private readonly Vector3 realGlobalPosition;
    private readonly int chunkSize;
    private readonly Geometry chunkMask;
    private readonly List<Node> nodes = new();

    private GISChunkBuilder(Vector3 realGlobalPosition, int chunkSize, Geometry chunkMask)
    {
        this.realGlobalPosition = realGlobalPosition;
        this.chunkSize = chunkSize;
        this.chunkMask = chunkMask;
    }

    public GISChunkBuilder AddHouses()
    {
        Log.Debug($"{MethodBase.GetCurrentMethod()?.Name}: {chunkMask.Boundary()}");
        return this;
    }

    public GISChunkBuilder AddVegetation(CityContext cityContext)
    {
        Log.Debug($"{MethodBase.GetCurrentMethod()?.Name}: {chunkMask.Boundary()}");
        try
        {
            var layer = cityContext.HabitatLayer;
            layer.SetSpatialFilter(chunkMask);
            double startX = chunkMask.GetX(0);
            double startY = chunkMask.GetY(0);
            var def = layer.GetLayerDefn();
            foreach (var feature in layer.Features())
            {
                var scene = new Plaza();
                Log.Debug(feature.ToJson(def));
                FeatureToVegetation(feature, scene, startX, startY);
                nodes.Add(scene);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        return this;
    }

    private void FeatureToVegetation(Feature feature, Plaza scene, double startX, double startY)
    {
        var coordinates = new List<Vector2>();
        var geometryRef = feature.GetGeometryRef();
        var buffer = new double[3];
        for (var i = 0; i < geometryRef.GetPointCount(); i++)
        {
            geometryRef.GetPoint(i, buffer);
            coordinates.Add(new Vector2
            {
                X = (float)(buffer[0] - startX),
                Y = (float)(buffer[1] - startY)
            });
        }

        scene.Kind = feature.GetFieldAsInteger(FieldIdentifier.OBJART_Z);
        scene.Name = feature.GetFieldAsString(FieldIdentifier.OBJID_Z);
        scene.Mode = CsgPolygon3D.ModeEnum.Depth;
        scene.Polygon = coordinates.ToArray();
        scene.RotateX(Mathf.DegToRad(90));
        scene.Depth = 1f;
    }

    public GISChunkBuilder AddPlaces(CityContext cityContext, PackedScene template)
    {
        Log.Debug($"{MethodBase.GetCurrentMethod()?.Name}: {chunkMask.Boundary()}");
        try
        {
            var layer = cityContext.PlacesLayer;
            layer.SetSpatialFilter(chunkMask);
            double startX = chunkMask.GetX(0);
            double startY = chunkMask.GetY(0);
            var def = layer.GetLayerDefn();
            foreach (var feature in layer.Features())
            {
                var scene = new Plaza();
                // log.Debug(feature.ToJson(def));
                FeatureToPlaza(feature, scene, startX, startY);
                nodes.Add(scene);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        return this;
    }

    private void FeatureToPlaza(Feature feature, Plaza scene, double startX, double startY)
    {
        var coordinates = new List<Vector2>();
        var geometryRef = feature.GetGeometryRef();
        var buffer = new double[3];
        for (var i = 0; i < geometryRef.GetPointCount(); i++)
        {
            geometryRef.GetPoint(i, buffer);
            coordinates.Add(new Vector2
            {
                X = (float)(buffer[0] - startX),
                Y = (float)(buffer[1] - startY)
            });
        }

        scene.Kind = feature.GetFieldAsInteger(FieldIdentifier.OBJART_Z);
        scene.Name = feature.GetFieldAsString(FieldIdentifier.OBJID_Z);
        scene.Mode = CsgPolygon3D.ModeEnum.Depth;
        scene.Polygon = coordinates.ToArray();
        scene.RotateX(Mathf.DegToRad(90));
        scene.Depth = 1f;
    }


    public GISChunkBuilder AddStreets(CityContext cityContext, PackedScene template)
    {
        Log.Debug($"{MethodBase.GetCurrentMethod()?.Name}: {chunkMask.ExportToJson(Array.Empty<string>())}");
        try
        {
            var layer = cityContext.StreetLayer;
            layer.SetSpatialFilter(chunkMask);
            double startX = chunkMask.GetX(0);
            double startY = chunkMask.GetY(0);
            foreach (var feature in layer.Features())
            {
                var scene = template.Instantiate<Street>();
                FeatureToStreet(feature, scene, startX, startY);
                nodes.Add(scene);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

        return this;
    }

    public static GISChunkBuilder Create(Vector3 realGlobalPosition, int chunkSize)
    {
        var gisChunkBuilder = new GISChunkBuilder(realGlobalPosition, chunkSize, CreateChunkMask(realGlobalPosition.X, realGlobalPosition.Y, chunkSize));
        return gisChunkBuilder;
    }

    private static Geometry CreateChunkMask(double startX, double startY, int size)
    {
        var g = new Geometry(wkbGeometryType.wkbLineString);
        g.AddPoint_2D(startX, startY);
        g.AddPoint_2D(startX + size, startY);
        g.AddPoint_2D(startX, startY + size);
        g.AddPoint_2D(startX + size, startY + size);
        return g;
    }

    public IEnumerable<Node> Build()
    {
        return nodes;
    }


    private static void FeatureToStreet(Feature feature, Street scene, double startX, double startY)
    {
        var coordinates = new Curve3D();
        var geometryRef = feature.GetGeometryRef();
        var buffer = new double[3];
        for (var i = 0; i < geometryRef.GetPointCount(); i++)
        {
            geometryRef.GetPoint(i, buffer);
            coordinates.AddPoint(AbstractChunkLoader.UtmToGodotCoordinates(
                (float)(startX - buffer[0]),
                (float)(startY - buffer[1]),
                0
            ));
        }

        scene.StreetName = feature.GetFieldAsString(FieldIdentifier.OBJID_Z);
        scene.Curve = coordinates;
        scene.Texture = "asphalt";
        scene.Width = 2;
    }
}

internal class FieldIdentifier
{
    public const string OBJID_Z = "OBJID_Z";
    public const string OBJART_Z = "OBJART_Z";
}