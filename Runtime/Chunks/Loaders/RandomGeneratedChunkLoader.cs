using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Leipzig3DGodot.Scripts.GIS;

namespace Leipzig3DGodot.Scripts;

public partial class RandomGeneratedChunkLoader : AbstractChunkLoader
{
    public static readonly string CHUNK_BASE_DIR = "res://Prefabs/Scenes/LowPolyChunks/";

    // public static readonly string LEVEL_STORE_BASE_DIR = "";
    public static readonly string LEVEL_STORE_BASE_DIR = "res://Resource/Scenes/LowPolyChunks/";
    protected readonly Godot.Collections.Dictionary<Vector3, int[]> NeighbourData = new Godot.Collections.Dictionary<Vector3, int[]>();

    public override async Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates crs)
    {
        var neighbours = new[] { 0, 0, 0, 0 };
        var realGlobalPosition = new Vector3(globalOrigin.X + spawnPosition.X * chunkSize, globalOrigin.Y + spawnPosition.Y * chunkSize, globalOrigin.Z + spawnPosition.Z * chunkSize);
        if (NeighbourData.ContainsKey(realGlobalPosition))
            neighbours = NeighbourData[realGlobalPosition];
        else
            NeighbourData.Add(realGlobalPosition, neighbours);
        //TODO do wave function collapse
        var baseChunk = ResourceLoader.Load<PackedScene>($"{CHUNK_BASE_DIR}{string.Join("-", neighbours)}.tscn").Instantiate<Area3D>();
        //TODO add trees and such
        baseChunk.Scale = new Vector3(chunkSize, 1, chunkSize);
        baseChunk.Name = $"{realGlobalPosition}";
        // levelChunkData.Chunk = baseChunk;
        // levelChunkData.ValidNeighbours = neighbours;
        // levelChunkData.VirtualPosition = spawnPosition;
        nodes.Enqueue(baseChunk);
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
    }
}