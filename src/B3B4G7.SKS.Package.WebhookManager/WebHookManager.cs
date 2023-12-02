using AutoMapper;
using B3B4G7.SKS.Package.Services.DTOs;
using B3B4G7.SKS.Package.WebhookManager.Interfaces;
using B3B4G7.SKS.Package.WebhookManager.JsonConvertClass;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace B3B4G7.SKS.Package.WebhookManager
{
    public class WebHookManager : IWebHookManager
    {
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WebHookManager> _logger;

        public WebHookManager(IMapper mapper, IHttpClientFactory httpClientFactory, ILogger<WebHookManager> logger)
        {
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public void ListParcelWebHooks(string trackingId)
        {
            throw new NotImplementedException();
        }

        public async void SubscribeParcelWebhook(string trackingId, string baseUrl)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                HttpResponseMessage response1 = await httpClient.PostAsync("https://www.toptal.com/developers/postbin/api/bin", null);
                response1.EnsureSuccessStatusCode();
                string resultTopTal = await response1.Content.ReadAsStringAsync();
                ToptalJson deserializedToptal = JsonConvert.DeserializeObject<ToptalJson>(resultTopTal);
                httpClient.Dispose();

                httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage response2 = await httpClient.PostAsync($"/parcel/{trackingId}/webhooks" +
                    $"?url=https://www.toptal.com/developers/postbin/{deserializedToptal.binId}", null);
                response2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
            }
        }

        public async void CallbackOnUrl(string trackingId, string url, string baseUrl)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage response1 = await httpClient.GetAsync($"/parcel/{trackingId}");
                response1.EnsureSuccessStatusCode();
                string resultTrackingInformation = await response1.Content.ReadAsStringAsync();
                TrackingInformation deserializedTrackingInformation = JsonConvert.DeserializeObject<TrackingInformation>(resultTrackingInformation);
                WebhookMessage webhookMessage = _mapper.Map<WebhookMessage>(deserializedTrackingInformation);
                webhookMessage.TrackingId = trackingId;

                httpClient.Dispose();

                httpClient = _httpClientFactory.CreateClient();

                dynamic foo = new ExpandoObject();
                foo.Body = JsonConvert.SerializeObject(webhookMessage);
                var stringContent = new StringContent(JsonConvert.SerializeObject(foo), Encoding.UTF8, "application/json");

                HttpResponseMessage response2 = await httpClient.PostAsync($"{url}?trackingId={trackingId}", stringContent);
                response2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
            }
        }

        public async void UnsubscribeParcelWebhook(string trackingId, string baseUrl)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage response1 = await httpClient.GetAsync($"/parcel/{trackingId}/webhooks");
                string result = await response1.Content.ReadAsStringAsync();

                List<WebhookResponse> deserializedResults = JsonConvert.DeserializeObject<List<WebhookResponse>>(result);

                foreach (var element in deserializedResults)
                {
                    HttpResponseMessage response2 = await httpClient.DeleteAsync($"/parcel/webhooks/{element.Id}");

                    response2.EnsureSuccessStatusCode();
                }


            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
            }
        }
    }
}