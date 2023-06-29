// using System.Numerics;
// using System.Text.Json.Serialization;
// using CityJson.Base;
//
// namespace CityJson.Parsing; 
//
// public class CityObject : ICityObject {
//     [JsonPropertyName("name")]
//     public string Name { get; set; } = string.Empty;
//
//     [JsonPropertyName("type")]
//     public string Type { get; set; } = string.Empty;
//
//     [JsonPropertyName("attributes")]
//     public Attributes Attributes { get; set; } = new();
//
//     [JsonPropertyName("geographicalExtent")]
//     public List<double> GeographicalExtent { get; set; } = new();
//
//     [JsonPropertyName("geometry")]
//     public List<Geometry> Geometry { get; set; } = new();
//
//     [JsonPropertyName("guid")]
//     public string Guid { get; set; } = string.Empty;
//
//     [JsonPropertyName("parents")]
//     public List<string> Parents { get; set; } = new();
//
//     [JsonPropertyName("children")]
//     public List<string> Children { get; set; } = new();
//
//     public Vector3 Start => new Vector3((float)GeographicalExtent[0], (float)GeographicalExtent[1], (float)GeographicalExtent[2]);
// }