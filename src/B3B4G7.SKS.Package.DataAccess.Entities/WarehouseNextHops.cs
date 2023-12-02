using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class WarehouseNextHops
    {
        public int WarehouseNextHopsId { get; set; }
        public int TraveltimeMins { get; set; }
        public Hop Hop { get; set; }
        public Warehouse Parent { get; set; }
        public string WarehouseFK { get; set; }
        public string HopFK { get; set; }
    }
}
