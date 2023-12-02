using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Entities.Validator
{
    [ExcludeFromCodeCoverage]
    public class RecipientValidator : AbstractValidator<Recipient>
    {
        public RecipientValidator()
        {
            string postalCodeRegEx = @"^(A-)(\d{4})$";
            string streetRegEx = @"^([A-ZÄÖÜ][a-zäöüß]+\s[1-9][0-9]*)[a-z]?((\/[1-9][0-9]*)[a-z]?)*$";
            string cityAndNameRegEx = @"^([A-ZÄÖÜ][a-zäöüß]+)((\s|\-)[A-ZÄÖÜ][a-zäöüß]+)*$";

            When(r => r.Country == "Austria", () =>
            {
                RuleFor(r => r.PostalCode).Matches(postalCodeRegEx);
                RuleFor(r => r.Street).Matches(streetRegEx);
                RuleFor(r => r.City).Matches(cityAndNameRegEx);
                RuleFor(r => r.Name).Matches(cityAndNameRegEx);
            });

            When(r => r.Country == "Österreich", () =>
            {
                RuleFor(r => r.PostalCode).Matches(postalCodeRegEx);
                RuleFor(r => r.Street).Matches(streetRegEx);
                RuleFor(r => r.City).Matches(cityAndNameRegEx);
                RuleFor(r => r.Name).Matches(cityAndNameRegEx);
            });
        }
    }
}
