using System;
using Faactory.Collections;

namespace GitPak
{
    public class Package
    {
        /// <summary>
        /// The name of the tool executable
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// The name of the tool
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The source of the tool [github, url]
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The download url template [github: optional, url: required]
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The owner of the package [github: repository owner]
        /// </summary>
        /// <value></value>
        public string Owner { get; set; }

        /// <summary>
        /// The version template
        /// </summary>
        public string Version { get; set; } = "$";

        /// <summary>
        /// Additional plugins to use
        /// </summary>
        /// <value></value>
        public Dictionary<Metadata> Plugins { get; set; }

        /// <summary>
        /// The tag template [url only]; tells installer to use tags (from GitHub) instead of releases
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The supported platforms
        /// </summary>
        public Metadata Platforms { get; set; }
    }
}
