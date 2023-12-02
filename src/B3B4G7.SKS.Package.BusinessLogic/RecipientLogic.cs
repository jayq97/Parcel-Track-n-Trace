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
    public class RecipientLogic : IRecipientLogic
    {
        private readonly IParcelRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipientLogic> _logger;
        public RecipientLogic(IMapper mapper, IParcelRepository repo, ILogger<RecipientLogic> logger)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
        }

        public Parcel TrackParcel(string trackingId)
        {
            try
            {
                DataAccess.Entities.Parcel parcelFromDAL = _repo.GetByTrackingId(trackingId);
                if(parcelFromDAL == null)
                    throw new ParcelNotExistException("Parcel does not exist with this tracking ID.");

                Parcel parcel = _mapper.Map<Parcel>(parcelFromDAL);

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

                Parcel parcelToBL = _mapper.Map<Parcel>(parcelFromDAL);

                _logger.LogInformation($"Found parcel data with: {JsonConvert.SerializeObject(parcelToBL, Formatting.Indented)}");
                return parcelToBL;
            }
            catch (InvalidOperationException ex)
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
