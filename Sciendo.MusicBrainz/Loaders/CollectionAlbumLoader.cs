using System.Collections.Generic;
using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    internal class CollectionAlbumLoader : Loader
    {
        public CollectionAlbumLoader(ItemMemoryCache cache) : base(cache)
        {
        }

        protected override string CacheKey => MatchingQuery.Music.Album.Name;

        protected override Item CurrentItem
        {
            get { return MatchingQuery.Music.Album; }
            set { MatchingQuery.Music.Album = value; }
        }
    }
}