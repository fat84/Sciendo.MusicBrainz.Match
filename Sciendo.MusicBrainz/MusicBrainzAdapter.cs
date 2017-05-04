using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicBrainz.Loaders;
using Sciendo.MusicBrainz.Queries;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class MusicBrainzAdapter:IMusicBrainzAdapter
    {
        private readonly GraphClient _graphClient;
        private readonly IQueryFactory _queryFactory;
        private readonly ItemMemoryCache _artistCache;
        private readonly ItemMemoryCache _individualAlbumCache;
        private readonly ItemMemoryCache _collectionAlbumCache;
        private readonly ILoaderFactory _loaderFactory;

        public MusicBrainzAdapter(GraphClient graphClient, IQueryFactory queryFactory,ILoaderFactory loaderFactory)
        {
            _graphClient = graphClient;
            _queryFactory = queryFactory;
            _loaderFactory = loaderFactory;
            _artistCache=new ItemMemoryCache();
            _individualAlbumCache= new ItemMemoryCache();
            _collectionAlbumCache=new ItemMemoryCache();
            

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
                CheckMatchingProgress?.Invoke(this, new CheckMatchingProgressEventArgs(music.FilePath, ExecutionStatus.NotExecuted,QueryType.None));
                return music;
            }
            return (music.TagAnalysis.MarkedAsPartOfCollection)
                ? CheckAndLoad(music,QueryType.CollectionAlbumMatching,QueryType.TitleInCollectionMatching)
                : CheckAndLoad(music,QueryType.AlbumMatching, QueryType.TitleMatching);
        }

        private Music CheckAndLoad(Music music, QueryType albumQueryType, QueryType titleQueryType)
        {
            ExecutionStatus result =
    _loaderFactory.Get(QueryType.ArtistMatching).UsingMatchingQuery(
        _queryFactory.Get(QueryType.ArtistMatching, _graphClient).UsingMusic(music)).Load();
            if (result == ExecutionStatus.Found)
            {
                result =
                    _loaderFactory.Get(albumQueryType)
                        .UsingMatchingQuery(
                            _queryFactory.Get(albumQueryType, _graphClient)
                                .UsingMusic(music))
                        .Load();
                if (result == ExecutionStatus.Found)
                {
                    result =
                        _loaderFactory.Get(titleQueryType)
                            .UsingMatchingQuery(
                                _queryFactory.Get(titleQueryType, _graphClient)
                                    .UsingMusic(music))
                            .Load();
                    CheckMatchingProgress?.Invoke(this,
                        new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result, titleQueryType));
                }
                else
                {
                    CheckMatchingProgress?.Invoke(this,
                        new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result, albumQueryType));

                }
            }
            else
            {
                CheckMatchingProgress?.Invoke(this,
                    new CheckMatchingProgressEventArgs($"{music.Id} - {music.FilePath}", result, QueryType.ArtistMatching));
            }
            return music;
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
