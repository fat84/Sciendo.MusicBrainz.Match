using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public class ArtistLoader:Loader
    {
        public ArtistLoader(ItemMemoryCache cache) : base(cache)
        {
        }

        protected override Item CurrentItem { get { return MatchingQuery.Music.Artist; } set
        {
            MatchingQuery.Music.Artist = value;
        } }

        protected override string CacheKey => MatchingQuery.Music.Artist.Name;
    }
}
