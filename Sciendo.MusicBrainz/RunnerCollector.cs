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
        private const string MatchedPostfixName = "matched";
        private const string UnMatchedArtistPostfixName = "artistunmatched";
        private const string UnMatchedAlbumPostfixName = "albumunmatched";
        private const string UnMatchedTitlePostfixName = "titleunmatched";
        private const string ErrorPostfixName = "error";
        private MusicBrainzAdapter musicBrainzAdapter;
        private readonly string _source;
        private readonly string _matched;
        private readonly bool _append;
        private readonly string _unMatchedArtists;
        private readonly string _unMatchedAlbums;
        private readonly string _unMatchedTitles;
        private readonly string _errors;

        public RunnerCollector(MusicBrainzAdapter musicBrainzAdapter, string source, bool append)
        {
            this.musicBrainzAdapter = musicBrainzAdapter;
            this._source = source;
            string sourceFileName = Path.GetFileNameWithoutExtension(source);
            string sourceDirectory = Path.GetDirectoryName(source);
            string sourceExtension = Path.GetExtension(source);

            this._matched =Path.Combine(sourceDirectory, $"{sourceFileName + MatchedPostfixName}.{sourceExtension}");
            this._unMatchedArtists = Path.Combine(sourceDirectory, $"{sourceFileName + UnMatchedArtistPostfixName}.{sourceExtension}");
            this._unMatchedAlbums = Path.Combine(sourceDirectory, $"{sourceFileName + UnMatchedAlbumPostfixName}.{sourceExtension}");
            this._unMatchedTitles = Path.Combine(sourceDirectory, $"{sourceFileName + UnMatchedTitlePostfixName}.{sourceExtension}");
            this._errors = Path.Combine(sourceDirectory, $"{sourceFileName + ErrorPostfixName}.{sourceExtension}");
            _append = append;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            AllResults=new List<Music>();
            var filesAnalysed = Serializer.DeserializeFromFile<Music>(_source);
            var startId = (_append)?GetStartId():0;
            AllResults = musicBrainzAdapter.CheckBulk(filesAnalysed.Where(f=>f.Id>=startId)).ToList();
        }

        private long GetStartId()
        {
            List<long> startIds= new List<long>();
            startIds.Add(GetStartId(_matched));
            startIds.Add(GetStartId(_unMatchedArtists));
            startIds.Add(GetStartId(_unMatchedAlbums));
            startIds.Add(GetStartId(_unMatchedTitles));
            startIds.Add(GetStartId(_errors));
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
            if (AllResults == null || !AllResults.Any()) return;
            UpSertSave(_matched,(Music f)=> { return f.AllItemsMatched(); });
            UpSertSave(_unMatchedArtists, (Music f) => { return f.AnyItemsWithUnMatched(QueryType.ArtistMatching); });
            UpSertSave(_unMatchedAlbums, (Music f) => { return f.AnyItemsWithUnMatched(QueryType.IndividualAlbumMatching); });
            UpSertSave(_unMatchedTitles, (Music f) => { return f.AnyItemsWithUnMatched(QueryType.TitleMatching); });
            UpSertSave(_errors, (Music f) => { return f.AnyItemsWithErrors(); });
        }
    }
}
