using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Transferwarehouse : Hop
    {
        public string RegionGeoJson { get; set; }
       
        public string LogisticsPartner { get; set; }
        public string LogisticsPartnerUrl { get; set; }
    }
}
