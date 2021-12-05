using System;
using System.IO;

namespace GitPak
{
    internal static class PathHelper
    {
        public static string GetTargetPath()
            => Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal )
                , ".gitpak/bin" );

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
