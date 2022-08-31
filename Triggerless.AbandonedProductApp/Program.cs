using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;
using Triggerless.Services.Server;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using Newtonsoft.Json;

namespace Triggerless.AbandonedProductApp
{
    class Program
    {
        static ImvuApiClient _client;
        static SqlConnection _conn;

        static async Task Main(string[] args)
        {
            var connStr = ConfigurationManager.ConnectionStrings["local"].ConnectionString;
            using (_conn = new SqlConnection(connStr))
            using (_client = new ImvuApiClient())
            {
                _conn.Open();
                var products = await GetProducts(100);
                ProcessProducts(products.Products);
            }
            //Console.WriteLine("Type any char to end");
            //Console.ReadKey();
        }

        static async Task<ImvuProductList> GetProducts(int howMany)
        {
            var random = new Random();
            var ids = Enumerable.Range(0, howMany).Select(n => (long)random.Next(80, 58525000));
            return await _client.GetProducts(ids);
        }

        static void ProcessProducts(IEnumerable<ImvuProduct> products)
        {
            var sql = @"
                IF NOT EXISTS (SELECT ProductId FROM AbandonedProduct WHERE ProductId = @ProductId)
                INSERT INTO AbandonedProduct (ProductId, ProductName, CreatorId, CreatorName,
                AllowsDerivation, ParentId, ProductPrice, Profit, Json)
                VALUES (@ProductId, @ProductName, @CreatorId, @CreatorName,
                @AllowsDerivation, @ParentId, @ProductPrice, @Profit, @Json);
                SELECT SCOPE_IDENTITY();
            ";

            foreach (var product in products)
            {
                if (product.CreatorName == null) continue;
                if (!product.CreatorName.Contains("_")) continue;

                Console.WriteLine($"Saving {product.Id} by {product.CreatorName}");

                var record = new SqlRecord
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CreatorId = product.CreatorId,
                    CreatorName = product.CreatorName,
                    AllowsDerivation = product.AllowsDerivation != 0,
                    ParentId = product.ParentId,
                    ProductPrice = product.Price,
                    Profit = product.Profit,
                    Json = JsonConvert.SerializeObject(product, Formatting.Indented)
                };
                var returnId = _conn.Query<int>(sql, record).SingleOrDefault();
                product.Id = returnId;





            }
        }
    }

    class SqlRecord
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public long CreatorId { get; set; }
        public string CreatorName { get; set; }
        public long? ParentId { get; set; }
        public bool AllowsDerivation { get; set; }
        public int ProductPrice { get; set; }
        public int Profit { get; set; }
        public string Json { get; set; }

    }
}
