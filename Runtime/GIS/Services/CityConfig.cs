using Godot;
using Godot.Collections;

namespace BackgroundTask.Demo;

[Tool]
public partial class CityConfig : Resource
{
    [Export]
    private Dictionary<FeatureKeys, StringName> config = new();

    public string this[FeatureKeys key] => config[key];

    public CityConfig(Dictionary<FeatureKeys, StringName> config)
    {
        this.config = config;
    }

    public CityConfig()
    {
        config.Add(FeatureKeys.AX_STREET, "ver01_l"); // AX_Strassenachse
        config.Add(FeatureKeys.AX_PLACES, "ver01_f"); // AX_Platz, AX_Strassenverkehr
        config.Add(FeatureKeys.AX_PATH, "ver02_l"); // AX_Fahrwegachse, AX_WegPfadSteig
        config.Add(FeatureKeys.AX_AGRICULTURE, "veg01_f"); // AX_Landwirtschaft
        config.Add(FeatureKeys.AX_FOREST, "veg02_f"); // AX_Wald
        config.Add(FeatureKeys.AX_HABITAT, "veg03_f"); // AX_Gehoelz, AX_Heide, AX_Moor, AX_Sumpf, AX_UnlandVegetationsloseFlaeche (Habitat, Biotop)
        config.Add(FeatureKeys.AX_VEGETATION_TRAIT, "veg04_f"); // AX_Vegetationsmerkmal
    }
}