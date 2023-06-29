using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Leipzig3DGodot.Scripts.GIS;
using log4net;
using FileAccess = Godot.FileAccess;
using Node = OsmSharp.Node;
using Range = System.Range;

namespace Leipzig3DGodot.Scripts;

public partial class ModelChunkLoader : AbstractChunkLoader
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(ModelChunkLoader));

    public string BasePath = "../Downloads/Quadrant_Innenstadt";

    public string SubPath = "glTF_Aufbereitet";

    // public string SubPath = "glTF_Daten";
    public string CSV = "CenterPoints_Innenstadt.blender.csv";
    public static readonly string CHUNK_BASE_DIR = $"res://Prefabs/Scenes/{nameof(ModelChunkLoader)}/";
    public static readonly string MODEL_BASE_DIR = "res://Models/Innenstadt/{0}.gltf";
    private readonly Dictionary<Vector3, string> cityModelPositions = new();

    public override async Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates coordinates)
    {
        // Log.Debug(MethodBase.GetCurrentMethod());
        var realGlobalPosition = new Vector3(globalOrigin.X + spawnPosition.X * chunkSize, globalOrigin.Z + spawnPosition.Z * chunkSize, globalOrigin.Y + spawnPosition.Y * chunkSize);
        var asUtm33N = coordinates.AsUTM_33N();
        realGlobalPosition += new Vector3(asUtm33N.x, asUtm33N.y, 0);
        
        foreach (((float x, float y, float z), string? sceneName) in cityModelPositions)
        {
            if ((realGlobalPosition.X <= x && x <= realGlobalPosition.X + chunkSize) &&
                (realGlobalPosition.Y <= y && y <= realGlobalPosition.Y + chunkSize))
            {
                Log.Debug($"Distance of '{sceneName}' to chunk ({x - (realGlobalPosition.X + chunkSize)}, {y - (realGlobalPosition.Y + chunkSize)})");
                var packedScene = ResourceLoader.Load<PackedScene>(string.Format(MODEL_BASE_DIR, sceneName));
                var node = packedScene.Instantiate<Node3D>();
                node.GlobalPosition = UtmToGodotCoordinates(x - (realGlobalPosition.X), y - (realGlobalPosition.Y), z);
                // node.GlobalPosition = UtmToGodotCoordinates(0, 0, 15);
                // node.RotateY(180);
                nodes.Enqueue(node);
                return;
            }
        }
    }

    public override void NotifyLevelChunkDataRemoved(LevelChunkData value)
    {
        // Log.Debug(MethodBase.GetCurrentMethod());
    }

    public override string LevelStoreBaseDir() => CHUNK_BASE_DIR;

    public override async Task Initialise(CRSCoordinates coordinates)
    {
        if (!BuildingsEnabled) return;
        string combine = Path.Combine(BasePath, CSV);
        if (File.Exists(combine))
        {
            var fileAccess = FileAccess.Open(combine, FileAccess.ModeFlags.Read);
            fileAccess.GetLine(); //skip header
            while (!fileAccess.EofReached())
            {
                string[] csvLine = fileAccess.GetCsvLine();
                try
                {
                    if (csvLine.Length > 1)
                    {
                        var vector3 = new Vector3(
                            float.Parse(csvLine[1], CultureInfo.InvariantCulture),
                            float.Parse(csvLine[2], CultureInfo.InvariantCulture),
                            0f
                        );
                        var marker = ResourceLoader.Load<PackedScene>("res://Prefabs/Marker.tscn").Instantiate<Marker>();
                        marker.Name = $"{int.Parse(csvLine[0]):000}";
                        marker.Text = marker.Name;
                        cityModelPositions.Add(vector3, marker.Name);
                        var asUtm33N = coordinates.AsUTM_33N();
                        
                        marker.GlobalPosition = UtmToGodotCoordinates(vector3.X - asUtm33N.x, vector3.Y - asUtm33N.y, vector3.Z);
                        marker.Scale *= 10;
                        nodes.Enqueue(marker);
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("couldn't parse: " + string.Join(',', csvLine));
                    Log.Error(e);
                }
            }
        }
    }
}