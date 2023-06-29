// using System.Numerics;
// using System.Text.Json.Serialization;
//
// namespace CityJson;
//
// public class Metadata {
//     [JsonPropertyName("geographicalExtent")]
//     public List<double> GeographicalExtent { get; set; } = new();
//
//     [JsonPropertyName("presentLoDs")]
//     public PresentLoDs PresentLoDs { get; set; } = new();
//
//     public Vector3 Start => GeographicalExtent.Count >= 3 ? new Vector3((float)GeographicalExtent[0], (float)GeographicalExtent[1], (float)GeographicalExtent[2]) : Vector3.Zero;
// }