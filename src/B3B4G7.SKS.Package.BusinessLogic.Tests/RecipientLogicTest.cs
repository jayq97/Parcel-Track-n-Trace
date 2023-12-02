using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.Services.Helper;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;

namespace B3B4G7.SKS.Package.BusinessLogic.Tests
{
    public class RecipientLogicTest
    {
        IMapper _mapper;
        ILogger<RecipientLogic> _logger;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfiles()));
            _mapper = new Mapper(configuration);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<RecipientLogic>();
        }

        [Test]
        public void TrackParcel_Success()
        {
            // Arrange
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = "^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var person1 = Builder<DataAccess.Entities.Recipient>.CreateNew().Build();
            var person2 = Builder<DataAccess.Entities.Recipient>.CreateNew().Build();
            var parcelDAL = Builder<DataAccess.Entities.Parcel>.CreateNew()
                .With(x => x.TrackingId = randomTrackingId)
                .With(x => x.Recipient = person1)
                .With(x => x.Sender = person2)
                .Build();

            var parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(m => m.GetByTrackingId(randomTrackingId)).Returns(parcelDAL);
            IParcelRepository parcelRepo = parcelRepoMock.Object;

            IRecipientLogic parcelLogic = new RecipientLogic(_mapper, parcelRepo, _logger);

            // Act
            var parcelBL = parcelLogic.TrackParcel(randomTrackingId);

            // Assert
            parcelRepoMock.Verify(x => x.GetByTrackingId(randomTrackingId), Times.Once);
            Assert.AreEqual(parcelDAL.Weight, parcelBL.Weight);
            Assert.AreEqual(parcelDAL.State.ToString(), parcelBL.State.ToString());
            Assert.AreEqual(parcelDAL.Recipient.Name, parcelBL.Recipient.Name);
            Assert.AreEqual(parcelDAL.Sender.Name, parcelBL.Sender.Name);
        }

        [Test]
        public void TrackParcel_ParcelNotExistException()
        {
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = "^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var person1 = Builder<DataAccess.Entities.Recipient>.CreateNew().Build();
            var person2 = Builder<DataAccess.Entities.Recipient>.CreateNew().Build();
            var parcelDAL = Builder<DataAccess.Entities.Parcel>.CreateNew()
                .With(x => x.TrackingId = randomTrackingId)
                .With(x => x.Recipient = person1)
                .With(x => x.Sender = person2)
                .Build();

            var parcelRepoMock = new Mock<IParcelRepository>();
            IParcelRepository parcelRepo = parcelRepoMock.Object;

            IRecipientLogic parcelLogic = new RecipientLogic(_mapper, parcelRepo, _logger);

            Assert.Throws<ParcelNotExistException>(() => parcelLogic.TrackParcel(randomTrackingId));
        }

        [Test]
        public void TrackParcel_InvalidParcelException()
        {
            var randomizerTextRegex = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = "^[A-Z0-9]{9}$" });
            string randomTrackingId = randomizerTextRegex.Generate();

            var parcelDAL = Builder<DataAccess.Entities.Parcel>.CreateNew()
                .With(x => x.TrackingId = randomTrackingId)
                .Build();

            var parcelRepoMock = new Mock<IParcelRepository>();
            parcelRepoMock.Setup(m => m.GetByTrackingId(randomTrackingId)).Returns(parcelDAL);
            IParcelRepository parcelRepo = parcelRepoMock.Object;

            IRecipientLogic parcelLogic = new RecipientLogic(_mapper, parcelRepo, _logger);

            Assert.Throws<InvalidObjectException>(() => parcelLogic.TrackParcel(randomTrackingId));
        }
    }
}