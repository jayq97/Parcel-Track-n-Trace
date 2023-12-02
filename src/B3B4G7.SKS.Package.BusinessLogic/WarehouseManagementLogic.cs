using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace B3B4G7.SKS.Package.BusinessLogic
{
    public class WarehouseManagementLogic : IWarehouseManagementLogic
    {
        private readonly IWarehouseRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<WarehouseManagementLogic> _logger;
        public WarehouseManagementLogic(IMapper mapper, IWarehouseRepository repo, ILogger<WarehouseManagementLogic> logger)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
        }

        public Warehouse ExportWarehouses()
        {
            try
            {
                DataAccess.Entities.Warehouse warehouseHierarchyFromDAL = _repo.GetWarehouses();
                Warehouse warehouseHierarchy = _mapper.Map<Warehouse>(warehouseHierarchyFromDAL);

                return warehouseHierarchy;
            }
            catch (HopsNotExistInDbException ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new HopsNotExistException(message);
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
        public Hop GetHop(string code)
        {
            try
            {
                DataAccess.Entities.Hop warehouseFromDAL = _repo.GetByCode(code);
                if(warehouseFromDAL == null)
                    throw new HopsNotExistException("Hop does not exist in the database.");

                Hop parcelToBL = _mapper.Map<Hop>(warehouseFromDAL);

                _logger.LogInformation($"Found hop with data: {JsonConvert.SerializeObject(parcelToBL, Formatting.Indented)}");
                return parcelToBL;
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

        public void ImportWarehouses(Warehouse warehouse)
        {
            try
            {
                var validator = new WarehouseValidator();
                var result = validator.Validate(warehouse);

                if (!result.IsValid)
                {
                    string message = $"The validation operation failed due to following error: {Environment.NewLine}";
                    foreach (var failure in result.Errors)
                    {
                        message += $"Property: {failure.PropertyName}, Error Code: {failure.ErrorCode}{Environment.NewLine}";
                    }
                    _logger.LogError(message);
                    throw new InvalidObjectException(message);
                }

                _repo.ClearDatabase();
                _logger.LogInformation($"Previous warehouse hierarchy and it's parcel data deleted");

                DataAccess.Entities.Warehouse warehouseToDAL = _mapper.Map<DataAccess.Entities.Warehouse>(warehouse);
                _repo.Create(warehouseToDAL);

                _logger.LogInformation($"Warehouse hierarchy imported");
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
