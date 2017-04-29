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
        //void LinkToExisting(IEnumerable<Music> filesAnalysed, bool forceCreate, bool testOnly);

        //void CreateNew(IEnumerable<Music> filesAnalysed, bool testOnly);

        Music Check(Music music);
        
        IEnumerable<Music> CheckBulk(IEnumerable<Music> filesAnalysed);

        event EventHandler<CheckMatchingProgressEventArgs> CheckMatchingProgress;

        //event EventHandler<ApplyProgressEventArgs> ApplyProgress;

        bool StopActivity { get; set; }
    }
}
