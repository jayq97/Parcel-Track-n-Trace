using B3B4G7.SKS.Package.BusinessLogic.Entities;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces
{
    public interface IParcelWebHookLogic
    {
        List<WebhookResponse> ListParcelWebhooks(string trackingId);
        WebhookResponse SubscribeParcelWebhook(string trackingId, string url, string baseUrl);
        void UnsubscribeParcelWebhook(long id);
    }
}
