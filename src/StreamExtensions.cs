using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CliKit
{
    internal static class StreamCopyExtensions
    {
        public static async Task CopyToAsync( this Stream source
            , Stream destination
            , int bufferSize
            , IProgress<long> progress = null
            , CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ( ( bytesRead = await source.ReadAsync( buffer, 0, buffer.Length, cancellationToken ).ConfigureAwait( false ) ) != 0 )
            {
                await destination.WriteAsync( buffer, 0, bytesRead, cancellationToken ).ConfigureAwait( false );

                totalBytesRead += bytesRead;
                
                progress?.Report( totalBytesRead );
            }
        }
    }
}
