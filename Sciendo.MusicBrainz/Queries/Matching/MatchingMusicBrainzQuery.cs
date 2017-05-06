using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Matching
{
    public abstract class MatchingMusicBrainzQuery:MusicBrainzQuery
    {
        public MatchingMusicBrainzQuery(GraphClient graphClient) : base(graphClient)
        {
        }

        public MatchingMusicBrainzQuery UsingMusic(Music music)
        {
            if (music == null)
                throw new ArgumentNullException(nameof(music));
            Music = music;
            return this;
        }

        public override MBEntry Execute()
        {
            var results = GetQuery().Results;
            return results.FirstOrDefault(e => e.name.ToLower() == CurrentItem.Name.ToLower() || e.disambiguation.ToLower() == CurrentItem.Name.ToLower()) ??
                results.FirstOrDefault();
        }

    }
}
