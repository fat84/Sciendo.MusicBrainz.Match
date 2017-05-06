using System;
using System.Linq;
using Neo4jClient;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Merging
{
    public abstract class MergingMusicBrainzQuery:MusicBrainzQuery
    {
        protected MergingMusicBrainzQuery(GraphClient graphClient) : base(graphClient)
        {
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
            return GetQuery().Results.FirstOrDefault();
       }
    }
}
