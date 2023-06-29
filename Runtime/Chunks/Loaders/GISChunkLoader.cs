using System.Threading.Tasks;
using BackgroundTask.Demo;
using Godot;
using Leipzig3DGodot.Scripts.GIS;
using log4net;
using OSGeo.OGR;

namespace Leipzig3DGodot.Scripts;

public partial class GISChunkLoader : AbstractChunkLoader
{
    [Export(PropertyHint.ResourceType, nameof(CityConfig))]
    private Resource cityConfig;

    [Export]
    private PackedScene streetTemplate;

    private static readonly ILog Log = LogManager.GetLogger(typeof(GISChunkLoader));
    private readonly CityContext context = new();

    // private float CenterPoint_x_UTM_25833 = 317254.95985f; //in meters
    // private float CenterPoint_y_UTM_25833 = 5690875.67465f; //in meters
    public static readonly string CHUNK_BASE_DIR = "res://Scenes/GIS/";
    public static readonly string LEVEL_STORE_BASE_DIR = "res://Resource/Scenes/GIS/";

    public override async Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates coordinates)
    {
        var realGlobalPosition = new Vector3(globalOrigin.X + spawnPosition.X * chunkSize, globalOrigin.Y + spawnPosition.Y * chunkSize, globalOrigin.Z + spawnPosition.Z * chunkSize);
        var asUtm33N = coordinates.AsUTM_33N();
        realGlobalPosition += new Vector3(asUtm33N.x, asUtm33N.y, 0);
        var chunkBuilder = GISChunkBuilder.Create(realGlobalPosition, chunkSize);
        if (RoadsEnabled)
            chunkBuilder.AddStreets(context, streetTemplate);
        if (LeisureEnabled)
            chunkBuilder.AddPlaces(context, streetTemplate);
        if (NaturalsEnabled)
            chunkBuilder.AddVegetation(context);
        foreach (var newNode in chunkBuilder.Build()) 
            nodes.Enqueue(newNode);
    }

    public override void _Ready()
    {
        // _context.Config = cityConfig as CityConfig;
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
        /* -------------------------------------------------------------------- */
        /*      Register format(s).                                             */
        /* -------------------------------------------------------------------- */
        Ogr.RegisterAll();


        /* -------------------------------------------------------------------- */
        /*      Open data source.                                               */
        /* -------------------------------------------------------------------- */
        var ds = Ogr.Open(context.SourcePath, 0);

        if (ds == null)
        {
            Log.Error("Datasource could not be opened");
            return;
        }

        context.DataSet = ds;

        /* -------------------------------------------------------------------- */
        /*      Get driver                                                      */
        /* -------------------------------------------------------------------- */
        var drv = ds.GetDriver();

        if (drv == null)
        {
            Log.Error("Can't get driver.");
            return;
        }

        Log.Info("Using driver " + drv.name);

        for (var iLayer = 0; iLayer < ds.GetLayerCount(); iLayer++)
        {
            var layer = ds.GetLayerByIndex(iLayer);

            if (layer == null)
            {
                Log.Error("FAILURE: Couldn't fetch advertised layer " + iLayer);
                return;
            }

            Log.Debug(layer.GetName() + ": " + layer.GetFeatureCount(0));
            // ReportLayer(layer);
        }

        Log.Info($"{ds.name}(count: {ds.GetLayerCount()})");
    }
}