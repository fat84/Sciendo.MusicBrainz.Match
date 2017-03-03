using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicBrainz.Match
{
    public interface IFileSystem
    {
        IEnumerable<string> GetFiles(string path,string[] extensions);

        IEnumerable<string> GetLeafDirectories(string path);
        bool DirectoriesExist(string[] paths);
        bool FileExists(string path);

        bool StopActivity { get; set; }

        event EventHandler<ExtensionsReadEventArgs> ExtensionsRead;

        event EventHandler<DirectoryReadEventArgs> DirectoryRead;
    }
}
