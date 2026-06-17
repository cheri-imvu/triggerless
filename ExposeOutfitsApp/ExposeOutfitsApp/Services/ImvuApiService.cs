using ExposeOutfitsApp.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExposeOutfitsApp.Services
{

    public class ImvuApiService
    {
        private readonly HttpClient _client;
        private readonly SemaphoreSlim _throttle = new(8);

        public ImvuApiService()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };

            handler.CookieContainer.Add(
                new Uri("https://api.imvu.com"),
                new Cookie("osCsid", "dbb2c2bbded7a041304198e0d0144d1e")
            );

            _client = new HttpClient(handler);
        }

        public async Task PopulateUserAsync(User user)
        {
            var url = $"https://api.imvu.com/user/user-{user.Id}";
            var json = await _client.GetFromJsonAsync<JsonElement>(url);

            var node = json
                .GetProperty("denormalized")
                .EnumerateObject()
                .First()
                .Value
                .GetProperty("data");

            user.Name = node.GetProperty("display_name").GetString();
            user.ImageUrl = node.GetProperty("thumbnail_url").GetString();
        }

        public async Task PopulateProductAsync(Product product)
        {
            await _throttle.WaitAsync();

            try
            {
                var url = $"https://api.imvu.com/product/product-{product.Id}";
                var json = await _client.GetFromJsonAsync<JsonElement>(url);

                var node = json
                    .GetProperty("denormalized")
                    .EnumerateObject()
                    .First()
                    .Value
                    .GetProperty("data");

                product.Name = node.GetProperty("product_name").GetString();
                product.ImageUrl = node.GetProperty("product_image").GetString();
                product.CreatorName = node.GetProperty("creator_name").GetString();
                product.CreatorId = node.GetProperty("creator_cid").GetInt64();
            }
            catch
            {
            }
            finally
            {
                _throttle.Release();
            }
        }
    }
}
