using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitPak
{
    [Command( Name = "list", Description = "List available tools" )]
    public class ListCommand
    {
        [Option( "--source|-s", Description = "Custom package source location" )]
        public string Source { get; set; }

        [Option( "--local", Description = "Show tools already installed" )]
        public bool ShowLocal { get; set; }

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            // TODO: unless --local is displayed, show all available tools for install
            if ( ShowLocal )
            {
                await ListInstalledTools();
                Console.WriteLine();

                return ( 0 );
            }

            var packageSource = await PackageHelper.LoadAsync( Source );

            if ( packageSource == null )
            {
                return ( 1 );
            }

            Console.WriteLine( "Available tools:" );
            Console.WriteLine();

            foreach ( var package in packageSource.Packages.OrderBy( x => x.Key ) )
            {
                Console.WriteLine( $"  - {package.Key}" );
            }

            Console.WriteLine();

            return 0;
        }

        private async Task ListInstalledTools()
        {
            Console.WriteLine( "Installed tools:" );
            Console.WriteLine();

            IEnumerable<string> files;

            if ( !System.IO.Directory.Exists( PathHelper.GetTargetPath() ) )
            {
                files = Enumerable.Empty<string>();
            }
            else
            {
                files = System.IO.Directory.EnumerateFiles( PathHelper.GetTargetPath() )
                    .Select( x => System.IO.Path.GetFileNameWithoutExtension( x ) )
                    .ToArray();
            }

            if ( !files.Any() )
            {
                Console.WriteLine( "None yet." );

                return;
            }

            var installedTools = await InstalledToolSource.LoadAsync();

            foreach ( var file in files )
            {
                var version = installedTools.Tools.GetValueOrDefault( file ) ?? string.Empty;

                Console.WriteLine( $"found {file} {version}" );
            }
        }
    }
}
