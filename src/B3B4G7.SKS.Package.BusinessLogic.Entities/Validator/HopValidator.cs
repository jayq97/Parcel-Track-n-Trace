using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class HopValidator : AbstractValidator<Hop>
    {
        public HopValidator()
        {
            RuleFor(h => h.LocationCoordinates).NotNull();
        }
    }
}
