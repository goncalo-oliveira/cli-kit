using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CliWrap;
using McMaster.Extensions.CommandLineUtils;

namespace CliKit
{
    [Command( Name = "add", Description = "Download a tool" )]
    public class AddCommand
    {
        [Option( "--source|-s", Description = "Custom package source location" )]
        public string Source { get; set; }

        [Option( "--version", Description = "Specific tool version to download" )]
        public string Version { get; set; }

        [Required]
        [Argument( 0, Name = "name", Description = "The name of the tool to download" )]
        public string Name { get; set; }

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            var packageSource = await PackageHelper.LoadAsync( Source );

            if ( packageSource == null )
            {
                return ( 1 );
            }

            // TODO: unless the name is '.' we install (or overwrite) a single tool

            // look for version alias (name:version or name@version)
            if ( Name.Contains( ':' ) || Name.Contains( '@' ) )
            {
                if ( !string.IsNullOrEmpty( Version ) )
                {
                    // adding version directly and by option! no can do...
                    Console.WriteLine( "Version number specified on the name but also with --version value!" );
                    Console.WriteLine( "Use only one of the methods." );
                    Console.WriteLine();

                    return ( 1 );
                }

                var idx = Name.IndexOfAny( new char[] { ':', '@' } );
                Version = Name.Substring( idx + 1 );

                Name = Name.Substring( 0, idx );
            }

            var package = packageSource.Packages.GetPackage( Name );

            if ( package == null )
            {
                Console.WriteLine( $"I'm sorry, I don't know how to download '{Name}' tool." );
                Console.WriteLine( "If you'd like to make a request or a contribution, visit us on GitHub" );
                Console.WriteLine( "https://github.com/goncalo-oliveira/cli-kit" );
                Console.WriteLine();

                return ( 1 );
            }

            return await InstallPackageAsync( package );
        }

        internal async Task<int> InstallPackageAsync( Package package )
        {
            if ( !package.VerifyFormat() )
            {
                return ( 1 );
            }

            // is there a platform for the environment OS?
            if ( package.GetPlatformTemplate() == null )
            {
                Console.WriteLine( $"I can't find a platform template for '{OSInformation.GetOSTemplate()}'." );
                Console.WriteLine();

                return ( 1 );
            }

            // specific version?
            if ( !string.IsNullOrEmpty( Version ) )
            {
                package.SetVersion( Version );
            }

            // is it a GitHub package?
            if ( package.Source.Equals( "github" ) && package.Version.Contains( '$' ) )
            {
                await SetPackageVersionFromGitHubAsync( package );
            }

            return await DownloadPackage( package );
        }

        private async Task<int> DownloadPackage( Package package )
        {
            if ( package.Version.Contains( '$' ) )
            {
                Console.WriteLine( $"I can't figure out the latest version for '{Name}' tool." );
                Console.WriteLine( "You can try manually specifying a --version value." );
                Console.WriteLine();

                return ( 1 );
            }

            Console.WriteLine( $"Downloading {package.FileName}:{package.GetSemVer()}..." );

            var platform = package.GetPlatformTemplate();
            var url = package.GetDownloadUrl();

            Console.WriteLine( url );

            var tmpFilepath = Path.GetTempFileName();

            // download file
            var httpClient = new System.Net.Http.HttpClient();

            using ( var streamIn = await httpClient.GetStreamAsync( url ) )
            {
                using ( var streamOut = new FileStream( tmpFilepath, FileMode.Open, FileAccess.Write, FileShare.None ) )
                {
                    await streamIn.CopyToAsync( streamOut );
                }
            }

            // TODO: handle tar.gz
            // TODO: handle .zip
            if ( platform.DownloadUrl.EndsWith( ".tar.gz" ) )
            {
                tmpFilepath = package.ExtractPackage( tmpFilepath, PackageCompression.TarGZip );

                if ( tmpFilepath == null )
                {
                    return ( 1 );
                }
            }

            if ( platform.DownloadUrl.EndsWith( ".zip" ) )
            {
                tmpFilepath = package.ExtractPackage( tmpFilepath, PackageCompression.Zip );

                if ( tmpFilepath == null )
                {
                    return ( 1 );
                }
            }

            PathHelper.EnsureTargetPathExists();

            File.Move( tmpFilepath, package.GetTargetFilePath(), true );
            Console.WriteLine( $"Tool saved to '{package.GetTargetFilePath()}'" );

            // give execution permission on Linux and Darwin
            if ( !OSInformation.IsWindows() )
            {
                await Cli.Wrap("/bin/bash")
                    .WithArguments( new[]
                    {
                        "-c",
                        $"chmod +x {package.GetTargetFilePath()}"
                    } )
                    .ExecuteAsync();
            }

            Console.WriteLine();

            // add tool to local database
            var installedTools = await InstalledToolSource.LoadAsync();

            installedTools.Tools[package.FileName] = package.GetSemVer();

            await installedTools.WriteAsync();

            return ( 0 );
        }

        private async Task<bool> SetPackageVersionFromGitHubAsync( Package package )
        {
            // let's attempt to retrieve the latest release
            var latestVersion = await package.GetLatestVersionAsync();

            // if that doesn't work we'll attempt to retrieve the latest tag (when available)
            if ( ( latestVersion == null ) && ( !string.IsNullOrEmpty( package.Tag ) ) )
            {
                latestVersion = await package.GetLatestVersionFromTagsAsync();

                // an alternative to the tags, depending on the package, naturally, could be
                // an external API
                // for example....
                // kubernetes releases can be retrieved using Google Storage API
                // https://storage.googleapis.com/storage/v1/b/kubernetes-release/o?prefix=release/latest-&fields=items(name,generation,timeCreated)
                // that returns the name and the timeCreated
                // that will be much faster than using GitHub tags
            }

            if ( latestVersion == null )
            {
                return ( false );
            }

            package.SetVersion( latestVersion, false );

            return ( true );
        }

    }
}
