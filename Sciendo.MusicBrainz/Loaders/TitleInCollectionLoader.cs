using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    internal class TitleInCollectionLoader : Loader
    {
        public TitleInCollectionLoader(ItemMemoryCache cache) : base(cache)
        {
        }

        protected override string CacheKey => string.Empty;
        protected override Item CurrentItem
        {
            get { return MatchingQuery.Music.Title; }
                set { MatchingQuery.Music.Title = value; } }
    }
}