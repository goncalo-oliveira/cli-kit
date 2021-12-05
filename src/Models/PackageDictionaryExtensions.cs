using System;
using System.Collections;
using System.Collections.Generic;

namespace GitPak
{
    internal static class PackageDictionaryExtensions
    {
        public static Package GetPackage( this IDictionary<string, Package> source, string name )
        {
            if ( !source.TryGetValue( name, out var package ) )
            {
                return ( null );
            }

            package.FileName = name;

            if ( string.IsNullOrEmpty( package.Name ) )
            {
                package.Name = name;
            }

            return ( package );
        }
    }
}
