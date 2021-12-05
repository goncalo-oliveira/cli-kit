using System;
using System.Collections.Generic;
using System.Linq;

namespace GitPak
{
    internal static class PackageExtensions
    {
        private static string ParseVariables( this Package package, string input )
        {
            var parsed = input;

            if ( parsed.Contains( "${version}" ) )
            {
                parsed = parsed.Replace( "${version}", package.Version );
            }

            if ( parsed.Contains( "${sem-version}" ) )
            {
                parsed = parsed.Replace( "${sem-version}", package.GetSemVer() );
            }

            return ( parsed );
        }

        public static string GetDownloadUrl( this Package package )
        {
            string url;

            if ( package.Source.Equals( "github" ) )
            {
                url = string.Join( '/', new string[]
                {
                    package.Url,
                    package.Version,
                    GetPlatformTemplate( package )
                } );
            }
            else
            {
                url = string.Join( '/', new string[]
                {
                    package.Url,
                    GetPlatformTemplate( package )
                } );
            }

            return ParseVariables( package, url );
        }

        public static string GetPlatformTemplate( this Package package )
        {
            var platform = package.Platforms.GetValueOrDefault( OSInformation.GetOSTemplate() );

            if ( platform == null )
            {
                return ( null );
            }

            // handle version and sem-version
            platform = ParseVariables( package, platform );

            return ( platform );
        }

        public static string GetSemVer( this Package package )
        {
            if ( package.Version.Contains( '$' ) )
            {
                return ( null );
            }

            var semver = package.Version.Substring( 
                package.Version.IndexOfAny( "0123456789".ToCharArray() ) );

            int idx;
            for ( idx = 0; idx < semver.Length; idx++ )
            {
                if ( !"0123456789.".Contains( semver[idx] ) )
                {
                    return semver.Substring( 0, idx );
                }
            }

            return ( semver );
        }

        public static void SetVersion( this Package package, string version )
        {
            package.Version = package.Version.Replace( "$", version );
        }

        public static string GetTargetFileName( this Package package )
        {
            if ( OSInformation.IsWindows() )
            {
                return string.Concat( package.FileName, ".exe" );
            }

            return package.FileName;
        }

        public static string GetTargetFilePath( this Package package )
            => System.IO.Path.Combine( PathHelper.GetTargetPath(), GetTargetFileName( package ) );

        public static bool VerifyFormat( this Package package, bool throwOnError = false )
        {
            bool isValid = true;

            // the Source property is required
            isValid &= ThrowError(
                () => !string.IsNullOrEmpty( package.Source )
                    , "The 'Source' value is required."
                    , throwOnError );

            // Source can only be 'github' or 'url'
            isValid &= ThrowError(
                () => new string[] { "github", "url" }.Contains( package.Source )
                    , "The 'Source' value can only be 'github' or 'url'."
                    , throwOnError );

            // Source: gitgub
            if ( package.Source.Equals( "github" ) )
            {
                // the Owner property is required
                isValid &= ThrowError(
                    () => !string.IsNullOrEmpty( package.Owner )
                        , "The 'Owner' value is required when the 'Source' is 'github'."
                        , throwOnError );
            }

            // Source: url
            if ( package.Source.Equals( "url" ) )
            {
                // the Url property is required
                isValid &= ThrowError(
                    () => !string.IsNullOrEmpty( package.Url )
                        , "The 'Url' value is required when the 'Source' is 'url'."
                        , throwOnError );
            }

            // the Platforms property is required
            isValid &= ThrowError(
                () => package.Platforms?.Any() == true
                    , "The 'Platforms' value is required."
                    , throwOnError );

            if ( !isValid )
            {
                return ( false );
            }

            // if Source if 'github' we need to set the Url (unless manually set)
            if ( package.Source.Equals( "github" ) && string.IsNullOrEmpty( package.Url ) )
            {
                package.Url = $"https://github.com/{package.Owner}/{package.Name}/releases/download";
            }

            package.Url = package.Url.TrimEnd( '/' );

            return ( true );
        }

        private static bool ThrowError( Func<bool> validate, string message, bool throwOnError )
        {
            if ( validate() )
            {
                return ( true );
            }

            if ( throwOnError )
            {
                throw new PackageFormatException( message );
            }

            Console.WriteLine( "Package format is not valid! " + message );

            return ( false );
        }
    }
}
