/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.2
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.Services.Attributes;
using B3B4G7.SKS.Package.Services.DTOs;
using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using System.Security.Policy;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace B3B4G7.SKS.Package.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ParcelWebhookApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IParcelWebHookLogic _logic;
        private readonly ILogger<ParcelWebhookApiController> _logger;
        public string BaseUrl { get; set; }

        [ActivatorUtilitiesConstructor]
        public ParcelWebhookApiController(IMapper mapper, IParcelWebHookLogic logic, ILogger<ParcelWebhookApiController> logger) : base()
        {
            _mapper = mapper;
            _logic = logic;
            _logger = logger;
        }

        public ParcelWebhookApiController(IMapper mapper, IParcelWebHookLogic logic, ILogger<ParcelWebhookApiController> logger, string baseUrl)
        {
            _mapper = mapper;
            _logic = logic;
            _logger = logger;
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Get all registered subscriptions for the parcel webhook.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <response code="200">List of webooks for the &#x60;trackingId&#x60;</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpGet]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [SwaggerOperation("ListParcelWebhooks")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<WebhookResponse>), description: "List of webooks for the &#x60;trackingId&#x60;")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ListParcelWebhooks([FromRoute (Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<WebhookResponse>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(Error));
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            try
            {
                _logger.LogInformation($"Tracking parcel with tracking ID: {trackingId}");
                List<BusinessLogic.Entities.WebhookResponse> webhookResponsesFromBL = _logic.ListParcelWebhooks(trackingId);

                List<WebhookResponse> webhookResponses = _mapper.Map<List<WebhookResponse>>(webhookResponsesFromBL);

                return StatusCode(200, webhookResponses);
            }
            catch (InvalidObjectException ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
            catch (ParcelNotExistException ex)
            {
                return StatusCode(404, new Error() { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Subscribe to a webhook notification for the specific parcel.
        /// </summary>
        /// <param name="trackingId"></param>
        /// <param name="url"></param>
        /// <response code="200">Successful response</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No parcel found with that tracking ID.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}/webhooks")]
        [ValidateModelState]
        [SwaggerOperation("SubscribeParcelWebhook")]
        [SwaggerResponse(statusCode: 200, type: typeof(WebhookResponse), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult SubscribeParcelWebhook([FromRoute (Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")]string trackingId, [FromQuery (Name = "url")][Required()]string url)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(WebhookResponse));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(Error));
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            try
            {
                if (BaseUrl != "Test")
                {
                    BaseUrl = HttpContext.Request.GetDisplayUrl().Replace(HttpContext.Request.Path.Value, "");
                }
                _logger.LogInformation($"Subscribe Webhook with tracking ID: {trackingId} and url: {url}");
                BusinessLogic.Entities.WebhookResponse parcelFromBL = _logic.SubscribeParcelWebhook(trackingId, url, BaseUrl);

                WebhookResponse webhookResponse = _mapper.Map<BusinessLogic.Entities.WebhookResponse, WebhookResponse>(parcelFromBL);

                return StatusCode(200, webhookResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Remove an existing webhook subscription.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Success</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">Subscription does not exist.</response>
        [HttpDelete]
        [Route("/parcel/webhooks/{id}")]
        [ValidateModelState]
        [SwaggerOperation("UnsubscribeParcelWebhook")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult UnsubscribeParcelWebhook([FromRoute (Name = "id")][Required]long id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(Error));
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            try
            {
                _logger.LogInformation($"Unsubscribe Webhook with ID: {id}");
                _logic.UnsubscribeParcelWebhook(id);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }
    }
}
