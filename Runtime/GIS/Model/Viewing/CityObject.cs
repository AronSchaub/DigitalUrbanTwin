// using System.Numerics;
// using CityJson.Base;
//
// namespace CityJson.Viewing;
//
// public class CityObject : ICityObject {
//     public CityObject(ICityObject cityObject) {
//         Name = cityObject.Name;
//         Type = cityObject.Type;
//         Attributes = cityObject.Attributes;
//         Guid = cityObject.Guid;
//         Parents = cityObject.Parents;
//         Children = cityObject.Children;
//     }
//
//     public string Name { get; set; }
//     public string Type { get; set; }
//     public Attributes Attributes { get; set; }
//     public string Guid { get; set; }
//     public List<string> Parents { get; set; }
//     public List<string> Children { get; set; }
//     public Vector3 Start { get; }
// }