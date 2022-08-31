using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using Triggerless.Models;
using Triggerless.XAFLib;
using static Triggerless.Services.Server.NVorbisService;
using log4net;

namespace Triggerless.Services.Server
{
    public class RipService
    {
        private ILog _log;

        public RipService(ILog log = null)
        {
            _log = log;
        }
        public static string GetUrlTemplate(int pid) => $"https://userimages-akm.imvu.com/productdata/{pid}/1/{{0}}";

        public static string GetUrl(int pid, string filename) => string.Format(GetUrlTemplate(pid), filename);

        public SongInfo GetSongInfo(int pid)
        {
            var dtStart = DateTime.Now;

            _log?.Debug($"{nameof(RipService)}.{nameof(GetSongInfo)} pid = {pid}");
            // Initialize variables
            SongInfo result = new SongInfo() { ProductID = pid };
            var contentsUrl = GetUrl(pid, "_contents.json");
            var indexUrl = GetUrl(pid, "index.xml");
            ProductList productList;
            Template template = null;

            // Populate productList and template
            using (var client = new HttpClient())
            {

                // Get the Product list
                _log?.Debug($"\tAcquiring contents");
                var responseJson = client.GetStringAsync(contentsUrl).Result;

                // cast into JSON we can deserialize
                var json = $"{{productArray: {responseJson}}}";
                _log?.Debug($"\tDeserializing product list");
                productList = JsonConvert.DeserializeObject<ProductList>(json);

                // remove any non-OGG assets
                _log?.Debug($"\tRemoving non-OGG assets");
                productList.productArray = productList.productArray.Where(p => p.name.ToLower().EndsWith(".ogg")).ToArray();

                // in cases where url is omitted, we would use name instead. Here we'll just populate null urls for ease of use
                foreach (var item in productList.productArray.Where(i => i.url == null))
                {
                    item.url = item.name;
                }

                // Get index.xml template, and deserialize using XAFLib Template class
                _log?.Debug($"\tAcquiring index.xml");
                var responseText = client.GetStringAsync(indexUrl).Result;
                _log?.Debug($"\tLoading Template");
                try {
                    template = Template.LoadXml(responseText);
                } catch (Exception exc)
                {
                    _log?.Error("Unable to load template", exc);
                }                
            }

            // Start populating the entries in our result
            foreach (var action in template?.Actions)
            {
                // Here, name is the name of the OGG file
                var name = action.Sound?.Name;
                if (string.IsNullOrWhiteSpace(name)) continue;

                // Create our initial entry and add it to the result entries
                SongEntry entry = new SongEntry();
                entry.Trigger = action.Name;
                var seq = GetSequence(action.Name.Split(new[] { ',' })[0]);
                entry.Sequence = seq.Item1;
                entry.TriggerPrefix = seq.Item2;
                entry.Filename = name;
                entry.Location = productList.productArray.Where(e => e.name == name).Select(e => e.url).First();
                result.Entries.Add(entry);
            }
            _log?.Debug($"\t{template.Actions.Count} triggers cued up.");

            // Now get all the OGG clip lengths
            var request = new GetLengthsRequest { PID = pid, Locations = result.Entries.Select(e => e.Location) };
            var response = new NVorbisService(_log).GetLengths(request);

            // update all our result entries with the clip length in milliseconds
            foreach (var item in response.Results)
            {
                var entry = result.Entries.First(e => e.Location == item.Item1); //Item1 is the location
                if (entry != null)
                {
                    entry.Length = item.Item2; //Item2 is the clip length
                }
            }

            // Now try to order the triggers in sequential order (because it makes things easier)
            result.Entries = result.Entries.OrderBy(e => e.Sequence).ToList();
            var ms = (DateTime.Now - dtStart).TotalMilliseconds;
            _log?.Debug($"GetSongInfo completed in {ms} milliseconds");
            result.Success = true;
            return result;
        }

        private (double, string) GetSequence(string trigger)
        {
            char[] DIGITS = "0123456789".ToArray();
            string letters = string.Empty;
            string numbers = string.Empty;
            foreach(char c in trigger)
            {
                if (DIGITS.Contains(c))
                {
                    numbers += c;
                } else
                {
                    letters += c;
                }
            }
            return (double.Parse(numbers), letters);
        }
    }
}
