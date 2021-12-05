using System;
using System.Text;
using Faactory.RestClient;

namespace GitPak.GitHub
{
    internal class GitHubJsonSerializer : ISerializer
    {
        private static readonly System.Text.Json.JsonSerializerOptions serializerOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };

        public byte[] Serialize<T>( T value )
        {
            var json = System.Text.Json.JsonSerializer.Serialize( value, serializerOptions );

            return Encoding.UTF8.GetBytes( json );
        }
        
        public T Deserialize<T>( byte[] content )
            => System.Text.Json.JsonSerializer.Deserialize<T>( content, serializerOptions );
    }
}
