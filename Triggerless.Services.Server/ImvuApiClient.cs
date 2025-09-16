using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Triggerless.Models;
using Triggerless.XAFLib;



namespace Triggerless.Services.Server
{
    public class ImvuApiClient : IDisposable
    {
        private readonly ImvuApiService _service;
        private readonly ILog _log;

        public ImvuApiClient(ILog log = null)
        {
            _log = log;
            _service = new ImvuApiService();
        }

        public async Task<ImvuProduct> GetProduct(long productId)
        {
#if USE_LOCAL
            return new ImvuProduct { 
                Id = productId,
                Name = $"Product {productId}",
                CreatorId = 0,
                CreatorName = "FakeUser",
                ProductImage = "https://triggerless.com/Content/icon.png"
            };
#endif

#pragma warning disable CS0162 // Unreachable code detected
            var relUri = $"/product/product-{productId}";
#pragma warning restore CS0162 // Unreachable code detected
            ImvuProduct result;
            var failure = new ImvuProduct { Id = productId, Status = "failure" };
            try
            {
                JObject j = await _service.GetJObject(relUri);

                result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuProduct>();
                var rels = j["denormalized"].First.ElementAt(0)["relations"].ToObject<ImvuProductRelations>();
                var parentUrl = rels.Parent;
                if (parentUrl != null)
                {
                    var parent = parentUrl.Replace("https://api.imvu.com/product/product-", string.Empty);
                    if (int.TryParse(parent, out int parentId))
                    {
                        result.ParentId = parentId;
                    }
                }
                else
                {
                    result.ParentId = 0;
                }
                result.Status = "success";

            }
            catch (Exception exc)
            {
                // product is hidden in the catalog
                // so we have to deep dive for this

                if (exc.Message.Contains("403") || exc.Message.Contains("401"))
                {
                    using (var pageClient = new ImvuPageClient())
                    {
                        try
                        {
                            var hiddenProduct = await pageClient.GetHiddenProduct(productId);
                            if (hiddenProduct.CreatorName != null && hiddenProduct.CreatorId != 0 && hiddenProduct.ProductImage != null)
                            {
                                result = hiddenProduct;
                                result.Status = "hidden";
                                result.Message = "Partial data collected by web scrape";
                            }
                            else
                            {
                                failure.Message = "Crappy Product Data";
                                result = failure;
                            }

                        }
                        catch (Exception exc2)
                        {
                            failure.Message = exc2.Message;
                            result = failure;
                        }

                    }

                }
                else
                {
                    failure.Message = exc.Message;
                    result = failure;
                }

            }
            return result;
        }

        public async Task<ImvuUser> GetUserByName(string userName)
        {
            ImvuUser result = new ImvuUser { Id = -1 };
            _log?.Debug($"{nameof(ImvuApiClient)}.{nameof(GetUserByName)}({userName})");
            //var relUri = $"/users/avatarname/{userName}";

            try
            {
                using (var client = new HttpClient())
                {
                    // Our best attempt at a page scrape
                    var url = $"https://imvu-customer-sandbox.com/{userName}";
                    var pattern = @"block=(\d+)";
                    var html = await client.GetStringAsync(url);
                    Match m = Regex.Match(html, pattern);
                    if (m.Success)
                    {
                        long value = -1;
                        if (long.TryParse(m.Groups[1].Value, out value))
                        {
                            result = await GetUser(value);
                        }
                        else
                        {
                            result.Message = "Something very weird happened, number was expected for CID";
                        }
                    }
                    else
                    {
                        result.Message = "Avatar does not exist or the page is not yet set up.";
                    }
                }

                /* old logic, this was removed from IMVU API
                JObject j = await _service.GetJObject(relUri);
                var result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuUser>();
                */
                _log?.Debug($"Success - {nameof(ImvuApiClient)}.{nameof(GetUserByName)}({userName})");
                return result;
            }
            catch (Exception exc)
            {
                _log?.Error("ImvuUser JSON could not be retrieved.", exc);
                result.Message = exc.Message;
            }
            return result;
        }

