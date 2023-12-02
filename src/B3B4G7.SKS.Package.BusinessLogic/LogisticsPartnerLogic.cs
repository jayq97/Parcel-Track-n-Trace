using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.ServiceAgents.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using Newtonsoft.Json;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using NetTopologySuite.Geometries;
using B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions;
using B3B4G7.SKS.Package.ServiceAgents.Exceptions;

namespace B3B4G7.SKS.Package.BusinessLogic
{
    public class LogisticsPartnerLogic : ILogisticsPartnerLogic
    {
        private readonly IParcelRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<LogisticsPartnerLogic> _logger;
        private readonly IGeoEncodingAgent _agent;
        public LogisticsPartnerLogic(IMapper mapper, IParcelRepository repo, ILogger<LogisticsPartnerLogic> logger, IGeoEncodingAgent agent)
        {
            _mapper = mapper;
            _repo = repo;
            _logger = logger;
            _agent = agent;
        }

        public string TransitionParcel(string trackingId, Parcel parcel)
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

                if (_repo.GetByTrackingId(trackingId) != null)
                    throw new ParcelAlreadyExistException("Parcel already exists with this tracking ID.");

                DataAccess.Entities.Parcel parcelDAL = _mapper.Map<DataAccess.Entities.Parcel>(parcel);
                parcelDAL.TrackingId = trackingId;
                parcelDAL.State = DataAccess.Entities.Parcel.StateEnum.PickupEnum;

                GeoCoordinate senderCoordinates = _agent.EncodeAddress(parcel.Sender).Result;
                GeoCoordinate recipientCoordinates = _agent.EncodeAddress(parcel.Recipient).Result;

                Point senderPoint = new Point(senderCoordinates.Lon, senderCoordinates.Lat);
                Point recipientPoint = new Point(recipientCoordinates.Lon, recipientCoordinates.Lat);

                _repo.Create(parcelDAL, senderPoint, recipientPoint);

                _logger.LogInformation($"Submitted parcel data to the logistics service with: {JsonConvert.SerializeObject(parcel, Formatting.Indented)}");
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
