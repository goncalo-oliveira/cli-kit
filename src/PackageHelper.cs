using System;
using System.Threading.Tasks;

namespace CliKit
{
    internal static class PackageHelper
    {
        public static async Task<PackageSource> LoadAsync( string source = null )
        {
            if ( source == null )
            {
                #if DEBUG
                source = "packages.yaml";
                #else
                source = "https://raw.githubusercontent.com/goncalo-oliveira/cli-kit/main/packages.yaml";
                #endif
            }

            // read from url
            if ( source.StartsWith( "https://" ) || source.StartsWith( "http://" ) )
            {
                try
                {
                    return await PackageSource.LoadFromUrlAsync( source );
                }
                catch ( System.Net.Http.HttpRequestException ex )
                {
                    string statusCode = ex.StatusCode.HasValue
                        ? ( (int)ex.StatusCode ).ToString()
                        : string.Empty;

                    Console.WriteLine( "Failed to load package source from URL." );
                    Console.WriteLine( $"GET {source}  {statusCode}" );

                    if ( ex.StatusCode == null )
                    {
                        Console.WriteLine( ex.Message );
                    }

                    return ( null );
                }
            }

            // read from file
            var packageSource = await PackageSource.LoadFromFileAsync( source );

            if ( packageSource == null )
            {
                Console.WriteLine( $"Failed to load package source from '{source}' file." );
            }

            return ( packageSource );
        }
    }
}
