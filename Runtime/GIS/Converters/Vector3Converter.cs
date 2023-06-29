using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackgroundTask.Demo;

public class Vector3Converter : JsonConverter<Vector3> {
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var vector3 = new Vector3();
        if (reader.TokenType != JsonTokenType.StartArray)
            return vector3;
        reader.Read();
        vector3.X = float.Parse(reader.GetDouble() + "");
        vector3.Y = float.Parse(reader.GetDouble() + "");
        vector3.Z = float.Parse(reader.GetDouble() + "");
        // Debug.Log(existingValue);
        while (reader.TokenType != JsonTokenType.EndArray) {
            reader.Read();
            // Debug.Log(reader.Read());
        }

        return vector3;
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteNumberValue(value.Z);
        writer.WriteEndArray();
    }
}