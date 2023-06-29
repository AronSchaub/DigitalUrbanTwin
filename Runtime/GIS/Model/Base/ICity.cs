using System.Collections.Generic;

namespace CityJson.Viewing {
}

namespace CityJson.Base {
    public interface ICity {
        string Type { get; set; }
        string Version { get; set; }
        // Metadata Metadata { get; set; }
        IEnumerable<string> CityObjectIdentifier { get; }
    }
}