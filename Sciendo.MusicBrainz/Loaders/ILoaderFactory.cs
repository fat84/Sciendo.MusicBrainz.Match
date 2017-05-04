using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public interface ILoaderFactory
    {
        Loader Get(QueryType queryType);
    }
}