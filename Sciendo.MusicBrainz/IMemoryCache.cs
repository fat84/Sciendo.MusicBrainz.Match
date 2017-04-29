using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicBrainz
{
    public  interface IMemoryCache<T>
    {
        bool Put(string key, T item);

        T Get(string key);

        bool Delete(string key);
    }
}
