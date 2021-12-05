using System;

namespace GitPak
{
    public class PackageFormatException : Exception
    {
        public PackageFormatException( string message, Exception innerException = null )
        : base( message, innerException )
        {}
    }
}
