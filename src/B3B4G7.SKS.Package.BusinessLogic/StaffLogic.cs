using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.WebhookManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace B3B4G7.SKS.Package.BusinessLogic
{
    public class StaffLogic : IStaffLogic
    {
        private readonly IWarehouseRepository _wRepo;
        private readonly IParcelRepository _pRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffLogic> _logger;
        private readonly IWebHookManager _manager;
        public StaffLogic(IMapper mapper, IWarehouseRepository wRepo, IParcelRepository pRepo, ILogger<StaffLogic> logger, IWebHookManager manager)
        {
            _mapper = mapper;
            _wRepo = wRepo;
            _pRepo = pRepo;
            _logger = logger;
            _manager = manager;
        }

        public void ReportParcelDelivery(string trackingId, string baseUrl)
        {
            try
            {
                DataAccess.Entities.Parcel parcelFromDAL = _pRepo.GetByTrackingId(trackingId);
                if (parcelFromDAL == null)
                    throw new ParcelNotExistException("Parcel does not exist with this tracking ID.");

                Parcel parcel = _mapper.Map<Parcel>(parcelFromDAL);

                _logger.LogInformation($"Found parcel data with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");

                var validator = new ParcelValidator();
                var result = validator.Validate(parcel);

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

                _pRepo.UpdateAllListHops(parcelFromDAL);

                _pRepo.UpdateState(parcelFromDAL, DataAccess.Entities.Parcel.StateEnum.DeliveredEnum);

                _manager.UnsubscribeParcelWebhook(trackingId, baseUrl);
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
        }

        public void ReportParcelHop(string trackingId, string code, string baseUrl)
        {
            try
            {
                DataAccess.Entities.Parcel parcelFromDAL = _pRepo.GetByTrackingId(trackingId);
                if (parcelFromDAL == null)
                    throw new ParcelNotExistException("Parcel does not exist with this tracking ID.");

                Parcel parcel = _mapper.Map<Parcel>(parcelFromDAL);

                _logger.LogInformation($"Found parcel data with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");

                DataAccess.Entities.Hop hopFromDAL = _wRepo.GetByCode(code);
                if (hopFromDAL == null)
                    throw new HopsNotExistException("Hop does not exist with this code.");

                Hop hop = _mapper.Map<Hop>(hopFromDAL);

                _logger.LogInformation($"Found hop data with: {JsonConvert.SerializeObject(hop, Formatting.Indented)}");

                var pValidator = new ParcelValidator();
                var pResult = pValidator.Validate(parcel);

                if (!pResult.IsValid)
                {
                    string message = $"The validation operation failed due to following error: {Environment.NewLine}";
                    foreach (var failure in pResult.Errors)
                    {
                        message += $"Property: {failure.PropertyName}, Error Code: {failure.ErrorCode}{Environment.NewLine}";
                    }
                    _logger.LogError(message);
                    throw new InvalidObjectException(message);
                }

                var hValidator = new HopValidator();
                var hResult = hValidator.Validate(hop);

                if (!hResult.IsValid)
                {
                    string message = $"The validation operation failed due to following error: {Environment.NewLine}";
                    foreach (var failure in hResult.Errors)
                    {
                        message += $"Property: {failure.PropertyName}, Error Code: {failure.ErrorCode}{Environment.NewLine}";
                    }
                    _logger.LogError(message);
                    throw new InvalidObjectException(message);
                }

                _pRepo.UpdateListHops(parcelFromDAL, hop.Code);

                switch (hop.HopType)
                {
                    case "warehouse":
                        _pRepo.UpdateState(parcelFromDAL, DataAccess.Entities.Parcel.StateEnum.InTransportEnum);
                        break;
                    case "truck":
                        _pRepo.UpdateState(parcelFromDAL, DataAccess.Entities.Parcel.StateEnum.InTruckDeliveryEnum);
                        break;
                    case "transferwarehouse":
                        // TO-DO
                        _pRepo.UpdateState(parcelFromDAL, DataAccess.Entities.Parcel.StateEnum.TransferredEnum);
                        break;
                    default:
                        break;
                }

                _manager.SubscribeParcelWebhook(trackingId, baseUrl);
            }
            catch (InvalidOperationException ex)
            {
                string message = nameof(Exception) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
