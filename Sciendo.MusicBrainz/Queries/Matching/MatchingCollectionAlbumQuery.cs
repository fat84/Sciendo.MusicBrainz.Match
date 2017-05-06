using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    internal class MatchingCollectionAlbumQuery : MatchingMusicBrainzQuery
    {
        public MatchingCollectionAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public override QueryType QueryType => QueryType.CollectionAlbumMatching;
        protected override string ProcessedParameter => Music.CreateNeo4JMatchingVersion().Album.Name;
        protected override Guid ParentId => Guid.Empty;
        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Match("(b:Release)")
                .Where("(b.name =~{albumName} or b.disambiguation =~{albumName})")
                .WithParam("albumName", "(?ui).*" + ProcessedParameter + ".*")
                .Return(b => b.As<MBEntry>());

        }

        protected override Guid SecondParentId => Guid.Empty;
        public override Item CurrentItem => Music.Album;
    }
}