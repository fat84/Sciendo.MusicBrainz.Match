using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    public class MatchingTitleInIndividualAlbumQuery:MatchingMusicBrainzQuery
    {
        public MatchingTitleInIndividualAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Match("(b:Release)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)")
                .Where("b.mbid ={albumGuid}")
                .WithParam("albumGuid", ParentId)
                .AndWhere("t.name =~{title}")
                .WithParam("title", "(?ui).*" + ProcessedParameter + ".*")
                .Return(t => t.As<MBEntry>());
        }

        public override QueryType QueryType => QueryType.TitleMatching;
        protected override string ProcessedParameter => Music.CreateNeo4JMatchingVersion().Title.Name;
        protected override Guid ParentId => Music.Album.Id;
        protected override Guid SecondParentId => Guid.Empty;
        public override Item CurrentItem => Music.Title;
    }
}
