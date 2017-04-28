using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class MusicBrainzAdapter:IMusicBrainzAdapter
    {
        private readonly GraphClient _graphClient;

        public MusicBrainzAdapter(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }
        public void LinkToExisting(IEnumerable<FileAnalysed> filesAnalysed, bool forceCreate, bool testOnly)
        {
            if(filesAnalysed==null)
                throw new ArgumentNullException(nameof(filesAnalysed));

            foreach (var fileAnalysed in filesAnalysed)
            {

                var applyStatus = UpsertOneItem(fileAnalysed, fileAnalysed.CreateNeo4JMatchingVersion(),
                    GetMatchInCollectionQuery,
                    GetMatchNotInCollectionQuery, testOnly, true);
                if (forceCreate && applyStatus == ApplyStatus.ErrorApplying)
                    UpsertOneItem(fileAnalysed, fileAnalysed.CreateNeo4JUpdatingVersion(), GetMergeInCollectionQuery,
                        GetMergehNotInCollectionQuery, testOnly, false);
            }
        }

        private ApplyStatus UpsertOneItem(FileAnalysed fileAnalysed, FileAnalysed workVersion, 
            Func<FileAnalysed,ICypherFluentQuery> getInCollectionQuery, 
            Func<FileAnalysed,ICypherFluentQuery> getNotInCollectionQuery, 
            bool testOnly,
            bool deletePreviousLocalTrack=false )
        {
            fileAnalysed.Neo4jApplyQuerries = new List<Neo4jApplyQuery>();
            if (deletePreviousLocalTrack)
            {
                fileAnalysed.Neo4jApplyQuerries.Add(new Neo4jApplyQuery
                {
                    ApplyStatus = ApplyStatus.None,
                    DebugQuery = DeleteLocalTrack(workVersion).Query.DebugQueryText
                });
                if(!testOnly)
                    DeleteLocalTrack(workVersion).ExecuteWithoutResults();
            }

            var applyStatus = ApplyStatus.None;
            if (fileAnalysed.MarkedAsPartOfCollection)
            {
                applyStatus = ApplyChanges(fileAnalysed, workVersion, getInCollectionQuery, deletePreviousLocalTrack, testOnly);
                if (ApplyProgress != null)
                    ApplyProgress(this, new ApplyProgressEventArgs(fileAnalysed.FilePath, applyStatus));
                return applyStatus;
            }
            applyStatus = ApplyChanges(fileAnalysed, workVersion, getNotInCollectionQuery,
            deletePreviousLocalTrack, testOnly);
            if (ApplyProgress != null)
                ApplyProgress(this, new ApplyProgressEventArgs(fileAnalysed.FilePath, applyStatus));
            return applyStatus;
        }

        private ICypherFluentQuery GetMergehNotInCollectionQuery(FileAnalysed fileAnalysed)
        {
//            MERGE(ac: ArtistCredit{ name: "2 Fabiola"})-[:CREDITED_ON]->(a: Album{ name: "Evolution"})-[:PART_OF] - (b:Release{ name: "Evolution"})-[:RELEASED_ON_MEDIUM] - (m:Cd{ name: "Evolution"})-[:APPEARS_ON] - (t:Track{ name: "She's after My piano (remix '16)"})
//with t
//limit 1
//MERGE(t) -[lr: HAVE_LOCAL] - (l:localTrack { name: "c:/users/octo/music/0-9/2 fabiola/evolution/01 - she's after my piano (remix '16).mp3"})
//RETURN l
                return _graphClient.Cypher.Merge(
                    "(ac:ArtistCredit{name:\""+fileAnalysed.Artist+ "\"})-[:CREDITED_ON]->(a:Album{name:\"" + fileAnalysed.Album + "\"})-[:PART_OF]-(b:Release{name:\"" + fileAnalysed.Album + "\"})-[:RELEASED_ON_MEDIUM]-(m:Cd{name:\"" + fileAnalysed.Album + "\"})-[:APPEARS_ON]-(t:Track{name:\"" + fileAnalysed.Title + "\"})")
                    .OnCreate().Set("t.mbid=\""+Guid.NewGuid()+"\"");
        }

        private ICypherFluentQuery GetMergeInCollectionQuery(FileAnalysed fileAnalysed)
        {
            throw new NotImplementedException();
        }

        private ICypherFluentQuery DeleteLocalTrack(FileAnalysed fileAnalysed)
        {
            return _graphClient.Cypher.Match("(l:localTrack)")
                .Where("l.name={filePath}")
                .WithParam("filePath", fileAnalysed.FilePath)
                .DetachDelete("l");
        }

        private ApplyStatus ApplyChanges(FileAnalysed fileAnalysed, FileAnalysed sanitizedFileAnalyzed, Func<FileAnalysed,ICypherFluentQuery> getQuery, bool deleteLocalTrackFirst, bool testOnly)
        {
            if (deleteLocalTrackFirst)
            {
                fileAnalysed.Neo4jApplyQuerries.Add(new Neo4jApplyQuery
                {
                    ApplyStatus = ApplyStatus.None,
                    DebugQuery = DeleteCurrentLocalTrack(sanitizedFileAnalyzed, getQuery).Query.DebugQueryText
                });
                if(!testOnly)
                    DeleteCurrentLocalTrack(sanitizedFileAnalyzed, getQuery).ExecuteWithoutResults();
            }

            fileAnalysed.Neo4jApplyQuerries.Add( new Neo4jApplyQuery {ApplyStatus=ApplyStatus.None,DebugQuery = UpdateLocalTrack(sanitizedFileAnalyzed,getQuery).Query.DebugQueryText});
            if (!testOnly)
                return (UpdateLocalTrack(sanitizedFileAnalyzed, getQuery).Results.Any())?ApplyStatus.Ok:ApplyStatus.ErrorApplying;
            return ApplyStatus.None;
        }

        private static ICypherFluentQuery<LocalTrack> UpdateLocalTrack(FileAnalysed fileAnalysed, Func<FileAnalysed, ICypherFluentQuery> getQuery)
        {
            return getQuery(fileAnalysed)
                .With("t")
                .Limit(1)
                .Merge("(t)-[lr: HAVE_LOCAL]-(l:localTrack {name:\"" + fileAnalysed.FilePath + "\"})")
                .Return(l => l.As<LocalTrack>());
        }

        private static ICypherFluentQuery DeleteCurrentLocalTrack(FileAnalysed fileAnalysed, Func<FileAnalysed, ICypherFluentQuery> getMatchQuery)
        {
            return getMatchQuery(fileAnalysed)
                .With("t")
                .Limit(1)
                .Match("(t)-[lp]-(l:localTrack)")
                .DetachDelete("l");
        }

        public void CreateNew(IEnumerable<FileAnalysed> filesAnalysed, bool testOnly)
        {
            if (filesAnalysed == null)
                throw new ArgumentNullException(nameof(filesAnalysed));

            foreach (var fileAnalysed in filesAnalysed)
            {
                UpsertOneItem(fileAnalysed, fileAnalysed.CreateNeo4JUpdatingVersion(), GetMergeInCollectionQuery,
                    GetMergehNotInCollectionQuery, testOnly, false);
            }
        }

        public FileAnalysed Check(FileAnalysed fileAnalysed)
        {
            return (fileAnalysed.MarkedAsPartOfCollection)
                ? CheckInCollection(fileAnalysed)
                : CheckNotInCollection(fileAnalysed);
        }

        private FileAnalysed CheckInCollection(FileAnalysed fileAnalysed)
        {
            var sanitizedFileAnalysed = fileAnalysed.CreateNeo4JMatchingVersion();
            var query = GetMatchInCollectionQuery(sanitizedFileAnalysed)
                .Return(t => t.As<MBEntry>()).Query;
            fileAnalysed.Neo4JMatchingQuery = query.DebugQueryText;
            if (fileAnalysed.Id3TagIncomplete)
            {
                CheckMatchingProgress?.Invoke(this, new CheckMatchingProgressEventArgs(fileAnalysed.FilePath, MatchStatus.ErrorMatching));
                fileAnalysed.FixSuggestion = "Complete the ID3 Tag.";
                return fileAnalysed;
            }
            MBEntry result;
            try
            {
                result = GetMatchInCollectionQuery(sanitizedFileAnalysed).Return(t => t.As<MBEntry>()).Results.FirstOrDefault();
                fileAnalysed.MbId = (string.IsNullOrEmpty(result?.mbid)) ? Guid.Empty : new Guid(result.mbid);
                fileAnalysed.MatchStatus = (result == null) ? MatchStatus.UnMatched : MatchStatus.Matched;
                fileAnalysed.FixSuggestion = (result == null) ? "No suggestion" : "No Fix needed.";
                fileAnalysed.FixSuggestions = new FixSuggestion();

                CheckMatchingProgress?.Invoke(this,
                result == null
                    ? new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.UnMatched)
                    : new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.Matched));

            }
            catch (Exception e)
            {
                fileAnalysed.MbId = Guid.Empty;
                fileAnalysed.MatchStatus = MatchStatus.ErrorMatching;
                CheckMatchingProgress?.Invoke(this,
                    new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.ErrorMatching));
            }
            return fileAnalysed;
        }

        private ICypherFluentQuery GetMatchInCollectionQuery(FileAnalysed sanitizedFileAnalysed)
        {
            return _graphClient.Cypher.Match(
                    "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)-[:CREDITED_ON]-(tac:ArtistCredit)")
                .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
                .WithParam("albumName", "(?ui).*" + sanitizedFileAnalysed.Album + ".*")
                .AndWhere("t.name=~{title}")
                .WithParam("title", "(?ui).*" + sanitizedFileAnalysed.Title + ".*")
                .AndWhere("tac.name=~{artistName}")
                .WithParam("artistName", "(?ui).*" + sanitizedFileAnalysed.Artist + ".*")
                .AndWhere("ac.name=~{albumArtistName}")
                .WithParam("albumArtistName","(?ui)"+sanitizedFileAnalysed.AlbumArtist);
        }


        //MATCH (ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m:Cd)-[:APPEARS_ON]-(t:Track)
        //WHERE a.name=~"(?ui).*True Blue.*"
        //and t.name=~"(?ui).*Open Your Heart.*"
        //and ac.name=~"(?ui).*Madonna.*"//
        //Return t.mbid
        //limit 1
        private FileAnalysed CheckNotInCollection(FileAnalysed fileAnalysed)
        {
            var sanitizedFileAnalysed = fileAnalysed.CreateNeo4JMatchingVersion();
            var query = GetMatchNotInCollectionQuery(sanitizedFileAnalysed)
                .Return(t => t.As<MBEntry>()).Query;
            fileAnalysed.Neo4JMatchingQuery = query.DebugQueryText;
            if (fileAnalysed.Id3TagIncomplete)
            {
                CheckMatchingProgress?.Invoke(this, new CheckMatchingProgressEventArgs(fileAnalysed.FilePath,MatchStatus.ErrorMatching));
                fileAnalysed.FixSuggestion = "Complete the ID3 Tag.";
                return fileAnalysed;
            }
            MBEntry result;
            try
            {
                result = GetMatchNotInCollectionQuery(sanitizedFileAnalysed).Return(t => t.As<MBEntry>()).Results.FirstOrDefault();
                fileAnalysed.MbId = (string.IsNullOrEmpty(result?.mbid)) ? Guid.Empty : new Guid(result.mbid);
                fileAnalysed.MatchStatus = (result == null) ? MatchStatus.UnMatched : MatchStatus.Matched;
                fileAnalysed.FixSuggestion = (result == null) ? "No suggestion" : "No Fix needed.";
                fileAnalysed.FixSuggestions = new FixSuggestion();

                CheckMatchingProgress?.Invoke(this,
                result == null
                    ? new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.UnMatched)
                    : new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.Matched));

            }
            catch (Exception e)
            {
                fileAnalysed.MbId = Guid.Empty;
                fileAnalysed.MatchStatus = MatchStatus.ErrorMatching;
                CheckMatchingProgress?.Invoke(this,
                    new CheckMatchingProgressEventArgs($"{fileAnalysed.Id} - {fileAnalysed.FilePath}", MatchStatus.ErrorMatching));
            }
            return fileAnalysed;
        }

        private ICypherFluentQuery GetMatchNotInCollectionQuery(FileAnalysed sanitizedFileAnalysed)
        {
            return _graphClient.Cypher.Match(
                    "(ac:ArtistCredit)-[:CREDITED_ON]->(a:Album)-[:PART_OF]-(b)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
                .Where("(a.name=~{albumName} OR a.disambiguation=~{albumName})")
                .WithParam("albumName", "(?ui).*" + sanitizedFileAnalysed.Album + ".*")
                .AndWhere("t.name=~{title}")
                .WithParam("title", "(?ui).*" + sanitizedFileAnalysed.Title + ".*")
                .AndWhere("ac.name=~{artistName}")
                .WithParam("artistName", "(?ui).*" + sanitizedFileAnalysed.Artist + ".*");
        }

        public IEnumerable<FileAnalysed> CheckBulk(IEnumerable<FileAnalysed> filesAnalysed)
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
        public event EventHandler<ApplyProgressEventArgs> ApplyProgress;
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
