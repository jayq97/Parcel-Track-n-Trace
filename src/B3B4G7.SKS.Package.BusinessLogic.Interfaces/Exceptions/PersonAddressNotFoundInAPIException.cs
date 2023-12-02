using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PersonAddressNotFoundException : Exception
    {
        public PersonAddressNotFoundException(string message) : base(message)
        {
        }
    }
}
