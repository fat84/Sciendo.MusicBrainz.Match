using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Sciendo.FilesAnalyser;

namespace Sciendo.MusicBrainz
{
    public interface IMusicBrainzAdapter
    {
        void LinkToExisting(IEnumerable<FileAnalysed> filesAnalysed);

        void CreateNew(IEnumerable<FileAnalysed> filesAnalysed);
    }
}
