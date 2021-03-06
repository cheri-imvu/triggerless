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

        public ImvuApiClient()
        {
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
                ProductImage = "http://localhost:61120/Content/icon.png"
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

                if (exc.Message.Contains("403"))
                {
                    using (var pageClient = new ImvuPageClient())
                    {
                        try
                        {
                            var hiddenProduct = await pageClient.GetHiddenProduct(productId);
                            if (hiddenProduct.CreatorName != null && hiddenProduct.CreatorId != 0 && hiddenProduct.ProductImage != null)
                            {
                                result = hiddenProduct;
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
            var relUri = $"/users/avatarname/{userName}";
            JObject j = await _service.GetJObject(relUri);
            var result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuUser>();
            return result;
        }

        public async Task<ImvuUser> GetUser(long userId)
        {
#if USE_LOCAL
            return new ImvuUser { 
                Id = userId,
                AvatarName = $"User{userId}",
                Photo = "http;//localhost:61120/Content/avatar.png"
            };
#endif
#pragma warning disable CS0162 // Unreachable code detected
            var relUri = $"/users/{userId}";
#pragma warning restore CS0162 // Unreachable code detected
            JObject j = await _service.GetJObject(relUri);
            var result = j["denormalized"].First.ElementAt(0)["data"].ToObject<ImvuUser>();
            return result;
        }

        public async Task<ImvuProductList> GetProducts(long[] p)
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

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}

