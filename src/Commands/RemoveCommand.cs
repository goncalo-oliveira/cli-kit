using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitPak
{
    [Command( Name = "remove", Description = "List available tools" )]
    public class RemoveCommand
    {
        [Required]
        [Argument( 0, Name = "name", Description = "The name of the tool to remove" )]
        public string Name { get; set; }

        protected async Task<int> OnExecuteAsync( CommandLineApplication app )
        {
            app.ShowRootCommandFullNameAndVersion();

            var filepath = Path.Combine( PathHelper.GetTargetPath(), Name );

            if ( OSInformation.IsWindows() )
            {
                filepath = string.Concat( filepath, ".exe" );
            }

            if ( !File.Exists( filepath ) )
            {
                Console.WriteLine( $"File '{filepath}' does not exist." );
                Console.WriteLine();

                return ( 1 );
            }

            File.Delete( filepath );

            Console.WriteLine( $"Deleted '{filepath}'." );
            Console.WriteLine();

            var installedTools = await InstalledToolSource.LoadAsync();

            if ( installedTools.Tools.ContainsKey( Name ) )
            {
                installedTools.Tools.Remove( Name );

                await installedTools.WriteAsync();
            }

            return ( 0 );
        }
    }
}
