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
using System.Collections.Generic;

namespace B3B4G7.SKS.Package.Services.Tests
{
    public class WarehouseManagementApiTest
    {
        IMapper _mapper;
        ILogger<WarehouseManagementApiController> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<WarehouseManagementApiController>();
        }

        [Test]
        public void ExportWarehouses_AssertReturnedListWarehouseAndMappedToDTOWarehouse()
        {
            // Arrange   
            BusinessLogic.Entities.Warehouse warehouses = new BusinessLogic.Entities.Warehouse();

            var warehouseManagementLogicMock = new Mock<IWarehouseManagementLogic>();
            warehouseManagementLogicMock.Setup(m => m.ExportWarehouses()).Returns(warehouses);
            IWarehouseManagementLogic warehouseManagementLogic = warehouseManagementLogicMock.Object;

            var warehouseManagementApi = new WarehouseManagementApiController(_mapper, warehouseManagementLogic, _logger);

            // Act
            var actionResult = warehouseManagementApi.ExportWarehouses();
            dynamic objectResult = actionResult as ObjectResult;

            // Assert
            Assert.That(objectResult.Value is DTOs.Warehouse);
            Assert.AreEqual(warehouses.Level, objectResult.Value.Level);
        }

        [Test]
        public void GetWarehouse_AssertRandomTrackingIdAndMappedToNewParcelInfo()
        {
            // Arrange   
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomCode = randomizerTextRegex.Generate();

            BusinessLogic.Entities.Warehouse warehouse = new BusinessLogic.Entities.Warehouse
            {
                Code = randomCode
            };

            var warehouseManagementLogicMock = new Mock<IWarehouseManagementLogic>();
            warehouseManagementLogicMock.Setup(m => m.GetHop(randomCode)).Returns(warehouse);
            IWarehouseManagementLogic warehouseManagementLogic = warehouseManagementLogicMock.Object;

            var warehouseManagementApi = new WarehouseManagementApiController(_mapper, warehouseManagementLogic, _logger);

            // Act
            var actionResult = warehouseManagementApi.GetWarehouse(randomCode);
            dynamic objectResult = actionResult as ObjectResult;

            // Assert
            Assert.That(objectResult.Value is DTOs.Warehouse);
            Assert.AreEqual(randomCode, objectResult.Value.Code);
        }

        [Test]
        public void ImportWarehouse_VerifyOnce()
        {
            // Arrange   
            var warehouseManagementLogicMock = new Mock<IWarehouseManagementLogic>();
            IWarehouseManagementLogic warehouseManagementLogic = warehouseManagementLogicMock.Object;

            var warehouseManagementApi = new WarehouseManagementApiController(_mapper, warehouseManagementLogic, _logger);

            // Act
            warehouseManagementApi.ImportWarehouses(It.IsAny<DTOs.Warehouse>());

            // Assert
            warehouseManagementLogicMock.Verify(x => x.ImportWarehouses(It.IsAny<BusinessLogic.Entities.Warehouse>()), Times.Once);
        }
    }
}