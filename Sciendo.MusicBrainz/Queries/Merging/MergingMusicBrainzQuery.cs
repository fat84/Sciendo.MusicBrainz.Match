using System;
using System.Linq;
using Neo4jClient;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Merging
{
    public abstract class MergingMusicBrainzQuery:MusicBrainzQuery
    {
        private readonly bool _simulateOnly;

        protected MergingMusicBrainzQuery(GraphClient graphClient, bool simulateOnly) : base(graphClient)
        {
            _simulateOnly = simulateOnly;
        }
        public MergingMusicBrainzQuery UsingMusic(Music music)
        {
            if (music == null)
                throw new ArgumentNullException(nameof(music));
            Music = music;
            return this;
        }

        public override MBEntry Execute()
        {
            if(!_simulateOnly)
                return GetQuery().Results.FirstOrDefault();
            return null;
        }
    }
}
