using System;
using System.IO;

namespace CliKit
{
    internal static class PathHelper
    {
        public static string GetTargetPath()
            => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal )
                , ".clikit/bin" );

        public static string GetHomePath()
            => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal )
                , ".clikit/" );

        public static void EnsureTargetPathExists()
        {
            var path = GetTargetPath();

            if ( !Directory.Exists( path ) )
            {
                Directory.CreateDirectory( path );
            }
        }
    }
}
