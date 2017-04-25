using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class CheckMatchingProgressEventArgs:EventArgs
    {
        public string File { get; private set; }

        public MatchStatus MatchStatus { get; private set; }

        public CheckMatchingProgressEventArgs(string file, MatchStatus matchStatus)
        {
            File = file;
            MatchStatus = matchStatus;
        }
    }
}