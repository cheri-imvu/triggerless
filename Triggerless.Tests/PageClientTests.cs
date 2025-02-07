using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services.Server;

namespace Triggerless.Tests
{
    [TestFixture]
    public class PageClientTests
    {
        private const long USER_ID = 70587910; //oP0PPYo
        private const long PRoDUCT_ID = 41967; // Ankh Collar hidden
        private ImvuPageClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new ImvuPageClient();
        }

        [TearDown]
        public void Teardown()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task GetHiddenProduct()
        {
            var product = await _client.GetHiddenProduct(PRoDUCT_ID);
            Assert.That(product != null);
            Assert.That(product.Name != null);
            Assert.That(product.CreatorName != null);
            Assert.That(product.ProductImage != null);
            Assert.That(!product.IsVisible);
            Assert.That(product.CreatorId  != 0, "CreatorId came back 0");  // still have to fulfill
        }
    }
}
