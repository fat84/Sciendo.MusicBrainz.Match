using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class CheckProgressEventArgs:EventArgs
    {
        public string File { get; private set; }

        public MatchStatus MatchStatus { get; private set; }

        public CheckProgressEventArgs(string file, MatchStatus matchStatus)
        {
            File = file;
            MatchStatus = matchStatus;
        }
    }
}