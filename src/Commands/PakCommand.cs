using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitPak
{
    [Command( Name = "gitpak" )]
    [Subcommand( 
        typeof( AddCommand ),
        typeof( ListCommand ),
        typeof( VersionCommand )
    )]
    public class PakCommand
    {
        protected Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowHelp();

            return Task.FromResult( 0 );
        }
    }
}
