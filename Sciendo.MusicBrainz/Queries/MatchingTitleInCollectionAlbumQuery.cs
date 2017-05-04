using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    internal class MatchingTitleInCollectionAlbumQuery : MatchingQuery
    {
        public MatchingTitleInCollectionAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public override QueryType QueryType => QueryType.TitleInCollectionMatching;
        protected override string MatchingVersion => Music.CreateNeo4JMatchingVersion().Title.Name;
        protected override Guid ParentId => Music.Album.Id;
        protected override ICypherFluentQuery GetQuery()
        {
            return GraphClient.Cypher.Match("(b:Release)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)-[:CREDITED_ON]-(tac:ArtistCredit)-[:CREDITED_AS]-(a:Artist)")
    .Where("b.mbid ={albumGuid}")
    .WithParam("albumGuid", ParentId)
    .AndWhere("a.mbid ={artistGuid}")
    .WithParam("artistGuid", SecondParentId)
    .AndWhere("t.name =~{title}")
    .WithParam("title", "(?ui).*" + MatchingVersion + ".*");

        }
        public override string GetQueryForDisplay()
        {
            return GetQuery().Return(t => t.As<MBEntry>()).Query.DebugQueryText;
        }

        public override MBEntry Execute(string exactMatch)
        {
            var results = GetQuery().Return(t => t.As<MBEntry>()).Results;
            return results.FirstOrDefault(e => e.name.ToLower() == exactMatch.ToLower()) ??
                   results.FirstOrDefault();
        }

        protected override Guid SecondParentId => Music.Artist.Id;
    }
}