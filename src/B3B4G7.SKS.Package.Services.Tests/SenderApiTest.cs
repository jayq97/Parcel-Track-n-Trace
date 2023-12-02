using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
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
    public class SenderApiTest
    {
        IMapper _mapper;
        ILogger<SenderApiController> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<SenderApiController>();
        }

        [Test]
        public void SubmitParcel_AssertRandomTrackingIdAndMappedToNewParcelInfo()
        {
            // Arrange   
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var senderLogicMock = new Mock<ISenderLogic>();
            senderLogicMock.Setup(m => m.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>())).Returns(randomTrackingId);
            ISenderLogic senderLogic = senderLogicMock.Object;

            var senderApi = new SenderApiController(_mapper, senderLogic, _logger);
            var parcel = Builder<DTOs.Parcel>.CreateNew().Build();

            // Act
            var actionResult = senderApi.SubmitParcel(parcel);
            dynamic objectResult = actionResult as ObjectResult;

            // Assert
            Assert.That(objectResult.Value is DTOs.NewParcelInfo);
            Assert.AreEqual(randomTrackingId, objectResult.Value.TrackingId);
        }
    }
}