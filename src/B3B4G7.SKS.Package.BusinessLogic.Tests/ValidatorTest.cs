using AutoMapper;
using B3B4G7.SKS.Package.BusinessLogic.Entities;
using B3B4G7.SKS.Package.BusinessLogic.Entities.Validator;
using B3B4G7.SKS.Package.Services.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3B4G7.SKS.Package.BusinessLogic.Tests
{
    public class ValidatorTest
    {
        [Test]
        public void HopArrival_Validation()
        {
            // Arrange
            var validator = new HopArrivalValidator();

            var validHopArrival = new HopArrival { Code = "PHWE23" };
            var invalidHopArrival = new HopArrival { Code = "PH23" }; // no 4 Uppercase letters -> invalid

            // Act
            var validResult = validator.Validate(validHopArrival);
            var invalidResult = validator.Validate(invalidHopArrival);

            // Assert

            Assert.That(validResult.IsValid);
            Assert.That(!invalidResult.IsValid);
        }

        [Test]
        public void Hop_Validation()
        {
            // Arrange
            var validator = new HopValidator();

            var validHop = new Hop
            {
                LocationCoordinates = new GeoCoordinate
                {
                    Lat = 1.2,
                    Lon = 2.5
                },
            };

            var invalidHop = new Hop(); // empty Coordinates -> invalid

            // Act
            var validResult = validator.Validate(validHop);
            var invalidResult = validator.Validate(invalidHop);

            // Assert
            Assert.That(validResult.IsValid);
            Assert.That(!invalidResult.IsValid);
        }

        [Test]
        public void Parcel_Validation()
        {
            // Arrange
            var validator = new ParcelValidator();

            var validParcel = new Parcel
            {
                Weight = 1,
                Recipient = new Recipient(),
                Sender = new Recipient(),
                TrackingId = "DSAJPA32A",
                VisitedHops = new List<HopArrival>(),
                FutureHops = new List<HopArrival>(),
            };

            var invalidParcel = new Parcel // null Recipient, Sender, VisitedHops, FutureHops
            {
                Weight = -1, // negative weight -> invalid
                TrackingId = "DSAJPA3", // tracking id not string length 9 -> invalid
            };

            // Act
            var validResult = validator.Validate(validParcel);
            var invalidResult = validator.Validate(invalidParcel);

            // Assert

            Assert.That(validResult.IsValid);
            Assert.That(!invalidResult.IsValid);
        }

        [Test]
        public void Recipient_Validation()
        {
            // Arrange
            var validator = new RecipientValidator();

            var validRecipient = new Recipient
            {
                City = "Wien",
                Country = "Austria",
                Name = "Jay Stefan",
                Street = "Höchststädtplatz 6",
                PostalCode = "A-1200"
            };

            var invalidRecipient = new Recipient
            {
                City = "wien", // lowercase -> invalid
                Country = "Austria",
                Name = "jay stefan", // lowercase -> invalid
                Street = "höchststädtplatz 6", // lowercase -> invalid
                PostalCode = "1200" // without "A-" -> invalid
            };


            // Act
            var validResult = validator.Validate(validRecipient);
            var invalidResult = validator.Validate(invalidRecipient);

            // Assert

            // Weight > 0 and TrackingID has Length 9, but no Instances => invalid
            Assert.That(validResult.IsValid);
            Assert.That(!invalidResult.IsValid);
        }
    }
}
