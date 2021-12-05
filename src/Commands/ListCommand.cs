using System;
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

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            // TODO: unless --local is displayed, show all available tools for install

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
    }
}
