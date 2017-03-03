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

        public IEnumerable<string> GetFiles(string path, string[] extensions)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if(extensions==null)
                throw new ArgumentNullException(nameof(extensions));
            if (!Directory.Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));
            if(DirectoryRead!=null)
                DirectoryRead(this, new DirectoryReadEventArgs(path));
            foreach (var extension in extensions)
            {
                if (!StopActivity)
                {
                    if(ExtensionsRead!=null)
                        ExtensionsRead(this,new ExtensionsReadEventArgs(extension));
                    foreach(var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(extension)))
                    {
                        yield return file;
                    }

                }

            }
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

        public bool StopActivity { get; set; }
        public event EventHandler<ExtensionsReadEventArgs> ExtensionsRead;
        public IEnumerable<string> GetLeafDirectories(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));
            return Directory.EnumerateDirectories(path, "*.*", SearchOption.AllDirectories)
                .Where(d => !Directory.EnumerateDirectories(d, "*.*", SearchOption.AllDirectories).Any());
        }

        public event EventHandler<DirectoryReadEventArgs> DirectoryRead;
    }
}
