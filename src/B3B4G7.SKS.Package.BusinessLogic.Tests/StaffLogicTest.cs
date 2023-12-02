using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Sql;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.Services.Helper;
using EntityFrameworkCore.Testing.Moq;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using B3B4G7.SKS.Package.WebhookManager.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using NUnit.Framework.Internal.Execution;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;

namespace B3B4G7.SKS.Package.BusinessLogic.Tests
{
    internal class StaffLogicTest
    {
        IMapper _mapper;
        ILogger<StaffLogic> _logger;

        DataAccess.Entities.Parcel _parcelDAL;

        Mock<IWarehouseRepository> _moqWRepo;
        Mock<IParcelRepository> _moqPRepo;
        Mock<IWebHookManager> _moqWHRepo;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var senderLogicLoggerFactory = new LoggerFactory();
            _logger = senderLogicLoggerFactory.CreateLogger<StaffLogic>();

            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z0-9]{9}$" });
            var randomTrackingId = randomizerTextRegex.Generate();

            var parcelBL = new BusinessLogic.Entities.Parcel()
            {
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

            _parcelDAL = _mapper.Map<DataAccess.Entities.Parcel>(parcelBL);

            var mockedDbContext = Create.MockedDbContextFor<TracknTraceContext>();
            _moqWRepo = new Mock<IWarehouseRepository>();
            _moqPRepo = new Mock<IParcelRepository>();
            _moqWHRepo = new Mock<IWebHookManager>();
        }


        [Test]
        public void ReportParcelDelivery_Success()
        {
            // Arrange

            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Act
            staffLogic.ReportParcelDelivery(_parcelDAL.TrackingId, string.Empty);

            //Assert
            _moqPRepo.Verify(x => x.UpdateAllListHops(_parcelDAL), Times.Once);
            _moqPRepo.Verify(x => x.UpdateState(_parcelDAL, DataAccess.Entities.Parcel.StateEnum.DeliveredEnum), Times.Once);
            _moqWHRepo.Verify(x => x.UnsubscribeParcelWebhook(_parcelDAL.TrackingId, string.Empty), Times.Once);
        }

        [Test]
        public void ReportParcelDelivery_ParcelNotExistException()
        {
            // Arrange
            _parcelDAL = new DataAccess.Entities.Parcel();
            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<ParcelNotExistException>(() => staffLogic.ReportParcelDelivery(_parcelDAL.TrackingId, string.Empty));
        }

        [Test]
        public void ReportParcelDelivery_InvalidParcelException()
        {
            // Arrange
            _parcelDAL = new DataAccess.Entities.Parcel();
            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<InvalidObjectException>(() => staffLogic.ReportParcelDelivery(_parcelDAL.TrackingId, string.Empty));
        }

        [Test]
        public void ReportParcelHop_HopTypeTruck()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();
            DataAccess.Entities.Parcel.StateEnum expectedEnum = DataAccess.Entities.Parcel.StateEnum.InTruckDeliveryEnum;

