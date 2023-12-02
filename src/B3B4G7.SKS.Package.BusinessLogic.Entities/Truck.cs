using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Truck : Hop
    {
        public string RegionGeoJson { get; set; }
        public string NumberPlate { get; set; }
    }
}
