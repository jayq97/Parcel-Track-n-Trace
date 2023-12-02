using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.Services.Controllers;
using B3B4G7.SKS.Package.Services.Helper;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace B3B4G7.SKS.Package.Services.Tests
{
    public class RecipientApiTest
    {
        IMapper _mapper;
        ILogger<RecipientApiController> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<RecipientApiController>();
        }

        [Test]
        public void TrackParcel_AssertReturnedParcelAndMappedToTrackingInformation()
        {
            // Arrange
            string trackingId = "ABCD2EFGH";
            var expectedParcelBL = new Parcel
            {
                State = Parcel.StateEnum.PickupEnum,
                VisitedHops = new List<HopArrival>()
                {
                    new() { Code = "code1", Description = "description1", DateTime = DateTime.Now },
                    new() { Code = "code2", Description = "description2", DateTime = DateTime.Now },
                },
                FutureHops = new List<HopArrival>()
                {
                    new() { Code = "code3", Description = "description3", DateTime = DateTime.Now },
                    new() { Code = "code4", Description = "description4", DateTime = DateTime.Now },
                }
            };

            var recipientLogicMock = new Mock<IRecipientLogic>();
            recipientLogicMock.Setup(m => m.TrackParcel(trackingId)).Returns(expectedParcelBL);
            IRecipientLogic recipientLogic = recipientLogicMock.Object;

            var recipientApi = new RecipientApiController(_mapper, recipientLogic, _logger);

            // Act
            var actionResult = recipientApi.TrackParcel(trackingId);
            dynamic objectResult = actionResult as ObjectResult;

            // Assert
            Assert.That(objectResult.Value is DTOs.TrackingInformation);
            Assert.AreEqual(expectedParcelBL.State.ToString(), objectResult.Value.State.ToString());
            Assert.AreEqual(expectedParcelBL.VisitedHops[0].Code, objectResult.Value.VisitedHops[0].Code);
            Assert.AreEqual(expectedParcelBL.VisitedHops[0].Description, objectResult.Value.VisitedHops[0].Description);
            Assert.AreEqual(expectedParcelBL.FutureHops[0].Code, objectResult.Value.FutureHops[0].Code);
            Assert.AreEqual(expectedParcelBL.FutureHops[0].Description, objectResult.Value.FutureHops[0].Description);
        }
    }
}