using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicBrainz.Match
{
    public interface IFileSystem
    {
        IEnumerable<IEnumerable<string>> GetFiles(string path,string[] extensions);
        bool DirectoriesExist(string[] paths);
        bool FileExists(string path);
    }
}
