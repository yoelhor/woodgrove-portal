using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace woodgrove_portal.Pages
{

    public class CacheModel : PageModel
    {
        private IMemoryCache _cache;
        public List<string> Items = new List<string>();

        public CacheModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnGet()
        {
            var keys = GetCacheKeys();

            foreach (var key in keys)
            {
                if (_cache.TryGetValue(key, out string cacheValue))
                {
                    Items.Add(cacheValue);
                }

            }

        }

        private List<string> GetCacheKeys()
        {
            var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

            var coherentStateValue = coherentState.GetValue(_cache);
            var entriesCollection = coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var entriesCollectionValue = entriesCollection.GetValue(coherentStateValue) as ICollection;

            var keys = new List<string>();
            if (entriesCollectionValue != null)
            {
                foreach (var item in entriesCollectionValue)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    keys.Add(val.ToString());
                }
            }

            return keys;
        }
    }
}
