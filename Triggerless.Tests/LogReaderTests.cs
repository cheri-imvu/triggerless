using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services;
using Triggerless.Services.Client;

namespace Triggerless.Tests
{
    [TestFixture]
    public class LogReaderTests
    {
        [Test]
        public void ReadConvo()
        {
            var reader = new FileReader();
            var convo = reader.ReadFile(@"D:\DEV\CS\Triggerless\Triggerless.Tests\imvu-logs\IMVULog.log.2");
            Assert.That(convo != null);
            Assert.That(convo.Events.Any(e => e.Text != null));

        }

        [Test]
        public void FolderTest1()
        {
            var reader = new FolderReader(@"D:\DEV\CS\Triggerless\Triggerless.Tests\imvu-logs");
            var convo = reader.Read();
            Assert.That(convo.Events.Count > 700, "Fewer than 700 events were generated");
            Console.WriteLine(convo.Events[0].Time);
            Assert.That(!convo.Events.Any(e => e.Author.AvatarName == null), "At least one null Author AvatarName was detected");
        }

        [Test]
        public void RecentMessages()
        {
            var reader = new FolderReader();
            var convo = reader.Read();
            convo.Dump();
        }
    }
}
