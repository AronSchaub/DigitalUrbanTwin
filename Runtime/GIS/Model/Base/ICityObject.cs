using System.Collections.Generic;
using System.Numerics;

namespace CityJson.Base;

public interface ICityObject {
    string Name { get; set; }
    string Type { get; set; }
    // Attributes Attributes { get; set; }
    string Guid { get; set; }
    List<string> Parents { get; set; }
    List<string> Children { get; set; }
    Vector3 Start { get; }
}