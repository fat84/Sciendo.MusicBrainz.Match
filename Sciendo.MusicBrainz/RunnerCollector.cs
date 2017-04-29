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
    public class RunnerCollector:IRunnerCollector
    {
        private MusicBrainzAdapter musicBrainzAdapter;
        private string source;
        private string matched;
        private string unMatched;
        private readonly bool _append;
        private string matchingErrors;

        public RunnerCollector(MusicBrainzAdapter musicBrainzAdapter, string source, string matched, string unMatched,string matchingErrors, bool append)
        {
            this.musicBrainzAdapter = musicBrainzAdapter;
            this.source = source;
            this.matched = matched;
            this.unMatched = unMatched;
            this.matchingErrors = matchingErrors;
            _append = append;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            AllResults=new List<Music>();
            var filesAnalysed = Serializer.DeserializeFromFile<Music>(source);
            var startId = (_append)?GetStartId():0;
            AllResults = musicBrainzAdapter.CheckBulk(filesAnalysed.Where(f=>f.Id>=startId)).ToList();
        }

        private long GetStartId()
        {
            List<long> startIds= new List<long>();
            startIds.Add(GetStartId(matched));
            startIds.Add(GetStartId(unMatched));
            startIds.Add(GetStartId(matchingErrors));
            return startIds.Max(s=>s);
        }

        private long GetStartId(string file)
        {
            long startId = 0;
            if (File.Exists(file))
            {
                startId = Serializer.DeserializeFromFile<Music>(file).Max(f => f.Id) +1;
            }
            return startId;
        }

        public List<Music> AllResults { get; set; }

        public void Stop()
        {
            musicBrainzAdapter.StopActivity = true;
        }

        
        public List<Music> FilesAnalysed { get; }

        private void UpSertSave(string file, Func<Music, bool> predicate=null)
        {
            if (_append)
            {
                var existingResults = Serializer.DeserializeFromFile<Music>(file);
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
                {
                    UpSertSave(matched,(Music f)=> { return f.AllItemsMatched(); });
                    UpSertSave(matchingErrors, (Music f) => { return f.AnyItemsWithErrors(); });
                }
                else
                {
                    UpSertSave(matched,(Music f)=> { return f.AllItemsMatched(); });
                    UpSertSave(unMatched, (Music f) => { return f.AnyItemsWithUnMatched(); });
                    UpSertSave(matchingErrors, (Music f) => { return f.AnyItemsWithErrors(); });
                }
        }
    }
}
