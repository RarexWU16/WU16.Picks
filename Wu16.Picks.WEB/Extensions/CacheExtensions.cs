using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wu16.Picks.WEB.Extensions
{
    public static class CacheExtensions
    {
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var value = cache.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value)
        {
            cache.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
