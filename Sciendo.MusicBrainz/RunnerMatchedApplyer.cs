using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sciendo.Common.Serialization;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class RunnerMatchedApplyer
    {
        private readonly string source;
        private readonly IMusicBrainzAdapter musicBrainzAdapter;

        public RunnerMatchedApplyer(IMusicBrainzAdapter musicBrainzAdapter, string source)
        {
            this.source = source;
            this.musicBrainzAdapter = musicBrainzAdapter;
        }
        public void Start()
        {
            MatchedFiles = new List<FileAnalysed>();
            var filesAnalysed = Serializer.DeserializeFromFile<FileAnalysed>(source);
            musicBrainzAdapter.LinkToExisting(filesAnalysed);
        }

        public List<FileAnalysed> MatchedFiles { get; private set; }

        public void Stop()
        {
            musicBrainzAdapter.StopActivity = true;
        }

    }
}
