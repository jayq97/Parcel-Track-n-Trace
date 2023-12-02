using B3B4G7.SKS.Package.DataAccess.Entities;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace B3B4G7.SKS.Package.DataAccess.Sql
{
    public class SqlWebhookRepository : IWebhookRepository
    {
        private TracknTraceContext _context;
        private ILogger<SqlWebhookRepository> _logger;

        public SqlWebhookRepository(TracknTraceContext context, ILogger<SqlWebhookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public WebhookResponse Create(string trackingId, string url)
        {
            try
            {
                var newWebhookResponse = new WebhookResponse()
                {
                    CreatedAt = DateTime.Now,
                    TrackingId = trackingId,
                    Url = url
                };

                _context.WebhookResponses.Add(newWebhookResponse);
                _context.SaveChanges();
                _logger.LogInformation("Webhook created.");
                return newWebhookResponse;
            }
            catch (OperationCanceledException ex)
            {
                string message = nameof(OperationCanceledException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                string message = nameof(DbUpdateException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public List<WebhookResponse> GetList(string trackingId)
        {
            try
            {
                List<WebhookResponse> webhookResponses = _context.WebhookResponses.Where(p => p.TrackingId == trackingId).ToList();
                _context.SaveChanges();
                _logger.LogInformation($"Parcel deleted with data {JsonConvert.SerializeObject(webhookResponses, Formatting.Indented)}");
                return webhookResponses;
            }
            catch (OperationCanceledException ex)
            {
                string message = nameof(OperationCanceledException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                string message = nameof(DbUpdateException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
        }

        public void Delete(long id)
        {
            try
            {
                WebhookResponse webhookResponse = _context.WebhookResponses.Where(p => p.Id == id).FirstOrDefault();
                _context.WebhookResponses.Remove(webhookResponse);
                _context.SaveChanges();
                _logger.LogInformation($"Parcel deleted with data {JsonConvert.SerializeObject(webhookResponse, Formatting.Indented)}");
            }
            catch (OperationCanceledException ex)
            {
                string message = nameof(OperationCanceledException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new Exception(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                string message = nameof(DbUpdateException) +
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
