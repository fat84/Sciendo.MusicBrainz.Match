using System;

namespace Sciendo.MusicBrainz
{
    public class CheckProgressEventArgs:EventArgs
    {
        public string File { get; private set; }

        public bool Matched { get; private set; }

        public CheckProgressEventArgs(string file, bool matched)
        {
            File = file;
            Matched = matched;
        }
    }
}