using B3B4G7.SKS.Package.DataAccess.Entities;
using NetTopologySuite.Geometries;

namespace B3B4G7.SKS.Package.DataAccess.Interfaces
{
    public interface IParcelRepository
    {

        void Create(Parcel parcel, Point sender, Point recipient);
        void UpdateState(Parcel parcel, Parcel.StateEnum state);
        void UpdateListHops(Parcel parcel, string code);
        void UpdateAllListHops(Parcel parcel);
        Parcel GetByTrackingId(string trackingid);
    }
}