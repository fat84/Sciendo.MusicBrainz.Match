using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public class MatchingArtistQuery:MatchingQuery
    {
        public MatchingArtistQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery GetQuery()
        {
            return GraphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)")
                .Where("ac.name =~{artistName}")
                .AndWhere("(a.name =~{artistName} or a.disambiguation =~{artistName})")
                .WithParam("artistName", "(?ui).*" + MatchingVersion + ".*");
        }

        public override MBEntry Execute(string exactMatch)
        {
            var results = GetQuery().Return(a => a.As<MBEntry>()).Results;
            return results.FirstOrDefault(e => e.name.ToLower() == exactMatch.ToLower() || e.disambiguation.ToLower() == exactMatch.ToLower()) ??
                null;
        }

        public override QueryType QueryType => QueryType.ArtistMatching;

        protected override string MatchingVersion {
            get { return Music.CreateNeo4JMatchingVersion().Artist.Name; }
        }

        protected override Guid ParentId => Guid.Empty;
        protected override Guid SecondParentId => Guid.Empty;
        public override string GetQueryForDisplay()
        {
            return GetQuery().Return(a => a.As<MBEntry>()).Query.DebugQueryText;
        }
    }
}
