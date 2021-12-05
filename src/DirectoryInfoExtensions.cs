using System;
using System.IO;

namespace GitPak
{
    internal static class DirectoryInfoExtensions
    {
        public static void Empty( this DirectoryInfo directory )
        {
            if ( !directory.Exists )
            {
                return;
            }

            foreach ( FileInfo file in directory.GetFiles() )
            {
                file.Delete();
            }

            foreach( DirectoryInfo subDirectory in directory.GetDirectories() )
            {
                subDirectory.Delete( true );
            }
        }
    }
}