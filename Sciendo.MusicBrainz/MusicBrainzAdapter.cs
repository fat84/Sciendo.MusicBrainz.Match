using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class MusicBrainzAdapter:IMusicBrainzAdapter
    {
        private readonly GraphClient _graphClient;
        private readonly ItemMemoryCache _artistCache;
        private readonly ItemMemoryCache _individualAlbumCache;

        public MusicBrainzAdapter(GraphClient graphClient)
        {
            _graphClient = graphClient;
            _artistCache=new ItemMemoryCache();
            _individualAlbumCache= new ItemMemoryCache();
        }
        //public void LinkToExisting(IEnumerable<Music> filesAnalysed, bool forceCreate, bool testOnly)
        //{
        //    if(filesAnalysed==null)
        //        throw new ArgumentNullException(nameof(filesAnalysed));

        //    foreach (var music in filesAnalysed)
        //    {

        //        var applyStatus = UpsertOneItem(music, music.CreateNeo4JMatchingVersion(),
        //            GetMatchInCollectionQuery,
        //            GetMatchArtistQuery, testOnly, true);
        //        if (forceCreate && applyStatus == ExecutionStatus.ExecutionError)
        //            UpsertOneItem(music, music.CreateNeo4JUpdatingVersion(), GetMergeInCollectionQuery,
        //                GetMergehNotInCollectionQuery, testOnly, false);
        //    }
        //}

        //private ExecutionStatus UpsertOneItem(Music music, Music workVersion, 
        //    Func<Music,ICypherFluentQuery> getInCollectionQuery, 
        //    Func<Music,ICypherFluentQuery> getNotInCollectionQuery, 
        //    bool testOnly,
        //    bool deletePreviousLocalTrack=false )
        //{
        //    music.Neo4jQuerries = new List<Neo4jQuery>();
        //    if (deletePreviousLocalTrack)
        //    {
        //        music.Neo4jQuerries.Add(new Neo4jQuery
        //        {
        //            ExecutionStatus = ExecutionStatus.NotExecuted,
        //            DebugQuery = DeleteLocalTrack(workVersion).Query.DebugQueryText
        //        });
        //        if(!testOnly)
        //            DeleteLocalTrack(workVersion).ExecuteWithoutResults();
        //    }

        //    var applyStatus = ExecutionStatus.NotExecuted;
        //    if (music.MarkedAsPartOfCollection)
        //    {
        //        applyStatus = ApplyChanges(music, workVersion, getInCollectionQuery, deletePreviousLocalTrack, testOnly);
        //        if (ApplyProgress != null)
        //            ApplyProgress(this, new ApplyProgressEventArgs(music.FilePath, applyStatus));
        //        return applyStatus;
        //    }
        //    applyStatus = ApplyChanges(music, workVersion, getNotInCollectionQuery,
        //    deletePreviousLocalTrack, testOnly);
        //    if (ApplyProgress != null)
        //        ApplyProgress(this, new ApplyProgressEventArgs(music.FilePath, applyStatus));
        //    return applyStatus;
        //}

//        private ICypherFluentQuery GetMergehNotInCollectionQuery(Music music)
//        {
////            MERGE(ac: ArtistCredit{ name: "2 Fabiola"})-[:CREDITED_ON]->(a: Album{ name: "Evolution"})-[:PART_OF] - (b:Release{ name: "Evolution"})-[:RELEASED_ON_MEDIUM] - (m:Cd{ name: "Evolution"})-[:APPEARS_ON] - (t:Track{ name: "She's after My piano (remix '16)"})
////with t
////limit 1
////MERGE(t) -[lr: HAVE_LOCAL] - (l:localTrack { name: "c:/users/octo/music/0-9/2 fabiola/evolution/01 - she's after my piano (remix '16).mp3"})
////RETURN l
//                return _graphClient.Cypher.Merge(
//                    "(ac:ArtistCredit{name:\""+music.Artist+ "\"})-[:CREDITED_ON]->(a:Album{name:\"" + music.Album + "\"})-[:PART_OF]-(b:Release{name:\"" + music.Album + "\"})-[:RELEASED_ON_MEDIUM]-(m:Cd{name:\"" + music.Album + "\"})-[:APPEARS_ON]-(t:Track{name:\"" + music.Title + "\"})")
//                    .OnCreate().Set("t.mbid=\""+Guid.NewGuid()+"\"");
//        }

//        private ICypherFluentQuery GetMergeInCollectionQuery(Music music)
//        {
//            throw new NotImplementedException();
//        }

        //private ICypherFluentQuery DeleteLocalTrack(Music music)
        //{
        //    return _graphClient.Cypher.Match("(l:localTrack)")
        //        .Where("l.name={filePath}")
        //        .WithParam("filePath", music.FilePath)
        //        .DetachDelete("l");
        //}

        //private ExecutionStatus ApplyChanges(Music music, Music sanitizedFileAnalyzed, Func<Music,ICypherFluentQuery> getQuery, bool deleteLocalTrackFirst, bool testOnly)
        //{
        //    if (deleteLocalTrackFirst)
        //    {
        //        music.Neo4jQuerries.Add(new Neo4jQuery
        //        {
        //            ExecutionStatus = ExecutionStatus.NotExecuted,
        //            DebugQuery = DeleteCurrentLocalTrack(sanitizedFileAnalyzed, getQuery).Query.DebugQueryText
        //        });
        //        if(!testOnly)
        //            DeleteCurrentLocalTrack(sanitizedFileAnalyzed, getQuery).ExecuteWithoutResults();
        //    }

        //    music.Neo4jQuerries.Add( new Neo4jQuery {ExecutionStatus=ExecutionStatus.NotExecuted,DebugQuery = UpdateLocalTrack(sanitizedFileAnalyzed,getQuery).Query.DebugQueryText});
        //    if (!testOnly)
        //        return (UpdateLocalTrack(sanitizedFileAnalyzed, getQuery).Results.Any())?ExecutionStatus.Found:ExecutionStatus.ExecutionError;
        //    return ExecutionStatus.NotExecuted;
        //}

        //private static ICypherFluentQuery<LocalTrack> UpdateLocalTrack(Music music, Func<Music, ICypherFluentQuery> getQuery)
        //{
        //    return getQuery(music)
        //        .With("t")
        //        .Limit(1)
        //        .Merge("(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:\"" + music.FilePath + "\"})")
        //        .Return(l => l.As<LocalTrack>());
        //}

        //private static ICypherFluentQuery DeleteCurrentLocalTrack(Music music, Func<Music, ICypherFluentQuery> getMatchQuery)
        //{
        //    return getMatchQuery(music)
        //        .With("t")
        //        .Limit(1)
        //        .Match("(t)-[lp]-(l:localTrack)")
        //        .DetachDelete("l");
        //}

        //public void CreateNew(IEnumerable<Music> filesAnalysed, bool testOnly)
        //{
        //    if (filesAnalysed == null)
        //        throw new ArgumentNullException(nameof(filesAnalysed));

        //    foreach (var music in filesAnalysed)
        //    {
        //        UpsertOneItem(music, music.CreateNeo4JUpdatingVersion(), GetMergeInCollectionQuery,
        //            GetMergehNotInCollectionQuery, testOnly, false);
        //    }
        //}

        public Music Check(Music music)
        {
            if (music.TagAnalysis.Id3TagIncomplete)
            {
                CheckMatchingProgress?.Invoke(this, new CheckMatchingProgressEventArgs(music.FilePath, ExecutionStatus.NotExecuted));
                return music;
            }
            return (music.TagAnalysis.MarkedAsPartOfCollection)
                ? CheckInCollection(music)
                : CheckNotInCollection(music);
        }

        private Music CheckInCollection(Music music)
        {
            //var sanitizedFileAnalysed = music.CreateNeo4JMatchingVersion();
            //var query = GetMatchInCollectionQuery(sanitizedFileAnalysed)
            //    .Return(t => t.As<MBEntry>()).Query;
            //music.Neo4JMatchingQuery = query.DebugQueryText;
            //if (music.Id3TagIncomplete)
            //{
            //    CheckMatchingProgress?.Invoke(this, new CheckMatchingProgressEventArgs(music.FilePath, MatchStatus.ErrorMatching));
            //    music.FixSuggestion = "Complete the ID3 Tag.";
            //    return music;
            //}
            //MBEntry result;
            //try
            //{
            //    result = GetMatchInCollectionQuery(sanitizedFileAnalysed).Return(t => t.As<MBEntry>()).Results.FirstOrDefault();
            //    music.MbId = (string.IsNullOrEmpty(result?.mbid)) ? Guid.Empty : new Guid(result.mbid);
            //    music.MatchStatus = (result == null) ? MatchStatus.UnMatched : MatchStatus.Matched;
            //    music.FixSuggestion = (result == null) ? "No suggestion" : "No Fix needed.";
            //    music.FixSuggestions = new FixSuggestion();

            //    CheckMatchingProgress?.Invoke(this,
            //    result == null
            //        ? new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", MatchStatus.UnMatched)
            //        : new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", MatchStatus.Matched));

            //}
            //catch (Exception e)
            //{
            //    music.MbId = Guid.Empty;
            //    music.MatchStatus = MatchStatus.ErrorMatching;
            //    CheckMatchingProgress?.Invoke(this,
            //        new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", MatchStatus.ErrorMatching));
            //}
            return music;
        }

        private ICypherFluentQuery GetMatchInCollectionQuery(MusicBase matchingVersion)
        {
            return _graphClient.Cypher.Match(
                    "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)-[:CREDITED_ON]-(tac:ArtistCredit)")
                .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
                .WithParam("albumName", "(?ui).*" + matchingVersion.Album + ".*")
                .AndWhere("t.name=~{title}")
                .WithParam("title", "(?ui).*" + matchingVersion.Title + ".*")
                .AndWhere("tac.name=~{artistName}")
                .WithParam("artistName", "(?ui).*" + matchingVersion.Artist + ".*")
                .AndWhere("ac.name=~{albumArtistName}")
                .WithParam("albumArtistName","(?ui)"+matchingVersion.AlbumArtist);
        }

        private Music CheckNotInCollection(Music music)
        {
            var matchingVersion = music.CreateNeo4JMatchingVersion();
            ExecutionStatus result= LoadArtist(music, matchingVersion.Artist.Name);
            if (result==ExecutionStatus.Found)
            {
                result = LoadAlbum(music,matchingVersion.Album.Name);
                if (result==ExecutionStatus.Found)
                {
                    result = LoadTitle(music,matchingVersion.Title.Name);
                    CheckMatchingProgress?.Invoke(this,
                        new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result));
                }
                else
                {
                    CheckMatchingProgress?.Invoke(this,
                        new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result));

                }
            }
            else
            {
                CheckMatchingProgress?.Invoke(this,
                    new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result));
            }
            return music;
        }

        private ExecutionStatus LoadTitle(Music music, string matchingVersionTitle)
        {
            var titleNotInCollectionQuery = GetMatchTitleInIndividualAlbumQuery(matchingVersionTitle, music.Album.Id);
            music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = ExecutionStatus.NotExecuted,
                DebugQuery = titleNotInCollectionQuery.Return(t => t.As<MBEntry>()).Query.DebugQueryText,
                QueryType = QueryType.TitleMatching
            });
            try
            {
                return LoadTitleInIndividualAlbumFromStore(music, titleNotInCollectionQuery);
            }
            catch (Exception e)
            {
                return ExecutionStatus.ExecutionError;
            }
        }

        private ExecutionStatus LoadTitleInIndividualAlbumFromStore(Music music, ICypherFluentQuery titleNotInCollectionQuery)
        {
            MBEntry result = null;
            var results = titleNotInCollectionQuery.Return(t => t.As<MBEntry>()).Results;
            result =
                results.FirstOrDefault(e => e.name.ToLower() == music.Title.Name.ToLower()) ??
                null;
            if (result != null)
            {
                music.Title.Id = new Guid(result.mbid);
                music.Title.Name = result.name;
                music.Title.FixSuggestion = "No Fix needed.";
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.TitleMatching) q.ExecutionStatus = ExecutionStatus.Found;
                });
                return ExecutionStatus.Found;
            }
            music.Neo4jQuerries.ForEach(q =>
            {
                if (q.QueryType == QueryType.TitleMatching) q.ExecutionStatus = ExecutionStatus.NotFound;
            });
            return ExecutionStatus.NotFound;
        }

        private ICypherFluentQuery GetMatchTitleInIndividualAlbumQuery(string matchingVersionTitle, Guid albumId)
        {
            return _graphClient.Cypher.Match("(b:Release)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
                .Where("b.mbid ={albumGuid}")
                .WithParam("albumGuid", albumId)
                .AndWhere("t.name =~{title}")
                .WithParam("title", "(?ui).*" + matchingVersionTitle + ".*");
            //return _graphClient.Cypher.Match(
            //        "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
            //    .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
            //    .WithParam("albumName", "(?ui).*" + matchingVersion.Album + ".*")
            //    .AndWhere("t.name=~{title}")
            //    .WithParam("title", "(?ui).*" + matchingVersion.Title + ".*")
            //    .AndWhere("ac.name=~{artistName}")
            //    .WithParam("artistName", "(?ui).*" + matchingVersion.Artist + ".*");
        }

        private ExecutionStatus LoadAlbum(Music music, string matchingVersionAlbumName)
        {
            var individualAlbumQuery = GetMatchIndividualAlbumQuery(matchingVersionAlbumName,music.Artist.Id);
            music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = ExecutionStatus.NotExecuted,
                DebugQuery = individualAlbumQuery.Return(a => a.As<MBEntry>()).Query.DebugQueryText,
                QueryType = QueryType.IndividualAlbumMatching
            });
            try
            {
                return TryLoadIndividualAlbumFromCache(music)
                    ? music.Neo4jQuerries.FirstOrDefault(q => q.QueryType == QueryType.IndividualAlbumMatching).ExecutionStatus
                    : LoadIndividualAlbumFromStore(music, individualAlbumQuery);
            }
            catch (Exception e)
            {
                return ExecutionStatus.ExecutionError;
            }
        }

        private ExecutionStatus LoadIndividualAlbumFromStore(Music music, ICypherFluentQuery individualAlbumQuery)
        {
            MBEntry result = null;
            var results = individualAlbumQuery.Return(b => b.As<MBEntry>()).Results;
            result =
                results.FirstOrDefault(e => e.name.ToLower() == music.Album.Name.ToLower() || e.disambiguation.ToLower() == music.Album.Name.ToLower()) ??
                null;
            if (result != null)
            {
                music.Album.Id = new Guid(result.mbid);
                music.Album.Name = result.name;
                music.Album.FixSuggestion = "No Fix needed.";
                _individualAlbumCache.Put(string.Format("{0}-{1}",music.Artist.Id,music.Album.Name), music.Album);
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.IndividualAlbumMatching) q.ExecutionStatus = ExecutionStatus.Found;
                });
                return ExecutionStatus.Found;
            }
            _individualAlbumCache.Put(string.Format("{0}-{1}", music.Artist.Id, music.Album.Name), music.Album);
            music.Neo4jQuerries.ForEach(q =>
            {
                if (q.QueryType == QueryType.IndividualAlbumMatching) q.ExecutionStatus = ExecutionStatus.NotFound;
            });
            return ExecutionStatus.NotFound;
        }

        private bool TryLoadIndividualAlbumFromCache(Music music)
        {
            var cachedIndividualAlbum = _individualAlbumCache.Get(string.Format("{0}-{1}",music.Artist.Id, music.Album.Name));
            if (cachedIndividualAlbum == null)
                return false;

            music.Album = cachedIndividualAlbum;
            if (music.Album.Id != Guid.Empty)
            {
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.IndividualAlbumMatching) q.ExecutionStatus = ExecutionStatus.Found;
                });
            }
            else
            {
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.IndividualAlbumMatching) q.ExecutionStatus = ExecutionStatus.NotFound;
                });
            }
            return true;
        }

        private ExecutionStatus LoadArtist(Music music, string matchingVersionArtistName)
        {
            var artistQuery = GetMatchArtistQuery(matchingVersionArtistName);
            music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = ExecutionStatus.NotExecuted,
                DebugQuery = artistQuery.Return(a => a.As<MBEntry>()).Query.DebugQueryText,
                QueryType = QueryType.ArtistMatching
            });
            try
            {
                return TryLoadArtistFromCache(music)
                    ? music.Neo4jQuerries.FirstOrDefault(q => q.QueryType == QueryType.ArtistMatching).ExecutionStatus
                    : LoadArtistFromStore(music, artistQuery);
            }
            catch (Exception e)
            {
                return ExecutionStatus.ExecutionError;
            }
        }

        private ExecutionStatus LoadArtistFromStore(Music music, ICypherFluentQuery matchingArtistQuery)
        {
            MBEntry result = null;
            var results =matchingArtistQuery.Return(a => a.As<MBEntry>()).Results;
            result =
                results.FirstOrDefault(e => e.name.ToLower() == music.Artist.Name.ToLower() || e.disambiguation.ToLower() == music.Artist.Name.ToLower()) ??
                null;
            if (result != null)
            {
                music.Artist.Id = new Guid(result.mbid);
                music.Artist.Name = result.name;
                music.Artist.FixSuggestion = "No Fix needed.";
                _artistCache.Put(music.Artist.Name, music.Artist);
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.ArtistMatching) q.ExecutionStatus = ExecutionStatus.Found;
                });
                return ExecutionStatus.Found;
            }
            _artistCache.Put(music.Artist.Name, music.Artist);
            music.Neo4jQuerries.ForEach(q =>
            {
                if (q.QueryType == QueryType.ArtistMatching) q.ExecutionStatus = ExecutionStatus.NotFound;
            });
            return ExecutionStatus.NotFound;
        }

        private bool TryLoadArtistFromCache(Music music)
        {
            var cachedArtist = _artistCache.Get(music.Artist.Name);
            if (cachedArtist == null)
                return false;

            music.Artist = cachedArtist;
            if (music.Artist.Id != Guid.Empty)
            {
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.ArtistMatching) q.ExecutionStatus = ExecutionStatus.Found;
                });
            }
            else
            {
                music.Neo4jQuerries.ForEach(q =>
                {
                    if (q.QueryType == QueryType.ArtistMatching) q.ExecutionStatus = ExecutionStatus.NotFound;
                });
            }
            return true;
        }

        private ICypherFluentQuery GetMatchArtistQuery(string matchingArtistName)
        {
            return _graphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)")
                .Where("ac.name =~{artistName}")
                .AndWhere("(a.name =~{artistName} or a.disambiguation =~{artistName})")
                .WithParam("artistName", "(?ui).*" + matchingArtistName + ".*");
            //return _graphClient.Cypher.Match(
            //        "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
            //    .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
            //    .WithParam("albumName", "(?ui).*" + matchingVersion.Album + ".*")
            //    .AndWhere("t.name=~{title}")
            //    .WithParam("title", "(?ui).*" + matchingVersion.Title + ".*")
            //    .AndWhere("ac.name=~{artistName}")
            //    .WithParam("artistName", "(?ui).*" + matchingVersion.Artist + ".*");
        }

        private ICypherFluentQuery GetMatchIndividualAlbumQuery(string matchingAlbumName, Guid artistGuid)
        {
            return _graphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)-[:CREDITED_ON]-(b:Release)")
                .Where("a.mbid ={artistGuid}")
                .WithParam("artistGuid", artistGuid)
                .AndWhere("(b.name =~{albumName} or a.disambiguation =~{albumName})")
                .WithParam("albumName", "(?ui).*" + matchingAlbumName + ".*");
            //return _graphClient.Cypher.Match(
            //        "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
            //    .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
            //    .WithParam("albumName", "(?ui).*" + matchingVersion.Album + ".*")
            //    .AndWhere("t.name=~{title}")
            //    .WithParam("title", "(?ui).*" + matchingVersion.Title + ".*")
            //    .AndWhere("ac.name=~{artistName}")
            //    .WithParam("artistName", "(?ui).*" + matchingVersion.Artist + ".*");
        }

        public IEnumerable<Music> CheckBulk(IEnumerable<Music> filesAnalysed)
        {
            foreach (var fileAnalysed in filesAnalysed)
            {
                if (StopActivity)
                    break;
                yield return Check(fileAnalysed);
            }
        }

        public event EventHandler<CheckMatchingProgressEventArgs> CheckMatchingProgress;
        public bool StopActivity { get; set; }
        //public event EventHandler<ApplyProgressEventArgs> ApplyProgress;
    }

    internal class LocalTrack
    {
        public string name { get; set; }
//        public string id { get; set; }
    }

    public class MBRecord
    {
        public string length { get; set; }
        
        public string position { get; set; }
    }
    public class MBEntry:MBRecord
    {
        public string mbid { get; set; }

        public string disambiguation { get; set; }

        public string name { get; set; }
    }

    public class MBArtistCredit:MBRecord
    {
        public string name { get; set; }
    }
}
