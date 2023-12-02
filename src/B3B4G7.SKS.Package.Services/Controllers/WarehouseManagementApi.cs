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
    public class WarehouseManagementApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWarehouseManagementLogic _logic;
        private readonly ILogger<WarehouseManagementApiController> _logger;

        public WarehouseManagementApiController(IMapper mapper, IWarehouseManagementLogic logic, ILogger<WarehouseManagementApiController> logger)
        {
            _mapper = mapper;
            _logic = logic;
            _logger = logger;
        }

        /// <summary>
        /// Exports the hierarchy of Warehouse and Truck objects. 
        /// </summary>
        /// <response code="200">Successful response</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No hierarchy loaded yet.</response>
        [HttpGet]
        [Route("/warehouse")]
        [ValidateModelState]
        [SwaggerOperation("ExportWarehouses")]
        [SwaggerResponse(statusCode: 200, type: typeof(Warehouse), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ExportWarehouses()
        {
            try
            {
                _logger.LogInformation($"Export warehouse hierarchy");
                BusinessLogic.Entities.Warehouse warehousesFromBL = _logic.ExportWarehouses();
                Warehouse warehouses = _mapper.Map<Warehouse>(warehousesFromBL);

                return StatusCode(200, warehouses);
            }
            catch (HopsNotExistException ex)
            {
                return StatusCode(404, new Error() { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Get a certain warehouse or truck by code
        /// </summary>
        /// <param name="code"></param>
        /// <response code="200">Successful response</response>
        /// <response code="400">The operation failed due to an error.</response>
        /// <response code="404">No hop with the specified id could be found.</response>
        [HttpGet]
        [Route("/warehouse/{code}")]
        [ValidateModelState]
        [SwaggerOperation("GetWarehouse")]
        [SwaggerResponse(statusCode: 200, type: typeof(Hop), description: "Successful response")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult GetWarehouse([FromRoute (Name = "code")][Required]string code)
        {
            try
            {
                _logger.LogInformation($"Get hop with code: {code}");
                BusinessLogic.Entities.Hop warehousesFromBL = _logic.GetHop(code);
                Hop hop = _mapper.Map<Hop>(warehousesFromBL);

                return StatusCode(200, hop);
            }
            catch (HopsNotExistException ex)
            {
                return StatusCode(404, new Error() { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Imports a hierarchy of Warehouse and Truck objects. 
        /// </summary>
        /// <param name="warehouse"></param>
        /// <response code="200">Successfully loaded.</response>
        /// <response code="400">The operation failed due to an error.</response>
        [HttpPost]
        [Route("/warehouse")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("ImportWarehouses")]
        [SwaggerResponse(statusCode: 400, type: typeof(Error), description: "The operation failed due to an error.")]
        public virtual IActionResult ImportWarehouses([FromBody]Warehouse warehouse)
        {
            try
            {
                _logger.LogInformation($"Import warehouse hierarchy");
                BusinessLogic.Entities.Warehouse warehouseToBL = _mapper.Map<BusinessLogic.Entities.Warehouse>(warehouse);

                _logic.ImportWarehouses(warehouseToBL);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new Error() { ErrorMessage = ex.Message });
            }
        }
    }
}
