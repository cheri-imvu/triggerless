using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Triggerless.Services.Server;
using static Triggerless.Services.Server.BootstersDbClient;

namespace Triggerless.Tests
{
    [TestFixture]
    internal class BootstersDbTests
    {
        [Test]
        public async Task TestEvent()
        {
            var client = new BootstersDbClient();
            var jsonText = "{ \"Purpose\": \"Test\" }";

            EventResult result = await client.SaveEventAsync(
                EventType.CutTune, 83079851, jsonText);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type == EventResultType.Success, Is.True, "It didn't work");

            // make sure null works too.
            result = await client.SaveEventAsync(
                EventType.CutTune, 83079851, null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type == EventResultType.Success, Is.True, "It didn't work");

            // test something with apostrophes
            jsonText = "{\"Purpose\": \"Let's party y'all!\"}";
            result = await client.SaveEventAsync(
                EventType.CutTune, 83079851, jsonText);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type == EventResultType.Success, Is.True, "It didn't work");

            // test something with emojis
            jsonText = "{\"Purpose\": \"Let's party y'all! 🎵💀\"}";
            result = await client.SaveEventAsync(
                EventType.CutTune, 83079851, jsonText);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Type == EventResultType.Success, Is.True, "It didn't work");

        }
    }
}
