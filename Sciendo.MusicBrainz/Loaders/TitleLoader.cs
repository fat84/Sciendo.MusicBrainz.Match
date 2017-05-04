using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public class TitleLoader:Loader
    {
        public TitleLoader(ItemMemoryCache cache) : base(cache)
        {
        }

        protected override Item CurrentItem {
            get { return MatchingQuery.Music.Title; }
            set { MatchingQuery.Music.Title = value; } }

        protected override string CacheKey => string.Empty;
    }
}
