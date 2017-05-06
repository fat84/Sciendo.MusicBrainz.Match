using System;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicBrainz.Queries.Matching;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Merging
{
    internal class MergingArtistQuery : MergingMusicBrainzQuery
    {
        public MergingArtistQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public override QueryType QueryType => QueryType.ArtistMerging;
        protected override string ProcessedParameter => Music.CreateNeo4JUpdatingVersion().Artist.Name;
        protected override Guid ParentId => Guid.Empty;
        protected override Guid SecondParentId => Guid.Empty;
        protected override ICypherFluentQuery<MBEntry> GetQuery()
        {
            return GraphClient.Cypher.Merge("a: Artist{ name: \"" + ProcessedParameter +
                                            "\"})-[c: CREDITED_AS] - (ac:ArtistCredit{ name: \"" + ProcessedParameter +
                                            "\"}")
                .OnCreate()
                .Set("a.mbid = " + Guid.NewGuid() + ")")
                .Set("a.disambiguation = \"" + ProcessedParameter + "\"")
                .Set("a.length=0")
                .Set("a.position=0")
                .Set("c.motion=0")
                .Set("c.year=0")
                .Set("c.position=0")
                .Set("c.join=\"\"")
                .Set("c.day=0")
                .Set("ac.length=0")
                .Set("ac.position=0")
                .Return(a => a.As<MBEntry>());
        }

        public override Item CurrentItem => Music.Artist;
    }
}