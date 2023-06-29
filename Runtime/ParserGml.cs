using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CityJson;
using Converter;
using Godot;

public class ParserGml
{
	public static City LoadGml(FileAccess resource)
	{
		var options = new JsonSerializerOptions();
		options.Converters.Add(new Vector3Converter());
		var city = JsonSerializer.Deserialize<City>(resource.GetAsText(), options);
		Console.WriteLine(city);
		return city;
	}
}
