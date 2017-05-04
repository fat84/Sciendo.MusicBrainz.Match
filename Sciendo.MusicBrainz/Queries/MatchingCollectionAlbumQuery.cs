using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    internal class MatchingCollectionAlbumQuery : MatchingQuery
    {
        public MatchingCollectionAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public override QueryType QueryType => QueryType.CollectionAlbumMatching;
        protected override string MatchingVersion => Music.CreateNeo4JMatchingVersion().Album.Name;
        protected override Guid ParentId => Guid.Empty;
        protected override ICypherFluentQuery GetQuery()
        {
            return GraphClient.Cypher.Match("(b:Release)")
    .Where("(b.name =~{albumName} or b.disambiguation =~{albumName})")
    .WithParam("albumName", "(?ui).*" + MatchingVersion + ".*");

        }

        public override MBEntry Execute(string exactMatch)
        {
            var results = GetQuery().Return(b => b.As<MBEntry>()).Results;
            return results.FirstOrDefault(e => e.name.ToLower() == exactMatch.ToLower() || e.disambiguation.ToLower() == exactMatch.ToLower()) ??
                results.FirstOrDefault();
        }

        protected override Guid SecondParentId => Guid.Empty;
        public override string GetQueryForDisplay()
        {
            return GetQuery().Return(b => b.As<MBEntry>()).Query.DebugQueryText;
        }
    }
}