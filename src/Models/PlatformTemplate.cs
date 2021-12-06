using System;

namespace CliKit
{
    public class PlatformTemplate
    {
        public PlatformTemplate()
        {}

        private PlatformTemplate( string template )
        {
            if ( template.Contains( ',' ) )
            {
                int idx = template.IndexOf( ',' );
                DownloadUrl = template.Substring( 0, idx ).TrimEnd();
                ExtractUrl = template.Substring( idx + 1 ).TrimStart();
            }
            else
            {
                DownloadUrl = template;
                ExtractUrl = null;
            }
        }

        public string DownloadUrl { get; }
        public string ExtractUrl { get; }

        public static implicit operator PlatformTemplate ( string template )
            => new PlatformTemplate( template );
    }
}
