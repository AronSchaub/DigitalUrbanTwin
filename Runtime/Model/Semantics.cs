using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJson;

public class Semantics
{
    [JsonPropertyName("values")]
    public List<int> Values { get; set; }

    [JsonPropertyName("surfaces")]
    public List<Surface> Surfaces { get; set; }
}