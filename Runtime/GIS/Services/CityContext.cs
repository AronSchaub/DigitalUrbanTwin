using System.Collections.Generic;
using Godot;
using OSGeo.OGR;

namespace BackgroundTask.Demo;

public class CityContext
{
    private Layer? streetLayer;

    public DataSource DataSet { get; set; }

    public CityConfig Config = new();
    public readonly Dictionary<FeatureKeys, List<Feature>?> FeatureCache = new();

    public Layer StreetLayer => streetLayer ??= DataSet.GetLayerByName(Config[FeatureKeys.AX_STREET]);

    public Layer PlacesLayer => streetLayer ??= DataSet.GetLayerByName(Config[FeatureKeys.AX_PLACES]);

    public Layer VegetationTradeLayer => streetLayer ??= DataSet.GetLayerByName(Config[FeatureKeys.AX_VEGETATION_TRAIT]);

    public Layer HabitatLayer => streetLayer ??= DataSet.GetLayerByName(Config[FeatureKeys.AX_HABITAT]);

    public string SourcePath => "../Data/Leipzig.gpkg";
}

public enum FeatureKeys
{
    AX_PLACES,
    AX_PATH,
    AX_AGRICULTURE,
    AX_FOREST,
    AX_HABITAT,
    AX_VEGETATION_TRAIT,
    AX_STREET
}