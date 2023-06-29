using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BackgroundTask.Demo;
using Godot;
using Leipzig3DGodot.Scripts.GIS;
using Node = OsmSharp.Node;

namespace Leipzig3DGodot.Scripts;

public interface IChunkLoader
{
    //https://wiki.openstreetmap.org/wiki/Map_features
    
    //https://wiki.openstreetmap.org/wiki/Highways -> Roads
    public bool RoadsEnabled { get; set; }
    public bool RailwaysEnabled { get; set; }

    //https://wiki.openstreetmap.org/wiki/Vegetation
    public bool NaturalsEnabled { get; set; }
    public bool LandUseEnabled { get; set; }
    public bool WetlandsEnabled { get; set; }
    public bool LeisureEnabled { get; set; }

    //https://wiki.openstreetmap.org/wiki/Buildings
    public bool BuildingsEnabled { get; set; }

    //Markers for addresses, POIs, etc. for now
    public bool MarkersEnabled { get; set; }

    Task PickNextChunk(Vector3 globalOrigin, Vector3 spawnPosition, int chunkSize, CRSCoordinates crs);
    void NotifyLevelChunkDataRemoved(LevelChunkData value);
    string LevelStoreBaseDir();
    Task Initialise(CRSCoordinates crs);
}