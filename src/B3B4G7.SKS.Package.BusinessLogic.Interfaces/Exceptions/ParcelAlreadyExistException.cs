using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ParcelAlreadyExistException : Exception
    {
        public ParcelAlreadyExistException(string message) : base(message)
        {
        }
    }
}
