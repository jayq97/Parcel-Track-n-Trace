using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.ServiceAgents.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class PersonAddressNotFoundInAPIException : Exception
    {
        public PersonAddressNotFoundInAPIException(string message) : base(message)
        {
        }
    }
}
