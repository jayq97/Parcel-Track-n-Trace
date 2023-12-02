using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class WarehouseNextHopsValidator : AbstractValidator<WarehouseNextHops>
    {
        public WarehouseNextHopsValidator()
        {
            RuleFor(r => r.Hop).NotNull();
        }
    }
}
