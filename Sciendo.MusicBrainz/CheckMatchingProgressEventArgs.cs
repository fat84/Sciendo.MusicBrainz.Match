using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class CheckMatchingProgressEventArgs:EventArgs
    {
        public string File { get; private set; }

        public ExecutionStatus MatchStatus { get; private set; }

        public CheckMatchingProgressEventArgs(string file, ExecutionStatus matchStatus)
        {
            File = file;
            MatchStatus = matchStatus;
        }
    }
}