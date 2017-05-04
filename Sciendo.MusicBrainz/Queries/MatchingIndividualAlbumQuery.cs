using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public class MatchingIndividualAlbumQuery:MatchingQuery
    {
        public MatchingIndividualAlbumQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        protected override ICypherFluentQuery GetQuery()
        {
            return GraphClient.Cypher.Match("(a:Artist)-[:CREDITED_AS]-(ac:ArtistCredit)-[:CREDITED_ON]-(b:Release)")
                .Where("a.mbid ={artistGuid}")
                .WithParam("artistGuid", ParentId)
                .AndWhere("(b.name =~{albumName} or b.disambiguation =~{albumName})")
                .WithParam("albumName", "(?ui).*" + MatchingVersion + ".*");

        }

        public override MBEntry Execute(string exactMatch)
        {
            var results = GetQuery().Return(b => b.As<MBEntry>()).Results;
            return results.FirstOrDefault(e => e.name.ToLower() == exactMatch.ToLower() || e.disambiguation.ToLower() == exactMatch.ToLower()) ??
                results.FirstOrDefault();
        }
        public override string GetQueryForDisplay()
        {
            return GetQuery().Return(b => b.As<MBEntry>()).Query.DebugQueryText;
        }

        public override QueryType QueryType => QueryType.AlbumMatching;

        protected override string MatchingVersion => Music.CreateNeo4JMatchingVersion().Album.Name;
        protected override Guid ParentId => Music.Artist.Id;
        protected override Guid SecondParentId => Guid.Empty;
    }
}
