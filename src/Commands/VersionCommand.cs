using System;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitPak
{
    [Command( Name = "version", Description = "Displays the version information" )]
    public class VersionCommand
    {
        protected int OnExecute( CommandLineApplication app )
        {
            Console.WriteLine( "gitpak " + GetType().Assembly.GetVersion() );
            Console.WriteLine( "Copyright (C) 2021 Goncalo Oliveira" );
            Console.WriteLine();

            Console.WriteLine( OSInformation.GetOSTemplate() );
            Console.WriteLine( OSInformation.GetOSDescription() );
            Console.WriteLine();

            return 0;
        }
    }
}
