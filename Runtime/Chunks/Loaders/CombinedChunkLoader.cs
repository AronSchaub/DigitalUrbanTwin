using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.GIS;

namespace Leipzig3DGodot.Scripts;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class CombinedChunkLoader : AbstractChunkLoader
{
    public static readonly string CHUNK_BASE_DIR = $"res://Prefabs/Scenes/{nameof(CombinedChunkLoader)}/";

    public List<AbstractChunkLoader> chunkLoaders;

    public override void _Ready()
    {
        base._Ready();
        chunkLoaders = this.GetChildren<AbstractChunkLoader>().ToList();
    }

    public override Node? PopNode()
    {
        return chunkLoaders.Select(loader => loader.PopNode()).FirstOrDefault(node => node != null);
    }

    public override async Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates coordinates)
    {
        foreach (var loader in chunkLoaders)
            await loader.PickNextChunk(globalOrigin, spawnPosition, chunkSize, coordinates);
    }

    public override void NotifyLevelChunkDataRemoved(LevelChunkData value)
    {
        foreach (var loader in chunkLoaders)
            loader.NotifyLevelChunkDataRemoved(value);
    }

    public override string LevelStoreBaseDir()
    {
        return CHUNK_BASE_DIR;
    }

    public override async Task Initialise(CRSCoordinates crs)
    {
        foreach (var loader in chunkLoaders)
            await loader.Initialise(crs);
    }
}