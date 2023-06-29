using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Leipzig3DGodot.Scripts.GIS;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public abstract partial class AbstractChunkLoader: Node, IChunkLoader
{
    protected readonly Queue<Node> nodes = new();

    [ExportGroup("Features")]
    [Export]
    public bool RoadsEnabled { get; set; }
    [Export]
    public bool RailwaysEnabled { get; set; }
    [Export]
    public bool NaturalsEnabled { get; set; }
    [Export]
    public bool LandUseEnabled { get; set; }
    [Export]
    public bool WetlandsEnabled { get; set; }
    [Export]
    public bool LeisureEnabled { get; set; }
    [Export]
    public bool BuildingsEnabled { get; set; }
    [Export]
    public bool MarkersEnabled { get; set; }

    public virtual Node? PopNode()
    {
        return nodes.TryDequeue(out var node) ? node : default;
    }
    public abstract Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates crs);
    public abstract void NotifyLevelChunkDataRemoved(LevelChunkData value);
    public abstract string LevelStoreBaseDir();
    public abstract Task Initialise(CRSCoordinates crs);

    public static Vector3 UtmToGodotCoordinates(Vector3 v)
    {
        return new Vector3(v.Y, v.Z, v.X);
    }

    public static Vector3 UtmToGodotCoordinates(float x, float y, float z)
    {
        return new Vector3(y, z, x);
    }
}