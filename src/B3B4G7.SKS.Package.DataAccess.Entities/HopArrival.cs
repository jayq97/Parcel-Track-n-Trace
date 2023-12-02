using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class HopArrival
    {
        public int HopArrivalId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string? VisitedHopsFK { get; set; }
        public string? FutureHopsFK { get; set; }
    }
}
