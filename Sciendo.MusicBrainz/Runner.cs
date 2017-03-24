using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sciendo.Common.Serialization;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class Runner:IRunnerCollector
    {
        private MusicBrainzAdapter musicBrainzAdapter;
        private string source;
        private string matched;
        private string unMatched;
        private readonly bool _append;

        public Runner(MusicBrainzAdapter musicBrainzAdapter, string source, string matched, string unMatched,bool append)
        {
            this.musicBrainzAdapter = musicBrainzAdapter;
            this.source = source;
            this.matched = matched;
            this.unMatched = unMatched;
            _append = append;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            AllResults=new List<FileAnalysed>();
            var filesAnalysed = Serializer.DeserializeFromFile<FileAnalysed>(source);
            var startId = (_append)?GetStartId():0;
            AllResults = musicBrainzAdapter.CheckBulk(filesAnalysed.Where(f=>f.Id>=startId)).ToList();
        }

        private long GetStartId()
        {
            long matchedStart =GetStartId(matched);
            long unMatchedStart = GetStartId(unMatched);
            return (matchedStart > unMatchedStart) ? matchedStart : unMatchedStart;
        }

        private long GetStartId(string file)
        {
            long startId = 0;
            if (File.Exists(file))
            {
                startId = Serializer.DeserializeFromFile<FileAnalysed>(file).Max(f => f.Id) +1;
            }
            return startId;
        }

        public List<FileAnalysed> AllResults { get; set; }

        public void Stop()
        {
            musicBrainzAdapter.StopActivity = true;
        }

        
        public List<FileAnalysed> FilesAnalysed { get; }

        private void UpSertSave(string file, Func<FileAnalysed, bool> predicate=null)
        {
            if (_append)
            {
                var existingResults = Serializer.DeserializeFromFile<FileAnalysed>(file);
                if (existingResults != null)
                    AllResults.AddRange(existingResults);
            }
            if(predicate==null)
                Serializer.SerializeToFile(AllResults, file);
            else
            {
                Serializer.SerializeToFile(AllResults.Where(predicate).ToList(),file);
            }

        }
        public void CollectAndSave()
        {
            if (AllResults != null && AllResults.Any())
                if (string.IsNullOrEmpty(unMatched))
                    UpSertSave(matched);
                else
                {
                    UpSertSave(matched,(FileAnalysed f)=> { return f.MbId != Guid.Empty; });
                    UpSertSave(unMatched, (FileAnalysed f) => { return f.MbId == Guid.Empty; });
                }
        }
    }
}
