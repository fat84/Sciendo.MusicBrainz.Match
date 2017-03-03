using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;

namespace Sciendo.MusicBrainz.Match
{
    public interface IAnalyser
    {
        FileAnalysed AnalyseFile(IMp3Stream mp3File,string filePath);
        
        string[] CollectionPaths { get; }

        string CollectionMarker { get; }

        bool StopActivity { get; set; }

        event EventHandler<AnalyserProgressEventArgs> AnalyserProgress;
    }
}
