using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.WebhookManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Policy;

namespace B3B4G7.SKS.Package.BusinessLogic
{
    public class ParcelWebHookLogic : IParcelWebHookLogic
    {
        private readonly IWebhookRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<ParcelWebHookLogic> _logger;
        private readonly IWebHookManager _manager;
        public ParcelWebHookLogic(IMapper mapper, IWebhookRepository repo, ILogger<ParcelWebHookLogic> logger, IWebHookManager manager)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
            _manager = manager;
        }

        public List<WebhookResponse> ListParcelWebhooks(string trackingId)
        {
            try
            {
                List<DataAccess.Entities.WebhookResponse> webhookResponsesFromDAL = _repo.GetList(trackingId);

                List<WebhookResponse> webhookResponsesToBL = _mapper.Map<List<WebhookResponse>>(webhookResponsesFromDAL);

                _logger.LogInformation($"Created Webhook with data: {JsonConvert.SerializeObject(webhookResponsesToBL, Formatting.Indented)}");
                return webhookResponsesToBL;
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public WebhookResponse SubscribeParcelWebhook(string trackingId, string url, string baseUrl)
        {
            try
            {
                DataAccess.Entities.WebhookResponse webhookResponseFromDAL = _repo.Create(trackingId, url);

                WebhookResponse webhookResponseToBL = _mapper.Map<WebhookResponse>(webhookResponseFromDAL);
                _logger.LogInformation($"Created Webhook with data: {JsonConvert.SerializeObject(webhookResponseToBL, Formatting.Indented)}");

                _manager.CallbackOnUrl(trackingId, url, baseUrl);
                return webhookResponseToBL;
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public void UnsubscribeParcelWebhook(long id)
        {
            try
            {
                _repo.Delete(id);

                _logger.LogInformation($"Delete Webhook with ID: {id}");
            }
            catch (Exception ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }
    }
}
