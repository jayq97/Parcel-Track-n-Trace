using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class Hop
    {   
        public string Code { get; set; }
        public string HopType { get; set; }
        public string Description { get; set; }
        public int ProcessingDelayMins { get; set; }
        public string LocationName { get; set; }
        public Point LocationCoordinates { get; set; }
        public WarehouseNextHops WarehouseNextHops { get; set; }
    }
}
