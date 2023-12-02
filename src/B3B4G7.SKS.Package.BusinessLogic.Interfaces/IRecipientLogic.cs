using B3B4G7.SKS.Package.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces
{
    public interface IRecipientLogic
    {
        public Parcel TrackParcel(string trackingId);
    }
}
