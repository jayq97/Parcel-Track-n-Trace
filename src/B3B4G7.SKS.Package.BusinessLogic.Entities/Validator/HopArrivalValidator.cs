using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class HopArrivalValidator : AbstractValidator<HopArrival>
    {
        public HopArrivalValidator()
        {
            RuleFor(ha => ha.Code).Matches(@"^[A-Z]{4}\d{1,4}$");
        }
    }
}
