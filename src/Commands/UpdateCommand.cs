using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliKit
{
    [Command( Name = "update", Description = "Look for available updates for installed tools" )]
    public class UpdateCommand
    {
        [Option( "--source|-s", Description = "Custom package source location" )]
        public string Source { get; set; }

        [Option( "--install|-i", Description = "Automatically install update")]
        public bool AutoInstall { get; set; }

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            // this only works if the installed.yaml is healthy
            var installedTools = await InstalledToolSource.LoadAsync();

            if ( !installedTools.Tools.Any() )
            {
                Console.WriteLine( "No tools installed or missing 'installed.yaml' file." );
                Console.WriteLine();

                return ( 1 );
            }

            var packageSource = await PackageHelper.LoadAsync( Source );

            if ( packageSource == null )
            {
                return ( 1 );
            }

            var tasks = installedTools.Tools.Keys.Select( x => GetLatestAsync( packageSource.Packages.GetPackage( x ) ) )
                .ToArray();

            Console.WriteLine( "Looking for updates..." );

            var results = await Task.WhenAll( tasks );

            Console.WriteLine();

            var numberOfUpdates = 0;
            foreach ( var update in results )
            {
                var version = installedTools.Tools.GetValueOrDefault( update.Package.FileName ) ?? string.Empty;

                if ( ( update.LatestVersion != null ) && !version.Equals( update.LatestVersion ) )
                {
                    Console.WriteLine( $"found {update.Package.FileName} {version}" );
                    Console.WriteLine( $"...a new version ({update.LatestVersion}) is available" );

                    if ( !AutoInstall )
                    {
                        numberOfUpdates++;
                        continue;
                    }

                    await InstallPackageAsync( update.Package, update.LatestVersion );
                }
            }

            if ( numberOfUpdates > 0 )
            {
                Console.WriteLine( $"{numberOfUpdates} tool(s) can be upgraded." );
            }
            else
            {
                Console.WriteLine( "All tools are up to date." );
            }

            Console.WriteLine();

            return 0;
        }

        private Task<int> InstallPackageAsync( Package package, string latestVersion )
        {
            var addCommand = new AddCommand
            {
                Source = Source,
                Name = package.Name,
                Version = latestVersion
            };

            return addCommand.InstallPackageAsync( package );
        }

        private async Task<PackageUpdate> GetLatestAsync( Package package )
        {
            // TODO: this code is sort of duplicated on AddCommand
            //       make it better

            var latestVersion = await package.GetLatestVersionAsync();

            if ( ( latestVersion == null ) && ( !string.IsNullOrEmpty( package.Tag ) ) )
            {
                latestVersion = await package.GetLatestVersionFromTagsAsync();
            }

            // extract semver
            if ( latestVersion != null )
            {
                latestVersion = new Package
                {
                    Version = latestVersion
                }
                .GetSemVer();
            }

            return ( new PackageUpdate
            {
                Package = package,
                LatestVersion = latestVersion
            } );
        }

        private class PackageUpdate
        {
            public Package Package { get; set; }
            public string LatestVersion { get; set; }
        }
    }
}
