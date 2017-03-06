using System;

namespace Sciendo.FilesAnalyser
{
    public class AnalyserProgressEventArgs:EventArgs
    {
        public string FilePath { get; private set; }

        public AnalyserProgressEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }
}