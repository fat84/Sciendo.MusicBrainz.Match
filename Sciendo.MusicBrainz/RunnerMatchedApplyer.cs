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
        private readonly ApplyType _applyType;
        private readonly IMusicBrainzAdapter musicBrainzAdapter;

        public RunnerMatchedApplyer(IMusicBrainzAdapter musicBrainzAdapter, string source, ApplyType applyType)
        {
            this.source = source;
            _applyType = applyType;
            this.musicBrainzAdapter = musicBrainzAdapter;
        }
        public void Start()
        {
            SetOfFiles = Serializer.DeserializeFromFile<FileAnalysed>(source);
            switch (_applyType)
            {
                case ApplyType.Matched:
                    musicBrainzAdapter.LinkToExisting(SetOfFiles,false);
                    break;
                case ApplyType.UnMatched:
                    musicBrainzAdapter.CreateNew(SetOfFiles);
                    break;
                case ApplyType.All:
                    musicBrainzAdapter.LinkToExisting(SetOfFiles,true);
                    break;
                default:
                    return;
            }
        }

        public List<FileAnalysed> SetOfFiles { get; private set; }

        public void Stop()
        {
            musicBrainzAdapter.StopActivity = true;
        }

        public void SaveTrace()
        {
            if (SetOfFiles != null && SetOfFiles.Any())
            {
                UpSertSave(source);
            }
        }

        private void UpSertSave(string file)
        {
            Serializer.SerializeToFile(SetOfFiles, file);
        }


    }
}
