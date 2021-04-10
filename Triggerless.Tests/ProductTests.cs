using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Triggerless.API.Controllers;
using Triggerless.Models;

namespace Triggerless.Tests
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public async Task GetAProductAsync() {
            var controller = new ProductController();
            ImvuProduct p = await controller.GetProduct(10599276);
            Console.WriteLine(p.Name);
            Console.WriteLine(p.CreatorName);
            Console.WriteLine($"{p.Price} cr");
        }

        [Test]
        public async Task GetProducts()
        {
            var c = new ProductController();
            var list = new long[] { 10599276, 10599277, 10599278 };
            ImvuProductList result = await c.GetProducts(list);
            Assert.AreEqual(list.Length, result.Products.Length);
            Assert.IsTrue(result.Products.Any(p => p.CreatorName.Contains("_")));
            int numHits = result.Products.Count(p => p.Status == "success");
            Console.WriteLine(numHits);
        }
    }
}
