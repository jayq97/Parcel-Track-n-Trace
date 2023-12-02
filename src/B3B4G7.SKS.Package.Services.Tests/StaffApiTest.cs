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
    public class StaffApiTest
    {
        IMapper _mapper;
        ILogger<StaffApiController> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<StaffApiController>();
        }

        [Test]
        public void ReportParcelDelivery_VerifyOnce()
        {
            // Arrange   
            var staffLogicMock = new Mock<IStaffLogic>();
            IStaffLogic staffLogic = staffLogicMock.Object;

            var staffApi = new StaffApiController(_mapper, staffLogic, _logger, "Test");

            // Act
            staffApi.ReportParcelDelivery(string.Empty);

            // Assert
            staffLogicMock.Verify(x => x.ReportParcelDelivery(string.Empty, staffApi.BaseUrl), Times.Once);
        }

        [Test]
        public void ReportParcelHop_VerifyOnce()
        {
            // Arrange   
            var staffLogicMock = new Mock<IStaffLogic>();
            IStaffLogic staffLogic = staffLogicMock.Object;

            var staffApi = new StaffApiController(_mapper, staffLogic, _logger, "Test");

            // Act
            staffApi.ReportParcelHop(string.Empty, string.Empty);

            // Assert
            staffLogicMock.Verify(x => x.ReportParcelHop(string.Empty, string.Empty, staffApi.BaseUrl), Times.Once);
        }
    }
}