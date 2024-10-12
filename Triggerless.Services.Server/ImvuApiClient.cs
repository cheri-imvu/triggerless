using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triggerless.Models;



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
            _log?.Debug($"{nameof(ImvuApiClient)}.{nameof(GetUserByName)}({userName})");
            var relUri = $"/users/avatarname/{userName}";
            try
            {
                JObject j = await _service.GetJObject(relUri);
                var result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuUser>();
                _log?.Debug($"Success - {nameof(ImvuApiClient)}.{nameof(GetUserByName)}({userName})");
                return result;
            }
            catch (Exception exc)
            {
                _log?.Error("ImvuUser JSON could not be retrieved.", exc);
                return new ImvuUser { Id = -1, Message = exc.Message };
            }
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
            foreach (var productId in p)
            {
                list.Add(await GetProduct(productId));
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
    }
}

