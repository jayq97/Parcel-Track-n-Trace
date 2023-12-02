namespace B3B4G7.SKS.Package.WebhookManager.Interfaces
{
    public interface IWebHookManager
    {
        public void ListParcelWebHooks(string trackingId);
        public void SubscribeParcelWebhook(string trackingId, string baseUrl);
        public void CallbackOnUrl(string trackingId, string url, string baseUrl);
        public void UnsubscribeParcelWebhook(string trackingId, string baseUrl);
    }
}