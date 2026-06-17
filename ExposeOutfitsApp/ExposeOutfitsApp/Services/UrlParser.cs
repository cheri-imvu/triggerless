using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExposeOutfitsApp.Helpers
{
    public static class ImvuUrlParser
    {
        public static (List<long> roomIds, Dictionary<long, List<long>> userProducts) Parse(string url)
        {
            var roomIds = new List<long>();
            var userProducts = new Dictionary<long, List<long>>();

            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            foreach (string key in query)
            {
                if (key == null) continue;
                var value = query[key];
                if (string.IsNullOrWhiteSpace(value)) continue;

                value = HttpUtility.UrlDecode(value);

                if (key.StartsWith("avatar", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(key.Substring(6), out long userId))
                    {
                        var ids = value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s =>
                                       {
                                           long.TryParse(s, out long id);
                                           return id;
                                       })
                                       .Where(id => id > 0)
                                       .ToList();

                        if (ids.Count > 0)
                            userProducts[userId] = ids;
                    }
                }
                else if (key.Equals("room", StringComparison.OrdinalIgnoreCase))
                {
                    var ids = value.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(s => long.TryParse(s.Split('x')[0], out long id) ? id : 0)
                                   .Where(id => id > 0)
                                   .ToList();

                    roomIds.AddRange(ids);
                }
            }

            return (roomIds, userProducts);
        }
    }
}
