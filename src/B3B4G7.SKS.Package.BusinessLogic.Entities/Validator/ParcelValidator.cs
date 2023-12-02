using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class ParcelValidator : AbstractValidator<Parcel>
    {
        public ParcelValidator()
        {
            RuleFor(p => p.Weight).GreaterThan(0);
            RuleFor(p => p.Recipient).NotNull();
            RuleFor(p => p.Sender).NotNull();
            RuleFor(p => p.TrackingId).Matches(@"^[A-Z0-9]{9}$");
            RuleForEach(p => p.VisitedHops).NotNull();
            RuleForEach(p => p.FutureHops).NotNull();
        }
    }
}
