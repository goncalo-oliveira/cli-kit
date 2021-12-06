using System;
using System.Text;
using System.Threading.Tasks;
using Faactory.Collections;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GitPak
{
    public class InstalledToolSource
    {
        public string Version { get; set; }
        public Dictionary<string> Tools { get; set; }

        internal Task WriteAsync()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention( CamelCaseNamingConvention.Instance )
                .Build();

            var yaml = serializer.Serialize( this );
            var filepath = System.IO.Path.Combine( PathHelper.GetHomePath(), "installed.yaml" );

            return System.IO.File.WriteAllTextAsync( filepath, yaml, new UTF8Encoding( false ) );
        }

        internal static InstalledToolSource LoadFromString( string yaml )
        {
            if ( string.IsNullOrEmpty( yaml ) )
            {
                return ( null );
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention( CamelCaseNamingConvention.Instance )
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<InstalledToolSource>( yaml );
        }

        internal static async Task<InstalledToolSource> LoadAsync()
        {
            var filepath = System.IO.Path.Combine( PathHelper.GetHomePath(), "installed.yaml" );

            if ( !System.IO.File.Exists( filepath ) )
            {
                return new InstalledToolSource
                {
                    Version = "0.1",
                    Tools = new Dictionary<string>()
                };
            }

            var yaml = await System.IO.File.ReadAllTextAsync( filepath, Encoding.UTF8 );

            return LoadFromString( yaml );
        }
    }
}
