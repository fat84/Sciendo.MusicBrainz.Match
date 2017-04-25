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
            MatchedFiles = Serializer.DeserializeFromFile<FileAnalysed>(source);
            musicBrainzAdapter.LinkToExisting(MatchedFiles);
        }

        public List<FileAnalysed> MatchedFiles { get; private set; }

        public void Stop()
        {
            musicBrainzAdapter.StopActivity = true;
        }

        public void SaveTrace()
        {
            if (MatchedFiles != null && MatchedFiles.Any())
            {
                UpSertSave(source);
            }
        }

        private void UpSertSave(string file)
        {
            Serializer.SerializeToFile(MatchedFiles, file);
        }


    }
}
