using System;
using GitPak;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = new HostBuilder();

try
{
    return await builder.RunCommandLineApplicationAsync<PakCommand>( args, app =>
    {
        app.FullName = "gitpak";
        app.ShortVersionGetter = () => System.Reflection.Assembly.GetExecutingAssembly().GetVersion();
    } );
}
catch ( Exception ex )
{
    Console.WriteLine( ex.Message );
    
    return ( 1 );
}
