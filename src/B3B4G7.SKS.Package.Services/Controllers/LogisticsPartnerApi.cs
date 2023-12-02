/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.1
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.Services.Attributes;
using B3B4G7.SKS.Package.Services.DTOs;
using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class LogisticsPartnerApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogisticsPartnerLogic _logic;
        private readonly ILogger<LogisticsPartnerApiController> _logger;

        public LogisticsPartnerApiController(IMapper mapper, ILogisticsPartnerLogic logic, ILogger<LogisticsPartnerApiController> logger)
        {
            _mapper = mapper;
            _logic = logic;
            _logger = logger;
        }

        /// <summary>
        /// Transfer an existing parcel into the system from the service of a logistics partner. 
        /// </summary>
        /// <param name="trackingId">The tracking ID of the parcel. E.g. PYJRB4HZ6 </param>
        /// <param name="parcel"></param>
        /// <response code="200">Successfully transitioned the parcel</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="409">A parcel with the specified trackingID is already in the system.</response>
        [HttpPost]
        [Route("/parcel/{trackingId}")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("TransitionParcel")]
        [SwaggerResponse(statusCode: 200, type: typeof(NewParcelInfo), description: "Successfully transitioned the parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult TransitionParcel([FromRoute(Name = "trackingId")][Required][RegularExpression("^[A-Z0-9]{9}$")] string trackingId, [FromBody] Parcel parcel)
        {
            try
            {
                _logger.LogInformation($"Transfer Parcel data with tracking id: {trackingId} {Environment.NewLine}" +
                    $"{JsonConvert.SerializeObject(parcel, Formatting.Indented)}");

                BusinessLogic.Entities.Parcel parcelToBL = _mapper.Map<BusinessLogic.Entities.Parcel>(parcel);
                trackingId = _logic.TransitionParcel(trackingId, parcelToBL);

                NewParcelInfo newParcelInfo = _mapper.Map<BusinessLogic.Entities.Parcel, NewParcelInfo>(parcelToBL);
                newParcelInfo.TrackingId = trackingId;

                return StatusCode(201, newParcelInfo);
            }
            catch (InvalidObjectException ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
            catch (PersonAddressNotFoundException ex)
            {
                return StatusCode(404, new Error() { ErrorMessage = ex.Message });
            }
            catch (ParcelAlreadyExistException ex)
            {
                return StatusCode(409, new Error() { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }
    }
}
