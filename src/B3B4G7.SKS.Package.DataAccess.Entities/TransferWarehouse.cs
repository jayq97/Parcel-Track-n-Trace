using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class Transferwarehouse : Hop
    {
        public Geometry Region { get; set; }
        public string LogisticsPartner { get; set; }
        public string LogisticsPartnerUrl { get; set; }
    }
}
