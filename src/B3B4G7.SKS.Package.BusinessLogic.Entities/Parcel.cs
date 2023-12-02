﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Parcel
    {
        public float Weight { get; set; }
        public Recipient Recipient { get; set; }
        public Recipient Sender { get; set; }
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