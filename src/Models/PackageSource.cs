using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Faactory.Collections;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CliKit
{
    public class PackageSource
    {
        public string Version { get; set; }
        public string Maintainer { get; set; }
        public Dictionary<Package> Packages { get; set; }

        internal static PackageSource LoadFromString( string yaml )
        {
            if ( string.IsNullOrEmpty( yaml ) )
            {
                return ( null );
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention( CamelCaseNamingConvention.Instance )
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<PackageSource>( yaml );
        }

        internal static async Task<PackageSource> LoadFromFileAsync( string filepath )
        {
            var yaml = await System.IO.File.ReadAllTextAsync( filepath, Encoding.UTF8 );

            return LoadFromString( yaml );
        }

        internal static async Task<PackageSource> LoadFromUrlAsync( string url )
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync( url );

            if ( response.StatusCode != System.Net.HttpStatusCode.OK )
            {
                throw new HttpRequestException( response.ReasonPhrase, null, response.StatusCode );
            }

            var yaml = await response.Content.ReadAsStringAsync();

            return LoadFromString( yaml );
        }
    }
}
