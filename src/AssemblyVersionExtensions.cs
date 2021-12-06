using System;
using System.Reflection;

namespace CliKit
{
    internal static class AssemblyVersionExtensions
    {
        public static string GetVersion( this Assembly assembly )
        {
            var version = Assembly.GetExecutingAssembly()
                .GetName().Version?.ToString( 3 ) ?? "0";

            if ( version.EndsWith( ".0" ) )
            {
                // trim revision number if zero
                version = version.Substring( 0, version.Length - 2 );
            }

            return ( version );
        }
    }
}
