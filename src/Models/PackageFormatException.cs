using System;

namespace CliKit
{
    public class PackageFormatException : Exception
    {
        public PackageFormatException( string message, Exception innerException = null )
        : base( message, innerException )
        {}
    }
}
