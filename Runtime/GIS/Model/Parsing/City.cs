// using System.Numerics;
// using System.Text.Json.Serialization;
// using CityJson.Base;
//
// namespace CityJson.Parsing;
//
// public class City : ICity {
//     [JsonPropertyName("type")]
//     public string Type { get; set; } = string.Empty;
//
//     [JsonPropertyName("version")]
//     public string Version { get; set; } = string.Empty;
//
//     [JsonPropertyName("metadata")]
//     public Metadata Metadata { get; set; } = new();
//
//     [JsonPropertyName("CityObjects")]
//     public Dictionary<string, CityObject> CityObjects { get; set; } = new();
//
//     public IEnumerable<string> CityObjectIdentifier => CityObjects.Keys.ToHashSet();
//
//     [JsonPropertyName("vertices")]
//     public Vector3[] Vertices { get; set; } = Array.Empty<Vector3>();
// }