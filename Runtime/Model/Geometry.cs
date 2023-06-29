using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJson;

public class Geometry
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("boundaries")]
    public List<List<List<int>>> Boundaries { get; set; }

    [JsonPropertyName("semantics")]
    public Semantics Semantics { get; set; }

    [JsonPropertyName("lod")]
    public int Lod { get; set; }
}