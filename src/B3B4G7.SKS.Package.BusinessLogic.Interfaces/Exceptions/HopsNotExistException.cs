using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class HopsNotExistException : Exception
    {
        public HopsNotExistException(string message) : base(message)
        {
        }
    }
}
