using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackgroundTask.Demo;

public class JsonStringConverter : JsonConverter<string> {
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        switch (reader.TokenType) {
            case JsonTokenType.Number: {
                int stringValue = reader.GetInt32();
                return stringValue.ToString();
            }
            case JsonTokenType.String:
                return reader.GetString();
            default:
                return "";
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
        writer.WriteStringValue(value);
    }
}