            var hopBL = new BusinessLogic.Entities.Hop()
            {
                HopType = "truck",
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() {  Lat = 23.0, Lon = 72.7 }
            };
            var hopToDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);


            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            _moqWRepo.Setup(m => m.GetByCode(hopToDAL.Code)).Returns(hopToDAL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Act
            staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty);

            //Assert
            _moqPRepo.Verify(x => x.UpdateListHops(_parcelDAL, randomHopCode), Times.Once);
            _moqPRepo.Verify(x => x.UpdateState(_parcelDAL, expectedEnum), Times.Once);
            _moqWHRepo.Verify(x => x.SubscribeParcelWebhook(_parcelDAL.TrackingId, string.Empty), Times.Once);
        }

        [Test]
        public void ReportParcelHop_HopTypeWarehouse()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();
            DataAccess.Entities.Parcel.StateEnum expectedEnum = DataAccess.Entities.Parcel.StateEnum.InTransportEnum;

            var hopBL = new BusinessLogic.Entities.Hop()
            {
                HopType = "warehouse",
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() { Lat = 23.0, Lon = 72.7 }
            };
            var hopToDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);


            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            _moqWRepo.Setup(m => m.GetByCode(hopToDAL.Code)).Returns(hopToDAL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Act
            staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty);

            //Assert
            _moqPRepo.Verify(x => x.UpdateListHops(_parcelDAL, randomHopCode), Times.Once);
            _moqPRepo.Verify(x => x.UpdateState(_parcelDAL, expectedEnum), Times.Once);
            _moqWHRepo.Verify(x => x.SubscribeParcelWebhook(_parcelDAL.TrackingId, string.Empty), Times.Once);
        }

        [Test]
        public void ReportParcelHop_HopTypeTransferwarehouse()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();
            DataAccess.Entities.Parcel.StateEnum expectedEnum = DataAccess.Entities.Parcel.StateEnum.TransferredEnum;

            var hopBL = new BusinessLogic.Entities.Hop()
            {
                HopType = "transferwarehouse",
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() { Lat = 23.0, Lon = 72.7 }
            };
            var hopToDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);


            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            _moqWRepo.Setup(m => m.GetByCode(hopToDAL.Code)).Returns(hopToDAL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Act
            staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty);

            //Assert
            _moqPRepo.Verify(x => x.UpdateListHops(_parcelDAL, randomHopCode), Times.Once);
            _moqPRepo.Verify(x => x.UpdateState(_parcelDAL, expectedEnum), Times.Once);
            _moqWHRepo.Verify(x => x.SubscribeParcelWebhook(_parcelDAL.TrackingId, string.Empty), Times.Once);
        }

        [Test]
        public void ReportParcelHop_ParcelNotExistException()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();

            _parcelDAL = new DataAccess.Entities.Parcel();

            var hopBL = new Hop()
            {
                HopType = "transferwarehouse",
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() { Lat = 23.0, Lon = 72.7 }
            };
            var hopToDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<ParcelNotExistException>(() => staffLogic.ReportParcelHop(_parcelDAL.TrackingId, string.Empty, string.Empty));
        }

        [Test]
        public void ReportParcelHop_HopsNotExistException()
        {
            // Arrange
            _parcelDAL = new DataAccess.Entities.Parcel();
            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);

            var hopToDAL = new DataAccess.Entities.Hop();
            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<HopsNotExistException>(() => staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty));
        }

        [Test]
        public void ReportParcelHop_InvalidParcelException()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();

            _parcelDAL = new DataAccess.Entities.Parcel();
            var hopBL = new BusinessLogic.Entities.Hop()
            {
                HopType = "transferwarehouse",
                Code = randomHopCode,
                LocationCoordinates = new GeoCoordinate() { Lat = 23.0, Lon = 72.7 }
            };
            var hopToDAL = _mapper.Map<DataAccess.Entities.Hop>(hopBL);

            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            _moqWRepo.Setup(m => m.GetByCode(hopToDAL.Code)).Returns(hopToDAL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<InvalidObjectException>(() => staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty));
        }

        [Test]
        public void ReportParcelHop_InvalidHopException()
        {
            // Arrange
            var randomizerTextRegex2 = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[A-Z]{4}\d{1,4}$" });
            string randomHopCode = randomizerTextRegex2.Generate();

            var hopToDAL = new DataAccess.Entities.Hop();

            _moqPRepo.Setup(m => m.GetByTrackingId(_parcelDAL.TrackingId)).Returns(_parcelDAL);
            _moqWRepo.Setup(m => m.GetByCode(hopToDAL.Code)).Returns(hopToDAL);

            IStaffLogic staffLogic = new StaffLogic(_mapper, _moqWRepo.Object, _moqPRepo.Object, _logger, _moqWHRepo.Object);

            //Assert
            Assert.Throws<InvalidObjectException>(() => staffLogic.ReportParcelHop(_parcelDAL.TrackingId, hopToDAL.Code, string.Empty));
        }
    }
}
