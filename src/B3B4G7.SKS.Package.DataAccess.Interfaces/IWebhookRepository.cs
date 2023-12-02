using B3B4G7.SKS.Package.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.DataAccess.Interfaces
{
    public interface IWebhookRepository
    {
        WebhookResponse Create(string trackingId, string url);
        List<WebhookResponse> GetList(string trackingId);
        void Delete(long id);
    }
}
