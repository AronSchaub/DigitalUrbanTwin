using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Leipzig3DASP.Extensions;
using Leipzig3DGodot.Scripts.GIS;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace Leipzig3DGodot.Scripts;

public partial class ChunkController : Node
{
    public static ChunkController Instance { get; private set; }
    //https://epsg.io/25833
    // private float CenterPoint_x_UTM_25833 = 317254.95985f; //in meters
    // private float CenterPoint_y_UTM_25833 = 5690875.67465f; //in meters

    private CRSCoordinates coordinates;

    public int LevelSize
    {
        get => Grid.Count;
        set
        {
            foreach (KeyValuePair<Vector2, LevelChunkData> levelChunkData in Grid)
                levelChunkData.Value.Chunk.QueueFree();
            Grid.Clear();
        }
    }

    [Export]
    public int ChunkSize = 64; //meters

    [Export]
    public int ChunksEdgeCount = 9;

    // [Export(PropertyHint.Enum, $"{nameof(GISChunkLoader)},{nameof(RESTChunkLoader)},{nameof(RandomNumberGenerator)}")]
    // public string chunkLoaderName;

    [Export]
    public bool CachingEnabled = true;

    [Export]
    public AbstractChunkLoader? ChunkLoader;

    private Vector3 spawnOrigin;
    private Vector3 playerPosition;
    private Camera3D? camera;
    private Task? initTask;
    protected readonly Dictionary<Vector2, LevelChunkData> Grid = new Dictionary<Vector2, LevelChunkData>();
    protected readonly ConcurrentQueue<IEnumerator> Coroutines = new ConcurrentQueue<IEnumerator>();

    public override void _Ready()
    {
        base._Ready();

        // switch (OS.GetName())
        // {
        //     case "Linux":
        //         Gdal.SetConfigOption("GDAL_DATA", @"/usr/local/share/gdal");
        //         // Gdal.SetConfigOption("GEOTIFF_CSV", @"/usr/local/share/gdal");
        //         // Gdal.SetConfigOption("GDAL_DRIVER_PATH", @"/usr/local/share/gdalplugins");
        //         break;
        //     case "Windows":
        //         Gdal.SetConfigOption("GDAL_DATA", @"C:\GDAL\data");
        //         break;
        // }
        Gdal.AllRegister();

        coordinates = new CRSCoordinates(317254.95985f, 5690875.67465f, SpatialReferenceExtensions.FromWKTAndEPSG(Osr.SRS_WKT_WGS84_LAT_LONG, 25833));


        // ChunkLoader ??= chunkLoaderName switch
        // {
        //     nameof(GISChunkLoader) => new GISChunkLoader(),
        //     nameof(RESTChunkLoader) => new RESTChunkLoader(),
        //     nameof(RandomGeneratedChunkLoader) => new RandomGeneratedChunkLoader(),
        //     _ => new RandomGeneratedChunkLoader()
        // };

        DirAccess? levelStoreBaseDir;
        if ((levelStoreBaseDir = DirAccess.Open(ChunkLoader.LevelStoreBaseDir())) == null || !levelStoreBaseDir.DirExists(ChunkLoader.LevelStoreBaseDir()))
            levelStoreBaseDir?.MakeDirRecursive(ChunkLoader.LevelStoreBaseDir());

        initTask = Task.Run(() => ChunkLoader.Initialise(coordinates));
        camera ??= GetViewport().GetCamera3D();
        StartCoroutine(PickAndSpawnAfterInit());
        // PickAndSpawnChunk();
        Instance = this;
        GD.Print(nameof(ChunkController));
    }

    private IEnumerator PickAndSpawnAfterInit()
    {
        while (!initTask.IsCompleted)
            yield return null;
        PickAndSpawnChunk();
    }

    public override void _Process(double delta)
    {
        foreach (var enumerator in Coroutines)
            enumerator.MoveNext();
        //
        //     base._Process(delta);
        //
        //     if (!initTask.IsCompleted)
        //         return;
        //     if ((int)(playerPosition.X / ChunkSize) != (int)(camera.GlobalTransform.Origin.X / ChunkSize) &&
        //         (int)(playerPosition.Z / ChunkSize) != (int)(camera.GlobalTransform.Origin.Z / ChunkSize))
        //     {
        //         playerPosition = camera.GlobalTransform.Origin;
        // PickAndSpawnChunk();
        //     }
        //
        //     if (GetChildCount() > ChunksEdgeCount * ChunksEdgeCount)
        //         RemoveChunk();
    }

