using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidObjectException : Exception
    {
        public InvalidObjectException(string message) : base(message)
        {
        }
    }
}
