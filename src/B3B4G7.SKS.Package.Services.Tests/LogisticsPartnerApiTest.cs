using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Sql;
using B3B4G7.SKS.Package.Services.Controllers;
using B3B4G7.SKS.Package.Services.Helper;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace B3B4G7.SKS.Package.Services.Tests
{
    public class LogisticsPartnerApiTest
    {
        IMapper _mapper;
        ILogger<LogisticsPartnerApiController> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<LogisticsPartnerApiController>();
        }

        [Test]
        public void TransitionParcel_AssertRandomTrackingIdAndMappedToNewParcelInfo()
        {
            // Arrange   
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var logisticsPartnerLogicMock = new Mock<ILogisticsPartnerLogic>();
            logisticsPartnerLogicMock.Setup(m => m.TransitionParcel(randomTrackingId, It.IsAny<BusinessLogic.Entities.Parcel>())).Returns(randomTrackingId);
            ILogisticsPartnerLogic logisticsPartnerLogic = logisticsPartnerLogicMock.Object;

            var logisticsPartnerApi = new LogisticsPartnerApiController(_mapper, logisticsPartnerLogic, _logger);
            var parcel = Builder<DTOs.Parcel>.CreateNew().Build();

            // Act
            var actionResult = logisticsPartnerApi.TransitionParcel(randomTrackingId, parcel);
            dynamic objectResult = actionResult as ObjectResult;

            // Assert
            Assert.That(objectResult.Value is DTOs.NewParcelInfo);
            Assert.AreEqual(randomTrackingId, objectResult.Value.TrackingId);
        }
    }
}