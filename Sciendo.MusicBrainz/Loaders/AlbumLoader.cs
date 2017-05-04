using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public class AlbumLoader : Loader
    {
        public AlbumLoader(ItemMemoryCache cache) : base(cache)
        {
        }

        protected override Item CurrentItem
        {
            get { return MatchingQuery.Music.Album; }
            set { MatchingQuery.Music.Album = value; }
        }

        protected override string CacheKey => $"{MatchingQuery.Music.Artist.Id}-{MatchingQuery.Music.Album.Name}";
    }
}
