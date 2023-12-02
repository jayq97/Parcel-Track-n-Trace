using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.Services.Helper;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Collections.Generic;

namespace B3B4G7.SKS.Package.BusinessLogic.Tests
{
    internal class WarehouseManagementLogicTest
    {
        IMapper _mapper;
        ILogger<WarehouseManagementLogic> _logger;

        DataAccess.Entities.Parcel _parcelDAL;

        Mock<IWarehouseRepository> _moqWRepo;
        IWarehouseManagementLogic _warehouseLogic;
        DataAccess.Entities.Warehouse _warehouseDAL;
        BusinessLogic.Entities.Warehouse _warehouseBL;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<WarehouseManagementLogic>();

            var mockedDbContext = Create.MockedDbContextFor<TracknTraceContext>();
            _moqWRepo = new Mock<IWarehouseRepository>();
            _warehouseLogic = new WarehouseManagementLogic(_mapper, _moqWRepo.Object, _logger);

            // Arrange
            _warehouseBL = new BusinessLogic.Entities.Warehouse()
            {
                Level = 0,
                NextHops = new List<WarehouseNextHops>() {
                    new WarehouseNextHops() {
                        Hop = new Hop() { Code = "WENA04", Description = "description", HopType = "hop", LocationCoordinates = new GeoCoordinate() { Lat = 48.2083537, Lon = 48.2083537 }, LocationName = "sdsd", ProcessingDelayMins = 2 }
                    },
                    new WarehouseNextHops() {
                        Hop = new Hop() { Code = "WBNA04", Description = "description2", HopType = "hip", LocationCoordinates = new GeoCoordinate() { Lat = 49.2083537, Lon = 48.2083537 }, LocationName = "sdsd", ProcessingDelayMins = 5 }
                    }
                },
                HopType = "Warehouse",
                Code = "WECA07",
                ProcessingDelayMins = 3,
                LocationName = "NumberOne",
                Description = "Root Warehouse - Österreich",
                LocationCoordinates = new GeoCoordinate() { Lat = 50.2083537, Lon = 50.2083537 }

            };
            _warehouseDAL = _mapper.Map<DataAccess.Entities.Warehouse>(_warehouseBL);
        }

        [Test]
        public void ExportWarehouses_Success()
        {
            _moqWRepo.Setup(m => m.GetWarehouses()).Returns(_warehouseDAL);

            //Act
            var result = _warehouseLogic.ExportWarehouses();

            //Assert
            _moqWRepo.Verify(x => x.GetWarehouses(), Times.Once);
            Assert.AreEqual(result.Code, _warehouseBL.Code);
        }

        [Test]
        public void GetHop_Success()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();

            BusinessLogic.Entities.Hop hopBL = new BusinessLogic.Entities.Hop()
            {
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() { Lat = 48.2083537, Lon = 48.2083537 },
                Description = "super duper",
                HopType = "hipedihopedi",
                ProcessingDelayMins = 3,
                LocationName = "Mega Warehouse"
            };

            DataAccess.Entities.Hop hopDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);
            _moqWRepo.Setup(m => m.GetByCode(randomHopCode)).Returns(hopDAL);

            // Act
            var result = _warehouseLogic.GetHop(randomHopCode);

            // Assert
            _moqWRepo.Verify(x => x.GetByCode(randomHopCode), Times.Once);
            Assert.AreEqual(result.Code, randomHopCode);
        }

        [Test]
        public void ImportWarehouses_Success()
        {
            // Act
            _warehouseLogic.ImportWarehouses(_warehouseBL);

            // Assert
            _moqWRepo.Verify(x => x.Create(It.IsAny<DataAccess.Entities.Warehouse>()), Times.Once);
            _moqWRepo.Verify(x => x.ClearDatabase(), Times.Once);
        }
    }
}