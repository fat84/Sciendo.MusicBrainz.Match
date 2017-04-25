using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class ApplyProgressEventArgs:EventArgs
    {
        public string FilePath { get; private set; }

        public ApplyStatus ApplyStatus { get; private set; }

        public ApplyProgressEventArgs(string filePath, ApplyStatus applyStatus)
        {
            FilePath = filePath;
            ApplyStatus = ApplyStatus;
        }
    }
}