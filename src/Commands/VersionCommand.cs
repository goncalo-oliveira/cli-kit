using System;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliKit
{
    [Command( Name = "version", Description = "Displays the version information" )]
    public class VersionCommand
    {
        protected int OnExecute( CommandLineApplication app )
        {
            Console.WriteLine( "CLI Kit " + GetType().Assembly.GetVersion() );
            Console.WriteLine( "Copyright (C) 2021 Goncalo Oliveira" );
            Console.WriteLine();

            Console.WriteLine( OSInformation.GetOSTemplate() );
            Console.WriteLine( OSInformation.GetOSDescription() );
            Console.WriteLine();

            return 0;
        }
    }
}
