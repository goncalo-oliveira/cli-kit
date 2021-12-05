using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faactory.Collections;
using Faactory.RestClient;
using GitPak.GoogleStorage;

namespace GitPak
{
    internal static class PAckageGoogleStorageExtensions
    {
        public static async Task<string> GoogleStorageGetLatestVersionAsync( this Package package )
        {
            var plugin = package.Plugins?.GetValueOrDefault( "google.storageapis" );

            if ( plugin == null )
            {
                return ( null );
            }

            var bucket = plugin.GetValueOrDefault( "bucket" );
            var prefix = plugin.GetValueOrDefault( "prefix" );
            var fields = plugin.GetValueOrDefault( "fields" );

            if ( string.IsNullOrEmpty( bucket )
                ||
                string.IsNullOrEmpty( prefix ) )
            {
                return ( null );
            }

            var client = new RestClient( new System.Net.Http.HttpClient(), "https://storage.googleapis.com/storage/v1" );

            var response = await client.Configure( $"b/{bucket}/o", options =>
            {
                options.QueryParameters.Add( "prefix", prefix );
                options.QueryParameters.Add( "fields", "items(name,timeCreated)" );
            } )
            .GetJsonAsync<StorageResult<StorageObject>>();

            if ( response.Content == null )
            {
                return ( null );
            }

            var latest = response.Content.Items.OrderByDescending( x => x.TimeCreated )
                .First();

            var semver = latest.Name.Substring( prefix.Length );

            int idx;
            for ( idx = 0; idx < semver.Length; idx++ )
            {
                if ( !"0123456789.".Contains( semver[idx] ) )
                {
                    var version = semver.Substring( 0, idx );

                    if ( version.EndsWith( '.' ) )
                    {
                        version = string.Concat( version, "0" );
                    }
                    
                    return ( version );
                }
            }

            return ( semver );
        }
    }
}