    private void RemoveChunk()
    {
        var basePos = new Vector2((int)playerPosition.X / ChunkSize, (int)playerPosition.Z / ChunkSize);
        IEnumerable<KeyValuePair<Vector2, LevelChunkData>>? filteredGrid = Grid.Where(chunkData => chunkData.Key.X < (int)basePos.X - ChunksEdgeCount / 2 || chunkData.Key.X > (int)basePos.X + ChunksEdgeCount / 2 ||
                                                                                                   chunkData.Key.Y < (int)basePos.Y - ChunksEdgeCount / 2 || chunkData.Key.Y > (int)basePos.Y + ChunksEdgeCount / 2);
        foreach (KeyValuePair<Vector2, LevelChunkData> chunkData in filteredGrid.OrderByDescending(cd => cd.Key.X - ((int)basePos.X - ChunksEdgeCount / 2)))
        {
            chunkData.Value.Chunk.QueueFree();
            Grid.Remove(chunkData.Key);
            ChunkLoader.NotifyLevelChunkDataRemoved(chunkData.Value);
        }
    }

    void PickAndSpawnChunk()
    {
        var basePos = new Vector2((int)playerPosition.X / ChunkSize, (int)playerPosition.Z / ChunkSize);
        for (int x = -ChunksEdgeCount / 2; x <= ChunksEdgeCount / 2; x++)
        {
            for (int z = -ChunksEdgeCount / 2; z <= ChunksEdgeCount / 2; z++) // GODOT flat is XZ (XY is for side scrolling games)
            {
                var chunkPosX = (int)(basePos.X + x);
                var chunkPosZ = (int)(basePos.Y + z);
                var chunkPos = new Vector2(chunkPosX, chunkPosZ);
                var spawnPosition = new Vector3(chunkPosX, 0, chunkPosZ);
                if (Grid.ContainsKey(chunkPos))
                {
                    //regenerate maybe
                }
                else
                {
                    LevelChunkData chunkToSpawn;
                    var virtualGlobalPosition = new Vector3I((int)(spawnOrigin.X + spawnPosition.X), (int)(spawnOrigin.Y + spawnPosition.Y), (int)(spawnOrigin.Z + spawnPosition.Z));
                    var realGlobalPosition = new Vector3I((int)(spawnOrigin.X + spawnPosition.X * ChunkSize), (int)(spawnOrigin.Y + spawnPosition.Y * ChunkSize), (int)(spawnOrigin.Z + spawnPosition.Z * ChunkSize));
                    var chunkSavePath = $"{ChunkLoader.LevelStoreBaseDir()}chunk.{realGlobalPosition.X}.{realGlobalPosition.Z}.tscn";
                    if (ResourceLoader.Exists(chunkSavePath) && CachingEnabled)
                    {
                        var baseChunk = ResourceLoader.Load<PackedScene>(chunkSavePath).Instantiate<Area3D>();
                        chunkToSpawn = new LevelChunkData
                        {
                            Chunk = baseChunk,
                            VirtualPosition = spawnPosition
                        };
                        AddChild(chunkToSpawn.Chunk);
                        Grid.Add(chunkPos, chunkToSpawn);
                    }
                    else
                    {
                        chunkToSpawn = new LevelChunkData
                        {
                            Chunk = new Area3D()
                        };

                        StartCoroutine(SpawnChunk(chunkToSpawn, chunkPosX, chunkPosZ, spawnPosition, chunkPos, chunkSavePath));
                    }
                }
            }
        }
    }

    private IEnumerator SpawnChunk(LevelChunkData chunkToSpawn, int chunkPosX, int chunkPosZ, Vector3 spawnPosition, Vector2 chunkPos, string chunkSavePath)
    {
        chunkToSpawn.Chunk.BodyEntered += OnEntered;
        chunkToSpawn.Chunk.BodyExited += OnExited;

        AddChild(chunkToSpawn.Chunk);
        chunkToSpawn.Chunk.Owner = this;
        var transform = chunkToSpawn.Chunk.Transform;
        transform.Origin = new Vector3(chunkPosX * ChunkSize, 0, chunkPosZ * ChunkSize);
        chunkToSpawn.Chunk.Transform = transform;
        // GD.Print($"new Chunk {spawnPosition} at: {chunkToSpawn.Chunk.GlobalTransform.Origin}");
        Grid.Add(chunkPos, chunkToSpawn);
        var task = ChunkLoader?.PickNextChunk(spawnOrigin, spawnPosition, ChunkSize, coordinates);

        Node? node;
        while ((node = ChunkLoader?.PopNode()) != null || task != null && !task.IsCompleted)
        {
            if (node != null)
            {
                chunkToSpawn.Chunk.AddChild(node);
                node.Owner = chunkToSpawn.Chunk;
            }

            yield return null;
        }

        if (CachingEnabled)
        {
            var packedScene = new PackedScene();
            packedScene.Pack(chunkToSpawn.Chunk);
            ResourceSaver.Save(packedScene, chunkSavePath);
        }
    }

    private void StartCoroutine(IEnumerator iEnumerator)
    {
        Coroutines.Enqueue(iEnumerator);
    }

    private void OnEntered(Node area)
    {
    }

    private void OnExited(Node area)
    {
        // PickAndSpawnChunk();
    }

    public void UpdateSpawnOrigin(Vector3 originDelta)
    {
        spawnOrigin += originDelta;
    }
}