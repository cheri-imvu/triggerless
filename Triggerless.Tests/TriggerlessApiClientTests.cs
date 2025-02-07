using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;
using Triggerless.Services.Client;

namespace Triggerless.Tests
{
    [TestFixture]
    public class TriggerlessApiClientTests
    {
        [Test]
        public async Task TriggerlessUserGet()
        {
            var userId = 82044182L;
            var client = new TriggerlessApiClient();
            var user = await client.GetUser(userId);
            Assert.That(user != null);
            Assert.That (user.AvatarName != null);

        }
    }
}
