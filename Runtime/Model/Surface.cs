using System.Text.Json.Serialization;

namespace CityJson;

public class Surface
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("creationDate")]
    public string CreationDate { get; set; }

    [JsonPropertyName("isClosure")]
    public string IsClosure { get; set; }

    [JsonPropertyName("QuelleClosure")]
    public string QuelleClosure { get; set; }
}