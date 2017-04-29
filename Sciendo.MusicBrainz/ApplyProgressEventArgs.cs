using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class ApplyProgressEventArgs:EventArgs
    {
        public string FilePath { get; private set; }

        public ExecutionStatus ApplyStatus { get; private set; }

        public ApplyProgressEventArgs(string filePath, ExecutionStatus applyStatus)
        {
            FilePath = filePath;
            ApplyStatus = applyStatus;
        }
    }
}