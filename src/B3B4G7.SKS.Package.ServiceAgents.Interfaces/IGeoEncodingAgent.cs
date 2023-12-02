using B3B4G7.SKS.Package.BusinessLogic.Entities;

namespace B3B4G7.SKS.Package.ServiceAgents.Interfaces
{
    public interface IGeoEncodingAgent
    {
        Task<GeoCoordinate> EncodeAddress(Recipient recipient);
    }
}
