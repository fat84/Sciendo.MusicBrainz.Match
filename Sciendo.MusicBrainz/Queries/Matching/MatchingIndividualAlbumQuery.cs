using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    public class MatchingIndividualAlbumQuery:MatchingMusicBrainzQuery
    {
        public MatchingIndividualAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)-[:CREDITED_ON]-(b:Release)")
                .Where("a.mbid ={artistGuid}")
                .WithParam("artistGuid", ParentId)
                .AndWhere("(b.name =~{albumName} or b.disambiguation =~{albumName})")
                .WithParam("albumName", "(?ui).*" + ProcessedParameter + ".*")
                .Return(b=>b.As<MBEntry>());

        }

        public override QueryType QueryType => QueryType.AlbumMatching;

        protected override string ProcessedParameter => Music.CreateNeo4JMatchingVersion().Album.Name;
        protected override Guid ParentId => Music.Artist.Id;
        protected override Guid SecondParentId => Guid.Empty;
        public override Item CurrentItem => Music.Album;
    }
}
