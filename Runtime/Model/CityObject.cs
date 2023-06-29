using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace CityJson;

public class CityObject
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("attributes")]
    public Attributes Attributes { get; set; }

    [JsonPropertyName("geographicalExtent")]
    public List<double> GeographicalExtent { get; set; }

    [JsonPropertyName("geometry")]
    public List<Geometry> Geometry { get; set; }

    [JsonPropertyName("guid")]
    public string Guid { get; set; }

    [JsonPropertyName("parents")]
    public List<string> Parents { get; set; }

    [JsonPropertyName("children")]
    public List<string> Children { get; set; }

    public Vector3 Start => new Vector3((float)GeographicalExtent[0], (float)GeographicalExtent[1], (float)GeographicalExtent[2]);
}