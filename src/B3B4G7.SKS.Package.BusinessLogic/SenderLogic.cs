using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.ServiceAgents;
using B3B4G7.SKS.Package.ServiceAgents.Interfaces;
using NetTopologySuite.Geometries;
using B3B4G7.SKS.Package.ServiceAgents.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.BusinessLogic
{
    public class SenderLogic : ISenderLogic
    {
        private readonly IParcelRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<SenderLogic> _logger;
        private readonly IGeoEncodingAgent _agent;
        public SenderLogic(IMapper mapper, IParcelRepository repo, ILogger<SenderLogic> logger, IGeoEncodingAgent agent)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
            _agent = agent;
        }

        public string SubmitParcel(Parcel parcel)
        {
            try
            {
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

                var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
                string randomTrackingId = randomizerTextRegex.Generate();

                _logger.LogInformation("Generate Tracking ID");

                DataAccess.Entities.Parcel ParcelIfExist = _repo.GetByTrackingId(randomTrackingId);
                while (ParcelIfExist != null)
                {
                    randomTrackingId = randomizerTextRegex.Generate();
                    ParcelIfExist = _repo.GetByTrackingId(randomTrackingId);
                };

                DataAccess.Entities.Parcel parcelDAL = _mapper.Map<DataAccess.Entities.Parcel>(parcel);
                parcelDAL.TrackingId = randomTrackingId;
                parcelDAL.State = DataAccess.Entities.Parcel.StateEnum.PickupEnum;

                GeoCoordinate senderCoordinates = _agent.EncodeAddress(parcel.Sender).Result;
                GeoCoordinate recipientCoordinates = _agent.EncodeAddress(parcel.Recipient).Result;

                Point senderPoint = new Point(senderCoordinates.Lon, senderCoordinates.Lat)
                {
                    SRID = 4326
                };
                Point recipientPoint = new Point(recipientCoordinates.Lon, recipientCoordinates.Lat)
                {
                    SRID = 4326
                };

                _repo.Create(parcelDAL, senderPoint, recipientPoint);

                _logger.LogInformation($"Submitted parcel data with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");
                return parcelDAL.TrackingId;
            }
            catch (HopsNotExistInDbException ex)
            {
                string message = nameof(HopsNotExistInDbException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new InvalidObjectException(ex.Message);
            }
            catch (PersonAddressNotFoundInAPIException ex)
            {
                string message = nameof(PersonAddressNotFoundInAPIException) +
                    $"{Environment.NewLine} Source: " + ex.Source +
                    $"{Environment.NewLine} Message: " + ex.Message +
                    $"{Environment.NewLine} StackTrace: {Environment.NewLine}" + ex.StackTrace +
                    $"{Environment.NewLine} Base-Exception: " + ex.GetBaseException().ToString();

                _logger.LogError(message);
                throw new PersonAddressNotFoundException(ex.Message);
            }
        }
    }
}

