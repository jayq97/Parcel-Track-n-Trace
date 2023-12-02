using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.ServiceAgents.Interfaces;
using B3B4G7.SKS.Package.Services.Helper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Threading.Tasks;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.BusinessLogic.Tests
{
    public class SenderLogicTest
    {
        IMapper _mapper;
        ILogger<SenderLogic> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var senderLogicLoggerFactory = new LoggerFactory();
            _logger = senderLogicLoggerFactory.CreateLogger<SenderLogic>();
        }

        [Test]
        public void SubmitParcel_Success()
        {
            // Arrange
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var parcelBL = new BusinessLogic.Entities.Parcel() { 
                Weight = 2000, 
                Recipient = new()
                {
                    Name = "string1",
                    Street = "string2",
                    PostalCode = "string3",
                    City = "string4",
                    Country = "string5",
                },
                Sender = new()
                {
                    Name = "string6",
                    Street = "string7",
                    PostalCode = "string8",
                    City = "string9",
                    Country = "string10",
                },
                TrackingId = randomTrackingId,
                State = Parcel.StateEnum.PickupEnum
            };

            var geoCoordinateSender = new GeoCoordinate() { Lat = 1, Lon = 2 };
            var geoCoordinateRecipient = new GeoCoordinate() { Lat = 3, Lon = 4 };

            var encodingAgentMock = new Mock<IGeoEncodingAgent>();
            encodingAgentMock.Setup(m => m.EncodeAddress(parcelBL.Sender)).Returns(Task.FromResult(geoCoordinateSender));
            encodingAgentMock.Setup(m => m.EncodeAddress(parcelBL.Recipient)).Returns(Task.FromResult(geoCoordinateRecipient));
            IGeoEncodingAgent encodingAgent = encodingAgentMock.Object;

            var parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(x => x.GetByTrackingId(randomTrackingId)).Returns(_mapper.Map<DataAccess.Entities.Parcel>(parcelBL));
            IParcelRepository parcelRepo = parcelRepoMock.Object;

            ISenderLogic senderLogic = new SenderLogic(_mapper, parcelRepo, _logger, encodingAgent);

            // Act
            var newTrackingId = senderLogic.SubmitParcel(parcelBL);

            // Assert
            parcelRepoMock.Verify(x => x.Create(
                It.IsAny<DataAccess.Entities.Parcel>(),
                It.IsAny<NetTopologySuite.Geometries.Point>(),
                It.IsAny<NetTopologySuite.Geometries.Point>()), Times.Once);

            Assert.AreEqual(randomTrackingId, parcelBL.TrackingId);
            Assert.AreEqual(_mapper.Map<Parcel>(parcelRepo.GetByTrackingId(randomTrackingId)).Weight, parcelBL.Weight);
        }

        [Test]
        public void SubmitParcel_InvalidParcelException()
        {
            // Arrange
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var parcelBL = new BusinessLogic.Entities.Parcel()
            {
                Weight = 0,
                TrackingId = randomTrackingId,
                State = Parcel.StateEnum.PickupEnum
            };

            var geoCoordinateSender = new GeoCoordinate() { Lat = 1, Lon = 2 };
            var geoCoordinateRecipient = new GeoCoordinate() { Lat = 3, Lon = 4 };

            var encodingAgentMock = new Mock<IGeoEncodingAgent>();
            IGeoEncodingAgent encodingAgent = encodingAgentMock.Object;

            var parcelRepoMock = new Mock<IParcelRepository>();
            IParcelRepository parcelRepo = parcelRepoMock.Object;

            ISenderLogic senderLogic = new SenderLogic(_mapper, parcelRepo, _logger, encodingAgent);

            // Assert
            Assert.Throws<InvalidObjectException>(() => senderLogic.SubmitParcel(parcelBL));
        }
    }
}