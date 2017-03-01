using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sciendo.MusicBrainz.Match
{
    public class FileSystem:IFileSystem
    {
        public IEnumerable<IEnumerable<string>> GetFiles(string path, string[] extensions)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if(extensions==null)
                throw new ArgumentNullException(nameof(extensions));
            if (!Directory.Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));
            foreach (var extension in extensions)
                yield return
                Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(extension));
        }

        public bool DirectoriesExist(string[] paths)
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    return false;
            }
            return true;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
