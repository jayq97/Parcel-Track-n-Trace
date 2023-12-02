using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Warehouse : Hop
    {
        public int Level { get; set; }
        public List<WarehouseNextHops> NextHops { get; set; }
    }
}
