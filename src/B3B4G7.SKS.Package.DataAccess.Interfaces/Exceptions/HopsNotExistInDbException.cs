using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.DataAccess.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class HopsNotExistInDbException : Exception
    {
        public HopsNotExistInDbException(string message) : base(message)
        {
        }
    }
}
