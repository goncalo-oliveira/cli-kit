using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Faactory.RestClient;
using GitPak.GitHub;

namespace GitPak
{
    internal class GitHubClient
    {
        private readonly RestClient client;

        public GitHubClient()
        {
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add( 
                new MediaTypeWithQualityHeaderValue( "application/vnd.github.v3+json" ) );

            httpClient.DefaultRequestHeaders.UserAgent.Add( 
                new ProductInfoHeaderValue( "gitpak", GetType().Assembly.GetVersion() ) );

            client = new RestClient( httpClient
                , "https://api.github.com/"
                , new GitHubJsonSerializer() );
        }

        public async Task<Release> GetLatestReleaseAsync( string owner, string repo )
        {
            var response = await client.GetJsonAsync<Release>( $"repos/{owner}/{repo}/releases/latest" );

            if ( response.Content == null )
            {
                return ( null );
            }

            if ( !response.Content.PreRelease )
            {
                return response.Content;
            }

            // let's dig deeper into the release list
            var releases = await GetReleasesAsync( owner, repo );

            return ( releases.FirstOrDefault() );
        }

        public async Task<IEnumerable<Release>> GetReleasesAsync( string owner, string repo )
        {
            var response = await client.GetJsonAsync<IEnumerable<Release>>( $"repos/{owner}/{repo}/releases/latest" );

            if ( response.Content == null )
            {
                return Enumerable.Empty<Release>();
            }

            // return only stable releases
            return response.Content.Where( x => !x.PreRelease )
                .OrderByDescending( x => x.PublishedAt )
                .ToArray();
        }

        public async Task<Tag> GetLatestTagAsync( string owner, string repo, string pattern )
        {
            var url = $"repos/{owner}/{repo}/tags";

            var ghTags = new List<Tag>();

            var page = 1;
            while ( true )
            {
                var response = await client.GetJsonAsync<IEnumerable<Tag>>( $"{url}?page={page}" );

                if ( response.Content == null )
                {
                    break;
                }

                var matchingTags = response.Content.Where( x => x.Name.StartsWith( pattern ) )
                    // TODO: support more complext patterns; currently only [prefix] is supported
                    .Where( x => x.Name.Substring( pattern.Length ).All( c => "0123456789.".Contains( c ) ) )
                    .ToArray();

                if ( matchingTags.Any() )
                {
                    return matchingTags.First();
                }

                page++;
            }

            return ( null );
        }
    }
}