        public async Task<ImvuUser> GetUser(long userId)
        {
            _log?.Debug($"{nameof(ImvuApiClient)}.{nameof(GetUser)}");
#if USE_LOCAL
            return new ImvuUser { 
                Id = userId,
                AvatarName = $"User{userId}",
                Photo = "https://triggerless.com/Content/avatar.png"
            };
#endif
            var relUri = $"/users/{userId}";
            try
            {
                JObject j = await _service.GetJObject(relUri);
                var result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuUser>();
                _log?.Debug($"Success - {nameof(ImvuApiClient)}.{nameof(GetUser)}");
                return result;
            }
            catch (Exception exc)
            {
                _log?.Error($"Unable to get User with ID ({userId})", exc);
                return new ImvuUser { Id = userId, AvatarName = "unavailable", Error = exc.Message, Status = "failed" };
            }
        }

        public async Task<ImvuProductList> GetProductsExt(IEnumerable<long> productIds)
        {
            var result = new ImvuProductList();
            var list = new ConcurrentBag<ImvuProduct>();
            var semaphore = new SemaphoreSlim(10); // Limit to 10 concurrent tasks

            var tasks = productIds.Select(async productId =>
            {
                await semaphore.WaitAsync(); // Wait for the semaphore to be available
                try
                {
                    var product = await GetProduct(productId);
                    list.Add(product); // Add the product to the ConcurrentBag
                }
                finally
                {
                    semaphore.Release(); // Release the semaphore
                }
            });

            await Task.WhenAll(tasks); // Wait for all tasks to complete

            result.Products = list.ToArray();
            return result;
        }

        public async Task<ImvuProductList> GetProducts(IEnumerable<long> p)
        {
            return await GetProductsExt(p);
            /*
            var result = new ImvuProductList();
            // the "dumb" way


            var list = new ConcurrentBag<ImvuProduct>();
            foreach (var currentProductId in p)
            {
                list.Add(await GetProduct(currentProductId));
            }
            result.Products = list.ToArray();
            return result;
            */
        }

        public async Task<string> GetAvatarCardJson(string idOrName)
        {
            _log?.Debug($"{nameof(ImvuApiClient)}.{nameof(GetAvatarCardJson)} begins");
            long id;
            if (!long.TryParse(idOrName, out id))
            {
                var user = await GetUserByName(idOrName);

                id = user.Id;
                if (id < 1)
                {
                    _log?.Warn($"Avatar with name {idOrName} not found.");
                    return $"\"status\": \"failed\", \"message\": \"(404) No user named {idOrName} was found\"";
                }
            }

            using (var pageClient = new ImvuPageClient(_log))
            {
                var json = await pageClient.GetAvatarCardJson(id);
                return json;
            }
        }

        public async Task<string> GetConversationsJson()
        {
            long cid = 25522141;
            var relUri = $"/user/user-{cid}/conversations";
            try
            {
                var result = await _service.GetJsonString(relUri);
                return result;
            }
            catch (Exception exc)
            {
                return $"{{\"status\":\"failed\", \"message\":\"{exc.Message}\"}}";
            }

        }

        public async Task<ConvoResponse> ConversationResponse()
        {
            return ConvoResponse.FromJson(await GetConversationsJson());
        }

        public void Dispose()
        {
            _service?.Dispose();
        }

        public async Task<ExposeOutfitsResponse> GetOutfits(ExposeOutfitsRequest req)
        {
            var bag = new ConcurrentBag<ExposeOutfitsResponseEntry>();

            var ss = new SemaphoreSlim(6);
            await ss.WaitAsync();

            Parallel.ForEach(req.Entries, async entry =>
            //foreach (var entry in req.Entries)
            {
                bag.Add(await GetOutfit(entry));
                ss.Release();
            });

            var list = new List<ExposeOutfitsResponseEntry>(bag.OrderBy(entry => entry.User.AvatarName.Replace("Guest_", "").ToUpper()));
            return new ExposeOutfitsResponse { Entries = list.ToArray() };
        }

