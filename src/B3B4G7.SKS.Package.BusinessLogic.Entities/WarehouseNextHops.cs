using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class WarehouseNextHops
    {
        public int TraveltimeMins { get; set; }
        public Hop Hop { get; set; }
    }
}
