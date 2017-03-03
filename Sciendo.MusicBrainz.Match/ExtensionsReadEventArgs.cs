using System;

namespace Sciendo.MusicBrainz.Match
{
    public class ExtensionsReadEventArgs:EventArgs
    {
        public string Extension { get; private set; }

        public ExtensionsReadEventArgs(string extension)
        {
            Extension = extension;
        }
    }
}