using System;

namespace GitPak.GitHub
{
    public class Release
    {
        public string Url { get; set; }
        public string AssetsUrl { get; set; }
        public string UploadUrl { get; set; }
        public string HtmlUrl { get; set; }
        public string TagName { get; set; }
        public bool PreRelease { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
