using B3B4G7.SKS.Package.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces
{
    public interface ILogisticsPartnerLogic
    {
        public string TransitionParcel(string trackingId, Parcel parcel);
    }
}
