using B3B4G7.SKS.Package.DataAccess.Sql;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Entities;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace B3B4G7.SKS.Package.DataAccess.Tests
{
    internal class SqlParcelRepositoryTest
    {
        ILogger<SqlParcelRepository> _pLogger;
        ILogger<SqlWarehouseRepository> _wLogger;
        IParcelRepository parcelRepository;
        TracknTraceContext mockedDbContext;

        [SetUp]
        public void Setup()
        {
            _pLogger = new LoggerFactory().CreateLogger<SqlParcelRepository>();
            _wLogger = new LoggerFactory().CreateLogger<SqlWarehouseRepository>();

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

            mockedDbContext = Create.MockedDbContextFor<TracknTraceContext>();
            IWarehouseRepository warehouseRepository = new SqlWarehouseRepository(mockedDbContext, _wLogger);

            //Act
            warehouseRepository.Create(warehouseTestEntity);
        }

        [Test]
        public void Create_Test()
        {
            //Arrange
            var parcelTestEntity = new Parcel
            {
                Weight = 1000,
                State = Parcel.StateEnum.PickupEnum,
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>()
            };

            parcelTestEntity.TrackingId = "PYJRB4HZ6";
            parcelRepository = new SqlParcelRepository(mockedDbContext, _pLogger);

            //Act
            parcelRepository.Create(parcelTestEntity, new Point(3, 4), new Point(5, 6));

            //Assert
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => mockedDbContext.Parcels.First());
                Assert.AreEqual(mockedDbContext.Parcels.Find("PYJRB4HZ6").Weight, parcelTestEntity.Weight);
            });
        }
    }
}
