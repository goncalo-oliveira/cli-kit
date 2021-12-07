using System;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CliKit
{
    internal class Config
    {
        public string Version { get; set; }
        public string DownloadPath { get; set; } // $HOME/.clikit/bin

        internal static async Task<Config> LoadAsync()
        {
            var filepath = System.IO.Path.Combine( PathHelper.GetHomePath(), "config.yaml" );
            var downloadPath = System.IO.Path.Combine( PathHelper.GetHomePath(), "bin" );

            if ( !System.IO.File.Exists( filepath ) )
            {
                return new Config
                {
                    Version = "0.1",
                    DownloadPath = downloadPath
                };
            }

            var yaml = await System.IO.File.ReadAllTextAsync( filepath, Encoding.UTF8 );

            var config = LoadFromString( yaml );

            if ( string.IsNullOrEmpty( config.DownloadPath ) )
            {
                config.DownloadPath = downloadPath;
            }

            return ( config );
        }

        private static Config LoadFromString( string yaml )
        {
            if ( string.IsNullOrEmpty( yaml ) )
            {
                return ( null );
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention( CamelCaseNamingConvention.Instance )
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<Config>( yaml );
        }
    }
}
