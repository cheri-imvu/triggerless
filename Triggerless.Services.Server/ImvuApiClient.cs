using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;



namespace Triggerless.Services.Server
{
    public class ImvuApiClient: IDisposable
    {
        private ImvuApiService _service;
        private ILog _log;

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
                } else
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
                            } else
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

                } else
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
            } catch (Exception exc)
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
                return new ImvuUser { Id = userId, AvatarName = "unavailable", Error = exc.Message, Status = "failed"};
            }
        }

        public async Task<ImvuProductList> GetProducts(IEnumerable<long> p)
        {
            var result = new ImvuProductList();
            // the "dumb" way
            

            var list = new ConcurrentBag<ImvuProduct>();
            foreach (var productId in p)
            {
                list.Add(await GetProduct(productId));
            }
            result.Products = list.ToArray();
            return result;
            






            // the "smart" way
            /*
            var cache = new Dictionary<long, ImvuProduct>();
            p.ToList().ForEach(id => cache[id] = null);

            var relUri = $"/product?id=";
            var delim = "";
            foreach (var productId in p)
            {
                relUri += delim + $"https%3A%2F%2Fapi.imvu.com%2Fproduct%2Fproduct-{productId}";
                delim = "%2C";
            }

            relUri = "/product?id=https%3A%2F%2Fapi.imvu.com%2Fproduct%2Fproduct-10599276%2Chttps%3A%2F%2Fapi.imvu.com%2Fproduct%2Fproduct-10599277%2Chttps%3A%2F%2Fapi.imvu.com%2Fproduct%2Fproduct-10599278";

            JObject j = await _service.GetJObject(relUri);
            var denorm = j["denormalized"];
            var returnedProducts = new ImvuProduct[denorm.Count() - 1];

            foreach (var child in denorm.Children())
            {
                var product = child.First["data"].ToObject<ImvuProduct>();
                product.Status = "success";
                if (product.Id == 0) continue;

                var rels = child.First["relations"].ToObject<ImvuProductRelations>();
                var parentUrl = rels.Parent;
                var parent = parentUrl.Replace("https://api.imvu.com/product/product-", string.Empty);
                if (int.TryParse(parent, out int parentId))
                {
                    product.ParentId = parentId;
                }
                cache[product.Id] = product;
            }

            foreach (var id in cache.Where(x => x.Value == null).ToList())
            {
                var product = new ImvuProduct
                {
                    Status = "failure",
                    Id = id.Key
                };
                cache[product.Id] = product;

            }

            result.Products = cache.Values.ToArray();

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
    }
}

