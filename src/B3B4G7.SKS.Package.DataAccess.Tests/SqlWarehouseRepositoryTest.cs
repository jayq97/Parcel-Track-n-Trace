using B3B4G7.SKS.Package.DataAccess.Sql;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Entities;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace B3B4G7.SKS.Package.DataAccess.Tests
{
    internal class SqlWarehouseRepositoryTest
    {
        ILogger<SqlWarehouseRepository> _logger;

        [SetUp]
        public void Setup()
        {
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<SqlWarehouseRepository>();
        }

        [Test]
        public void Create_Test()
        {
            //Arrange
            var serializer = GeoJsonSerializer.CreateDefault();
            var feature = (Feature)serializer.Deserialize(new StringReader("{\"type\":\"Feature\"," +
                "\"geometry\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[0,0]," +
                "[0,10],[10,10],[10,0],[0,0]]]]}}"), typeof(Feature));
            var geometry = feature.Geometry;

            var warehouseTestEntity = new Warehouse
            {
                Code = "772786",
                Description = "byebye",
                HopType = "Haus",
                Level = 2,
                LocationCoordinates = new Point(5, 6),
                LocationName = "Maus",
                NextHops = new List<WarehouseNextHops>()
                {
                    new()
                    {
                        WarehouseNextHopsId = 22355,
                        TraveltimeMins = 2,
                        Hop = new Warehouse
                        {
                            HopType = "warehouse",
                            Code = "789655",
                            Description = "ahasdha",
                            LocationCoordinates = new Point(3, 4),
                            LocationName = "abcde",
                            ProcessingDelayMins = 5,
                            NextHops = new List<WarehouseNextHops>()
                            {
                                new()
                                {
                                    WarehouseNextHopsId = 13355,
                                    TraveltimeMins = 2,
                                    Hop = new Truck
                                    {
                                        HopType = "truck",
                                        Code = "558898",
                                        Description = "ahasdha",
                                        LocationCoordinates = new Point(1, 2),
                                        LocationName = "hallo",
                                        ProcessingDelayMins = 3,
                                        NumberPlate = "W-747200",
                                        Region = geometry
                                    }
                                },
                            }
                        }
                    }
                }
            };
            var mockedDbContext = Create.MockedDbContextFor<TracknTraceContext>();
            IWarehouseRepository parcelRepository = new SqlWarehouseRepository(mockedDbContext, _logger);

            //Act
            parcelRepository.Create(warehouseTestEntity);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => mockedDbContext.Warehouses.First());
                Assert.AreEqual(mockedDbContext.Warehouses.Find("772786").LocationName, warehouseTestEntity.LocationName);
            });
        }
    }
}
