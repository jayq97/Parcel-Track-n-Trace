using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class WarehouseValidator : AbstractValidator<Warehouse>
    {
        public WarehouseValidator()
        {
            RuleForEach(r => r.NextHops).NotNull();
            RuleFor(r => r.Description).Matches(@"^([A-ZÄÖÜa-zäöüß]+)((\s|\-)*[A-ZÄÖÜa-z0-9äöüß]+)*$");
        }
    }
}
