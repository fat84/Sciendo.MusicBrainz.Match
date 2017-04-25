using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Sciendo.FilesAnalyser;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public interface IMusicBrainzAdapter
    {
        void LinkToExisting(IEnumerable<FileAnalysed> filesAnalysed);

        void CreateNew(IEnumerable<FileAnalysed> filesAnalysed);

        FileAnalysed Check(FileAnalysed fileAnalysed);
        
        IEnumerable<FileAnalysed> CheckBulk(IEnumerable<FileAnalysed> filesAnalysed);

        event EventHandler<CheckMatchingProgressEventArgs> CheckMatchingProgress;

        event EventHandler<ApplyProgressEventArgs> ApplyProgress;

        bool StopActivity { get; set; }
    }
}
