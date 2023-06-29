using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace CityJson;

public class Metadata
{
    [JsonPropertyName("geographicalExtent")]
    public List<double> GeographicalExtent { get; set; }

    [JsonPropertyName("presentLoDs")]
    public PresentLoDs PresentLoDs { get; set; }

    public Vector3 Start => new Vector3((float)GeographicalExtent[0], (float)GeographicalExtent[1], (float)GeographicalExtent[2]);
}