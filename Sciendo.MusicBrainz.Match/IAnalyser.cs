using System;
using Id3;

namespace Sciendo.FilesAnalyser
{
    public interface IAnalyser
    {
        FileAnalysed AnalyseFile(IMp3Stream mp3File,string filePath);
        
        string[] CollectionPaths { get; }

        string CollectionMarker { get; }

        bool StopActivity { get; set; }

        event EventHandler<AnalyserProgressEventArgs> AnalyserProgress;

        string Mp3IocKey { get; }
    }
}
