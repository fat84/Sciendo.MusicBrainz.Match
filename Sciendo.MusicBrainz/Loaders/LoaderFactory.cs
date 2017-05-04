using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public class LoaderFactory : ILoaderFactory
    {
        private readonly ItemMemoryCache _artistCache;
        private readonly ItemMemoryCache _albumCache;

        public LoaderFactory(ItemMemoryCache artistCache, ItemMemoryCache albumCache)
        {
            _artistCache = artistCache;
            _albumCache = albumCache;
        }
        public Loader Get(QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.ArtistMatching:
                    return new ArtistLoader(_artistCache);
                case QueryType.AlbumMatching:
                    return new AlbumLoader(_albumCache);
                case QueryType.TitleMatching:
                    return new TitleLoader(null);
                case QueryType.CollectionAlbumMatching:
                    return new CollectionAlbumLoader(_albumCache);
                case QueryType.TitleInCollectionMatching:
                    return new TitleInCollectionLoader(null);
                default:
                    return new ArtistLoader(_artistCache);
            }
        }
    }
}
