using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Converter;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // if (reader.TokenType != JsonToken.StartArray)
        //     return existingValue;
        // existingValue.X = float.Parse(reader.ReadAsDouble() + "");
        // existingValue.Y = float.Parse(reader.ReadAsDouble() + "");
        // existingValue.Z = float.Parse(reader.ReadAsDouble() + "");
        // // Debug.Log(existingValue);
        // while (reader.TokenType != JsonToken.EndArray)
        // {
        //     reader.Read();
        //     // Debug.Log(reader.Read());
        // }
        //
        // return existingValue;
        return new Vector3();
    }

    public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
    {
        // writer.WriteStartArray();
        // writer.WriteValue(value.X);
        // writer.WriteValue(value.Y);
        // writer.WriteValue(value.Z);
        // writer.WriteEndArray();
    }
}