using System;
using System.Collections.Generic;
using TagLib;

namespace Sciendo.FilesAnalyser
{
    public interface IFileSystem
    {
        IEnumerable<string> GetFiles(string path,string[] extensions);

        IEnumerable<string> GetLeafDirectories(string path);
        bool DirectoriesExist(string[] paths);
        bool FileExists(string path);

        bool StopActivity { get; set; }

        Tag ReadTagFromFile(string filePath);

        event EventHandler<ExtensionsReadEventArgs> ExtensionsRead;

        event EventHandler<DirectoryReadEventArgs> DirectoryRead;
    }
}
