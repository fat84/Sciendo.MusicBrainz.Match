using System;
using System.IO;
using Sciendo.MusicMatch.Contracts;
using TagLib;

namespace Sciendo.FilesAnalyser
{
    public interface IAnalyser
    {
        Music AnalyseFile(string filePath);
        
        string[] CollectionPaths { get; }

        string CollectionMarker { get; }

        bool StopActivity { get; set; }

        event EventHandler<AnalyserProgressEventArgs> AnalyserProgress;
    }
}
