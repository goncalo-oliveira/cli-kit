using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CliKit
{
    internal static class HttpClientDownloadExtensions
    {
        public static async Task DownloadAsync( this HttpClient httpClient
            , string url
            , string destinationFilePath
            , IProgress<float> progress = null
            , CancellationToken cancellationToken = default( CancellationToken ) )
        {
            using ( var streamOut = new FileStream( destinationFilePath, FileMode.Open, FileAccess.Write, FileShare.None ) )
            {
                using ( var response = await httpClient.GetAsync( url, System.Net.Http.HttpCompletionOption.ResponseHeadersRead ) )
                {
                    var contentLength = response.Content.Headers.ContentLength;

                    using ( var streamIn = await response.Content.ReadAsStreamAsync() )
                    {

                        // Ignore progress reporting when no progress reporter was 
                        // passed or when the content length is unknown
                        if ( progress == null || !contentLength.HasValue )
                        {
                            await streamIn.CopyToAsync( streamOut );
                            return;
                        }

                        var relativeProgress = new Progress<long>( totalBytes => progress.Report( (float)totalBytes / contentLength.Value ) );

                        // Use extension method to report progress while downloading
                        await streamIn.CopyToAsync( streamOut, 81920, relativeProgress, cancellationToken );

                        progress.Report( 1 );
                    }
                }
            }
        }
    }
}
