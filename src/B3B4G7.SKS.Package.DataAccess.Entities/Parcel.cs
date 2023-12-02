using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class Parcel
    {
        public int ParcelId { get; set; }
        public float Weight { get; set; }
        public Recipient Recipient { get; set; }
        public int RecipientFK { get; set; }
        public Recipient Sender { get; set; }
        public int SenderFK { get; set; }
        public string TrackingId { get; set; }
        public enum StateEnum
        {
            PickupEnum = 1,
            InTransportEnum = 2,
            InTruckDeliveryEnum = 3,
            TransferredEnum = 4,
            DeliveredEnum = 5
        }
        public StateEnum State { get; set; }

        public List<HopArrival> VisitedHops { get; set; }
        public List<HopArrival> FutureHops { get; set; }

    }
}