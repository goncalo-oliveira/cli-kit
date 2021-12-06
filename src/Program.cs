using System;
using CliKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = new HostBuilder();

try
{
    return await builder.RunCommandLineApplicationAsync<CliKitCommand>( args, app =>
    {
        app.FullName = "CLI Kit";
        app.ShortVersionGetter = () => System.Reflection.Assembly.GetExecutingAssembly().GetVersion();
    } );
}
catch ( Exception ex )
{
    Console.WriteLine( ex.Message );
    
    return ( 1 );
}
