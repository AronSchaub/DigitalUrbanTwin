using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using CityJson;

public partial class Load : Node3D
{
    [Export(PropertyHint.File)]
    public string CityJsonFile { get; set; }

    private City city;
    public List<Node3D> GameObjects { get; } = new();
    private IEnumerator generateCityObjects;

    public override void _Ready()
    {
        city = ParserGml.LoadGml(FileAccess.Open(CityJsonFile, FileAccess.ModeFlags.Read));
        generateCityObjects = GenerateCityObjects();
    }

    public override void _Process(double delta)
    {
        generateCityObjects.MoveNext();
    }

    private IEnumerator GenerateCityObjects()
    {
        Rotation = new Vector3(-90, 0, 0); //rotation degrees?
        var i = 0;
        foreach (KeyValuePair<string, CityObject> b in city.CityObjects)
        {
            // yield return new WaitForSeconds(.25f);
            i++;
            var item = new Polygon();
            // item.GetParent().RemoveChild(item);
            AddChild(item);
            item.Name = b.Key;
            item.Draw(city);
            GameObjects.Add(item);
            // GD.Print(b.Key + " created");
            if (i % 100 == 0)
            {
                yield return null;
                // yield break;
            }
        }

        // StaticBatchingUtility.Combine(GameObjects.ToArray(), gameObject);
    }
}