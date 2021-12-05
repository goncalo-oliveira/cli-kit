using System;
using System.Runtime.InteropServices;

namespace GitPak
{
    internal static class OSInformation
    {
        public static string GetOS()
        {
            if ( IsLinux() )
            {
                return ( "linux" );
            }

            if ( IsDarwin() )
            {
                return ( "darwin" );
            }

            if ( IsWindows() )
            {
                return ( "windows" );
            }

            return ( "other" );
        }

        public static bool IsLinux()
            => RuntimeInformation.IsOSPlatform( OSPlatform.Linux );

        public static bool IsDarwin()
            => RuntimeInformation.IsOSPlatform( OSPlatform.OSX );

        public static bool IsWindows()
            => RuntimeInformation.IsOSPlatform( OSPlatform.Windows );

        public static string GetOSDescription()
            => RuntimeInformation.OSDescription;

        public static string GetOSArchitecture()
            => RuntimeInformation.OSArchitecture.ToString().ToLower();

        public static string GetOSTemplate()
            => $"{GetOS()}.{GetOSArchitecture()}";
    }
}
