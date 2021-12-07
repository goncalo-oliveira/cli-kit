using System;
using System.Threading.Tasks;

namespace CliKit
{
    internal static class PackageGitHubExtensions
    {
        private static Lazy<GitHubClient> ghClientFactory = new Lazy<GitHubClient>();
        private static GitHubClient Client => ghClientFactory.Value;

        public static async Task<string> GetLatestVersionFromGitHubAsync( this Package package )
        {
            // let's attempt to retrieve the latest release
            var latestVersion = await package.GetLatestVersionFromReleasesAsync();

            // if that doesn't work we'll attempt to retrieve the latest tag (when available)
            if ( ( latestVersion == null ) && ( !string.IsNullOrEmpty( package.Tag ) ) )
            {
                latestVersion = await package.GetLatestVersionFromTagsAsync();
            }

            return ( latestVersion );
        }

        private static async Task<string> GetLatestVersionFromReleasesAsync( this Package package )
        {
            var ghRelease = await Client.GetLatestReleaseAsync( package.Owner, package.Name );

            if ( ghRelease == null )
            {
                return ( null );
            }

            return ( ghRelease.TagName );
        }

        private static async Task<string> GetLatestVersionFromTagsAsync( this Package package )
        {
            if ( string.IsNullOrEmpty( package.Tag ) )
            {
                return ( null );
            }

            var tagTemplate = package.Tag.Replace( "$", package.GetSemVer() );

            var ghTag = await Client.GetLatestTagAsync( package.Owner, package.Name, tagTemplate );

            if ( ghTag == null )
            {
                return ( null );
            }

            return ghTag.Name.Substring( tagTemplate.Length );
        }
    }
}