        public static ExposeOutfitsRequest RequestFromUrl(string input)
        {
            const string PREFIX = "https://www.imvu.com/client.php?";
            if (!input.StartsWith(PREFIX))
            {
                throw new ArgumentException("URL input not supported");
            }

            var pieces = input.Substring(PREFIX.Length).Split('&');
            var list = new List<ExposeOutfitsRequestEntry>();
            foreach (var piece in pieces)
            {
                if (!piece.StartsWith("avatar")) { continue; }
                var nameValue = piece.Split('=');
                if (nameValue.Length == 2)
                {
                    var entry = new ExposeOutfitsRequestEntry { };
                    entry.AvatarId = long.Parse(nameValue[0].Replace("avatar", ""));
                    entry.ProductIds = nameValue[1].Split(new[] { "%3B" }, StringSplitOptions.None).Select(pid => long.Parse(pid)).ToArray();
                    list.Add(entry);
                }
            }
            return new ExposeOutfitsRequest { Entries = list.ToArray() };
        }

        public async Task<ExposeOutfitsResponseEntry> GetOutfit(ExposeOutfitsRequestEntry entry)
        {
            return new ExposeOutfitsResponseEntry {
                User = await GetUser(entry.AvatarId),
                Products = (await GetProductsExt(entry.ProductIds)).Products
            };
        }

        public async Task<ProductSoundTriggerPayload> GetProductSoundTriggerPayload(long pid)
        {
            var result = new ProductSoundTriggerPayload();
            var triggerList = new List<ProductSoundTrigger>();

            var product = await GetProduct(pid);
            result.CreatorName = product.CreatorName;
            result.ProductId = product.Id;
            result.ProductName = product.Name;
            result.ImageLocation = product.ProductImage;

            // Start with the product of interest

            var currentProductId = pid;
            var stopPids = new long[] { 
                80,          // IMVU Female Avatar
                191,         // IMVU Male Avatar
                0, -1,
                56816049,    // Empty Female Accessory
                2191901,     // Empty Female Clothing
                56919490,    // Empty Male Accessory
                9911131,     // Empty Male Clothing 
                669,         // Glasses King Gold
                682,         // Glasses Spice Lagoon 
                11638,       // Glasses 48s in X22
            };

            // Now we run up the derivation tree up to Female or Male Avatar base products.
            // I've found that there are products that contain triggers at many levels of
            // the derivation tree and this will catch all of them.

            Debug.WriteLine($"Product Id: {currentProductId}");
            while (!stopPids.Contains(currentProductId)) // base Female and Male Avatar product ids
            {
                try
                {
                    // Get triggers for current product
                    var currentTriggerList = await GetProductSoundTriggerList(currentProductId);

                    // Grab available triggers at this level
                    // Some products have OGG files with no associated trigger, grrrr....
                    triggerList.AddRange(currentTriggerList.Where(t => !String.IsNullOrWhiteSpace(t.Trigger) && !String.IsNullOrWhiteSpace(t.Location)));

                    // Branch up the derivation tree one level
                    currentProductId = currentTriggerList.ParentProductId;
                    Debug.WriteLine($"Product Id: {currentProductId}");

                }
                catch (Exception)
                {
                    // If you try to GetProduct for one of the earliest IMVU products (such as Female Avatar)
                    // the API will throw an exception. Just bail and accept there were no more triggers to be found.

                    break;
                }
            }

            triggerList.Sort(new SoundTriggerComparer());
            result.Triggers = triggerList.ToArray();
            return result;
        }

