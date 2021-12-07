using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace CliKit
{
    internal enum PackageCompression
    {
        TarGZip,
        Zip
    }

    internal static class PackageExtractExtensions
    {
        public static string ExtractPackage( this Package package, string tmpFilePath, PackageCompression compression )
        {
            var compressedFilename = Path.GetFileName( package.GetPlatformTemplate().DownloadUrl );

            Console.WriteLine( $"Extracting {compressedFilename}..." );

            var tmpPath = Path.Combine( Path.GetTempPath(), $"{package.ToolName}-{Guid.NewGuid().ToString( "N" ).Substring( 0, 6 )}" );

            try
            {
                switch ( compression )
                {
                    case PackageCompression.TarGZip:
                        return ExtractPackageTarGz( package, tmpFilePath, tmpPath );

                    case PackageCompression.Zip:
                        return ExtractPackageZip( package, tmpFilePath, tmpPath );

                    default:
                    {
                        Console.WriteLine( "I don't know how to extract this file!" );

                        return ( null );
                    }
                }
            }
            catch ( FileNotFoundException )
            {
                Console.WriteLine( $"Can't find '{package.GetTargetFileName()}' on the extracted files!" );

                if ( File.Exists( tmpFilePath ) )
                {
                    File.Delete( tmpFilePath );
                }

                return ( null );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "Failed to extract file: " + ex.Message );

                if ( File.Exists( tmpFilePath ) )
                {
                    File.Delete( tmpFilePath );
                }

                return ( null );
            }
            finally
            {
                var tmpFolder = new DirectoryInfo( tmpPath );

                tmpFolder.Empty();
            }
        }

        public static string ExtractPackageTarGz( this Package package, string tmpFilepath, string tmpPath )
        {
            using ( Stream inStream = File.OpenRead( tmpFilepath ) )
            {
                using ( Stream gzipStream = new GZipInputStream( inStream ) )
                {
                    TarArchive tarArchive = TarArchive.CreateInputTarArchive( gzipStream, Encoding.UTF8 );
                    tarArchive.ExtractContents( tmpPath );
                    tarArchive.Close();
                }
            }

            File.Delete( tmpFilepath );

            // move tool executable
            var filepath = Path.Combine( tmpPath, package.GetPlatformTemplate().ExtractUrl ?? package.ToolName );

            if ( !File.Exists( filepath ) )
            {
                throw new FileNotFoundException();
            }

            tmpFilepath = Path.GetTempFileName();

            File.Move( filepath, tmpFilepath, true );

            return ( tmpFilepath );
        }

        public static string ExtractPackageZip( this Package package, string tmpFilepath, string tmpPath )
        {
            using ( var zipFile = new ZipFile( File.OpenRead( tmpFilepath ), false ) )
            {
                var entry = zipFile.GetEntry( package.GetPlatformTemplate().ExtractUrl ?? package.GetTargetFileName() );

                if ( entry == null )
                {
                    throw new FileNotFoundException();
                }

                var tmpZipExtractPath = Path.GetTempFileName();

                try
                {
                    using ( var zipStream = zipFile.GetInputStream( entry ) )
                    {
                        using ( var tmpZipExtractStream = File.OpenWrite( tmpZipExtractPath ) )
                        {
                            zipStream.CopyTo( tmpZipExtractStream );
                        }
                    }

                    File.Delete( tmpFilepath );

                    return ( tmpZipExtractPath );
                }
                catch ( Exception ex )
                {
                    if ( File.Exists( tmpZipExtractPath ) )
                    {
                        File.Delete( tmpZipExtractPath );
                    }

                    #pragma warning disable CA2200
                    throw ex;
                    #pragma warning restore CA2200
                }
            }
            
        }
    }
}
