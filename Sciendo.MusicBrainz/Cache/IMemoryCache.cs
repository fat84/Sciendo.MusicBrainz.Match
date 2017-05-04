namespace Sciendo.MusicBrainz.Cache
{
    public  interface IMemoryCache<T>
    {
        bool Put(string key, T item);

        T Get(string key);

        bool Delete(string key);
    }
}
