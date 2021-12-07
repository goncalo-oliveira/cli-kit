using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CliWrap;
using McMaster.Extensions.CommandLineUtils;
using ShellProgressBar;

namespace CliKit
{
    [Command( Name = "add", Description = "Download a tool" )]
    public class AddCommand
    {
        [Option( "--source|-s", Description = "Custom package source location" )]
        public string Source { get; set; }

        [Option( "--platform", Description = "Override the platform template to download" )]
        public string PlatformOverride { get; set; }

        [Required]
        [Argument( 0, Name = "name", Description = "The name(s) of the tool(s) to download" )]
        public string[] Names { get; set; }

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            var packageSource = await PackageHelper.LoadAsync( Source );

            if ( packageSource == null )
            {
                return ( 1 );
            }

            var packagesToInstall = new List<Package>();
            foreach ( var nameItem in Names )
            {
                var name = nameItem;
                var version = (string)null;

                // look for version alias (name:version or name@version)
                if ( nameItem.Contains( ':' ) || nameItem.Contains( '@' ) )
                {
                    var idx = nameItem.IndexOfAny( new char[] { ':', '@' } );

                    version = nameItem.Substring( idx + 1 );
                    name = nameItem.Substring( 0, idx );
                }

                var package = packageSource.Packages.GetPackage( name );

                if ( package == null )
                {
                    Console.WriteLine( $"I'm sorry, I don't know how to download '{name}' tool." );
                    Console.WriteLine( "If you'd like to make a request or a contribution, visit us on GitHub" );
                    Console.WriteLine( "https://github.com/goncalo-oliveira/cli-kit" );
                    Console.WriteLine();

                    continue;
                }

                // specific version?
                if ( !string.IsNullOrEmpty( version ) )
                {
                    package.SetVersion( version );
                }

                packagesToInstall.Add( package );
            }

            if ( !packagesToInstall.Any() )
            {
                // nothing to install
                return ( 1 );
            }

            // install single tool
            foreach ( var package in packagesToInstall )
            {
                await InstallPackageAsync( package );
            }

            Console.WriteLine();

            return ( 0 );
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

            // is it a GitHub package?
            if ( package.Source.Equals( "github" ) && package.Version.Contains( '$' ) )
            {
                var latestVersion = await package.GetLatestVersionFromGitHubAsync();

                if ( latestVersion != null )
                {
                    package.SetVersion( latestVersion, false );
                }
            }

            // download tool (and extract if necessary)
            var tmpFilepath = await DownloadPackage( package );

            // copy tool to output path
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

            // add tool to local database
            var installedTools = await InstalledToolSource.LoadAsync();

            installedTools.Tools[package.ToolName] = package.GetSemVer();

            await installedTools.WriteAsync();

            return ( 0 );
        }

        private async Task<string> DownloadPackage( Package package )
        {
            if ( package.Version.Contains( '$' ) )
            {
                Console.WriteLine( $"I can't figure out the latest version for '{package.ToolName}' tool." );
                Console.WriteLine( "You can try manually specifying a --version value." );
                Console.WriteLine();

                return ( null );
            }

            Console.WriteLine( $"Downloading {package.ToolName}:{package.GetSemVer()}..." );

            var platform = package.GetPlatformTemplate();
            var url = package.GetDownloadUrl();

            Console.WriteLine( url );

            var tmpFilepath = Path.GetTempFileName();

            // download file
            var httpClient = new System.Net.Http.HttpClient();

            using ( var progressBar = new ProgressBar( 10000, string.Empty, new ProgressBarOptions
            {
                ProgressCharacter = '.',
                ForegroundColor = Console.ForegroundColor
            } ) )
            {
                await httpClient.DownloadAsync( url, tmpFilepath, progressBar.AsProgress<float>() );
            }

            // TODO: handle tar.gz
            // TODO: handle .zip
            if ( platform.DownloadUrl.EndsWith( ".tar.gz" ) )
            {
                tmpFilepath = package.ExtractPackage( tmpFilepath, PackageCompression.TarGZip );
            }

            if ( platform.DownloadUrl.EndsWith( ".zip" ) )
            {
                tmpFilepath = package.ExtractPackage( tmpFilepath, PackageCompression.Zip );
            }

            return ( tmpFilepath );
        }
    }
}
