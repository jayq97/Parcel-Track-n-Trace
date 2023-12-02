/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.1
 * 
 * Generated by: https://openapi-generator.tech
 */

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.Services.Attributes;
using B3B4G7.SKS.Package.Services.DTOs;
using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class SenderApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISenderLogic _logic;
        private readonly ILogger<SenderApiController> _logger;

        public SenderApiController(IMapper mapper, ISenderLogic logic, ILogger<SenderApiController> logger)
        {
            _mapper = mapper;
            _logic = logic;
            _logger = logger;
        }

        /// <summary>
        /// Submit a new parcel to the logistics service. 
        /// </summary>
        /// <param name="parcel"></param>
        /// <response code="201">Successfully submitted the new parcel</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">The address of sender or receiver was not found.</response>
        [HttpPost]
        [Route("/parcel")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("SubmitParcel")]
        [SwaggerResponse(statusCode: 201, type: typeof(NewParcelInfo), description: "Successfully submitted the new parcel")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        [SwaggerResponse(statusCode: 404, type: typeof(Error), description: "The address of sender or receiver was not found.")]
        public virtual IActionResult SubmitParcel([FromBody] Parcel parcel)
        {
            try 
            {
                _logger.LogInformation($"Submit Parcel data with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");
                BusinessLogic.Entities.Parcel parcelToBL = _mapper.Map<BusinessLogic.Entities.Parcel>(parcel);
                string trackingId = _logic.SubmitParcel(parcelToBL);

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
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }
    }
}