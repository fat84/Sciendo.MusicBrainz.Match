using System;

namespace Sciendo.FilesAnalyser
{
    public class DirectoryReadEventArgs:EventArgs
    {
        public string Directory { get; private set; }

        public DirectoryReadEventArgs(string directory)
        {
            Directory = directory;
        }
    }
}