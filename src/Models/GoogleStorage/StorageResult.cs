using System;
using System.Collections.Generic;

namespace GitPak.GoogleStorage
{
    public class StorageResult<T>
    {
        public IEnumerable<T> Items { get; set; }
    }
}
