using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    internal class MatchingTitleInCollectionAlbumQuery : MatchingMusicBrainzQuery
    {
        public MatchingTitleInCollectionAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public override QueryType QueryType => QueryType.TitleInCollectionMatching;
        protected override string ProcessedParameter => Music.CreateNeo4JMatchingVersion().Title.Name;
        protected override Guid ParentId => Music.Album.Id;
        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Match(
                    "(b:Release)-[:RELEASED_ON_MEDIUM]-(m)-[:APPEARS_ON]-(t:Track)-[:CREDITED_ON]-(tac:ArtistCredit)-[:CREDITED_AS]-(a:Artist)")
                .Where("b.mbid ={albumGuid}")
                .WithParam("albumGuid", ParentId)
                .AndWhere("a.mbid ={artistGuid}")
                .WithParam("artistGuid", SecondParentId)
                .AndWhere("t.name =~{title}")
                .WithParam("title", "(?ui).*" + ProcessedParameter + ".*")
                .Return(t => t.As<MBEntry>());

        }

        protected override Guid SecondParentId => Music.Artist.Id;
        public override Item CurrentItem => Music.Title;
    }
}