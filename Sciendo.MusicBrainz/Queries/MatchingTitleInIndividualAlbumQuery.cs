using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public class MatchingTitleInIndividualAlbumQuery:MatchingQuery
    {
        public MatchingTitleInIndividualAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery GetQuery()
        {
            return GraphClient.Cypher.Match("(b:Release)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
                .Where("b.mbid ={albumGuid}")
                .WithParam("albumGuid", ParentId)
                .AndWhere("t.name =~{title}")
                .WithParam("title", "(?ui).*" + MatchingVersion + ".*");

        }

        public override MBEntry Execute(string exactMatch)
        {
            var results = GetQuery().Return(t => t.As<MBEntry>()).Results;
            return results.FirstOrDefault(e => e.name.ToLower() == exactMatch.ToLower()) ??
                   results.FirstOrDefault();
        }

        public override string GetQueryForDisplay()
        {
            return GetQuery().Return(t => t.As<MBEntry>()).Query.DebugQueryText;
        }

        public override QueryType QueryType => QueryType.TitleMatching;
        protected override string MatchingVersion => Music.CreateNeo4JMatchingVersion().Title.Name;
        protected override Guid ParentId => Music.Album.Id;
        protected override Guid SecondParentId => Guid.Empty;
    }
}
