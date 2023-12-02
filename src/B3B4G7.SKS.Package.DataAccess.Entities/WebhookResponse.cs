using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]
    public class WebhookResponse
    {
        public long Id { get; set; }
        public string TrackingId { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
