using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    public class MatchingArtistQuery:MatchingMusicBrainzQuery
    {
        public MatchingArtistQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)")
                .Where("ac.name =~{artistName}")
                .AndWhere("(a.name =~{artistName} or a.disambiguation =~{artistName})")
                .WithParam("artistName", "(?ui).*" + ProcessedParameter + ".*")
                .Return(a => a.As<MBEntry>());
        }

        public override QueryType QueryType => QueryType.ArtistMatching;

        protected override string ProcessedParameter {
            get { return Music.CreateNeo4JMatchingVersion().Artist.Name; }
        }

        protected override Guid ParentId => Guid.Empty;
        protected override Guid SecondParentId => Guid.Empty;
        public override Item CurrentItem => Music.Artist;
    }
}
