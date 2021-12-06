using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliKit
{
    [Command( Name = "clik" )]
    [Subcommand( 
        typeof( AddCommand ),
        typeof( ListCommand ),
        typeof( RemoveCommand ),
        typeof( UpdateCommand ),
        typeof( VersionCommand )
    )]
    public class CliKitCommand
    {
        protected Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowHelp();

            return Task.FromResult( 0 );
        }
    }
}
