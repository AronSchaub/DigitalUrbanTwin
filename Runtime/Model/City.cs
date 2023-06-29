using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace CityJson;

public class City
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("CityObjects")]
    public Dictionary<string, CityObject> CityObjects { get; set; }

    [JsonPropertyName("vertices")]
    public Vector3[] Vertices { get; set; }
}