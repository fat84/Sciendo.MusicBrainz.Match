using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class ItemMemoryCache:IMemoryCache<Item>
    {
        private readonly Dictionary<string, Item> _store;

        internal ItemMemoryCache()
        {
            _store= new Dictionary<string, Item>();
        }
        public bool Put(string key, Item item)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if(item==null)
                throw new ArgumentNullException(nameof(item));
            if (Get(key) == null)
            {
                _store.Add(key.ToLower(),item);
                return true;
            }
            return false;
        }

        public Item Get(string key)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (_store.ContainsKey(key.ToLower()))
                return _store[key.ToLower()];
            return null;
        }

        public bool Delete(string key)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return _store.Remove(key);
        }
    }
}
