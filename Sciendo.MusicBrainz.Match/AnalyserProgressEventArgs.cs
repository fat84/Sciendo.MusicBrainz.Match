using System;

namespace Sciendo.MusicBrainz.Match
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