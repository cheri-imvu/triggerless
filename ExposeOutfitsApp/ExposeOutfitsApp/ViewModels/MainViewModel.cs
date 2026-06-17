using ExposeOutfitsApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExposeOutfitsApp.ViewModels
{
    public class MainViewModel
    {
        private readonly HttpClient _client;
        private readonly SemaphoreSlim _throttle = new(8);
        private int _completedProducts = 0;

        public State State { get; set; } = new();

        public MainViewModel()
        {
            _client = new HttpClient();
        }

        public async Task LoadAsync(List<long> roomIds, Dictionary<long, List<long>> userProducts)
        {
            _completedProducts = 0;
            State.ProgressValue = 0;

            int totalProducts =
                roomIds.Count +
                userProducts.Values.Sum(list => list.Count);

            State.ProgressMaximum = totalProducts;

            // Load Room Products
            State.Room = new Room(roomIds.Select(id => new Product { Id = id }));

            foreach (var product in State.Room)
            {
                _ = PopulateProductAsync(product);
            }

            // Load Users
            State.Users.Clear();

            foreach (var kvp in userProducts)
            {
                var user = new User
                {
                    Id = kvp.Key,
                    Products = new ObservableCollection<Product>(
                        kvp.Value.Select(id => new Product { Id = id }))
                };

                State.Users.Add(user);

                _ = PopulateUserAsync(user);
            }
        }

        private async Task PopulateUserAsync(User user)
        {
            try
            {
                var url = $"https://api.imvu.com/user/user-{user.Id}";
                var json = await GetJsonAsync(url);

                var denormalized = json.GetProperty("denormalized")
                                       .EnumerateObject()
                                       .First()
                                       .Value
                                       .GetProperty("data");

                user.Name = denormalized.GetProperty("username").GetString();
                user.ImageUrl = denormalized.GetProperty("thumbnail_url").GetString();

                var tasks = user.Products.Select(p => PopulateProductAsync(p));
                await Task.WhenAll(tasks);
            }
            catch
            {
                // Ignore user load failure
            }
        }

        private async Task PopulateProductAsync(Product product)
        {
            await _throttle.WaitAsync();

            try
            {
                var url = $"https://api.imvu.com/product/product-{product.Id}";
                var json = await GetJsonAsync(url);

                var node = json.GetProperty("denormalized")
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
                product.Name = "Hidden Product";
                product.ImageUrl = "pack://application:,,,/Images/dummy.png";
                product.CreatorName = "";
                product.CreatorId = 0;
            }
            finally
            {
                int newValue = Interlocked.Increment(ref _completedProducts);
                State.ProgressValue = newValue;

                _throttle.Release();
            }
        }

        private async Task<JsonElement> GetJsonAsync(string url)
        {
            using var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            var doc = await JsonDocument.ParseAsync(stream);
            return doc.RootElement;
        }
    }
}
