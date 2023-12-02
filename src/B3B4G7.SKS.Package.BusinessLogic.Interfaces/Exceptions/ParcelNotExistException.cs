using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ParcelNotExistException : Exception
    {
        public ParcelNotExistException(string message) : base(message)
        {
        }
    }
}