        private async Task<ProductSoundTriggerList> GetProductSoundTriggerList(long pid)
        {
            var dtStart = DateTime.Now;

            // Initialize variables
            var result = new ProductSoundTriggerList();
            var contentsUrl = RipService.GetUrl(pid, "_contents.json");
            ProductContentList productList;
            Template template = null;

            // Populate productList and template
            using (var client = new HttpClient())
            {

                // Get the Product list
                _log?.Debug($"\tAcquiring contents");
                var responseJson = await client.GetStringAsync(contentsUrl);

                // cast into JSON we can deserialize
                var json = $"{{productArray: {responseJson}}}";
                _log?.Debug($"\tDeserializing product list");
                productList = JsonConvert.DeserializeObject<ProductContentList>(json);

                // remove any non-OGG assets
                _log?.Debug($"\tRemoving non-OGG assets");
                productList.productArray = productList.productArray.Where(p => p.name.ToLower().EndsWith(".ogg")).ToArray();

                // in cases where url is omitted, we would use name instead. Here we'll just populate null urls for ease of use
                foreach (var item in productList.productArray.Where(i => i.url == null))
                {
                    item.url = item.name;
                }

                template = await GetTemplate(pid);
            }

            if (template == null)
            {
                return new ProductSoundTriggerList();
            }

            result.ParentProductId = template.ParentProductID;
            // Start populating the entries in our result
            foreach (var action in template?.Actions)
            {
                // Here, name is the name of the OGG file
                var name = action.Sound?.Name;
                if (string.IsNullOrWhiteSpace(name)) continue;

                // Create our initial entry and add it to the result entries
                var entry = new ProductSoundTrigger();
                entry.Trigger = action.Name;
                if (productList.productArray != null && productList.productArray.Length > 0)
                {
                    var location = productList.productArray.Where(e => e.name == name).Select(e => e.url).First();
                    if (String.IsNullOrWhiteSpace(location)) continue;
                    entry.Location = RipService.GetUrl(pid, location).Replace(" ", "+");
                    result.Add(entry);
                }
            }
            _log?.Debug($"\t{template.Actions.Count} triggers cued up.");

            return result;
        }

        public async Task<Template> GetTemplate(long productId)
        {
            Template result = null;
            // Get index.xml template, and deserialize using XAFLib Template class
            _log?.Debug($"\tAcquiring index.xml");
            var indexUrl = RipService.GetUrl(productId, "index.xml");

            using (var client = new HttpClient())
            {
                var responseText = await client.GetStringAsync(indexUrl);
                _log?.Debug($"\tLoading Template");
                try
                {
                    result = Template.LoadXml(responseText);
                }
                catch (Exception exc)
                {
                    // This happens when you get a template that has no content and 
                    // doesn't connect to anything
                    // Just send back an empty template with ParentProductId = 80

                    _log?.Error("Unable to load template", exc);
                    return new Template { ParentProductID = 80 };
                }
            }
            return result;
        }

        public class ProductSound
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        private class ContentsEntry
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("location")]
            public string Location { get; set; }
        }

        public async Task<ProductSound[]> GetProductSounds(long productId)
        {
            var result = new List<ProductSound>();
            using (var client = new HttpClient())
            {
                var contentUrl = RipService.GetUrl(productId, "_contents.json");
                var indexUrl = RipService.GetUrl(productId, "index.xml");

                var contentsJson = await client.GetStringAsync(contentUrl);
                var indexXml = await client.GetStringAsync(indexUrl);

                var contents = JsonConvert.DeserializeObject<List<ContentsEntry>>(contentsJson);
                foreach (var entry in contents)
                {
                    if (string.IsNullOrWhiteSpace(entry.Location)) entry.Location = entry.Name;
                }

                var t = Template.LoadXml(indexXml);

                var actions = t.Actions.Where(a => !string.IsNullOrWhiteSpace(a.Sound?.Name))
                    .OrderBy(a => a.Name.ToLowerInvariant()).ToList();

                foreach (var action in actions)
                {
                    var contentsEntry = contents
                        .FirstOrDefault(e => e.Name.ToLowerInvariant() ==
                            action.Sound.Name.ToLowerInvariant());
                    if (contentsEntry == null || string.IsNullOrWhiteSpace(contentsEntry.Location)) continue;
                    result.Add(new ProductSound
                    {
                        Name = action.Name,
                        Url = RipService.GetUrl(productId, contentsEntry.Location),
                    });
                }
            }



            return result.ToArray();
        }
    }
}

