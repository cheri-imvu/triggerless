using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services.Server;
using Triggerless.Models;
using log4net;

namespace Triggerless.Tests
{
    [TestFixture]
    public class RadioTests
    {
        public static readonly ILog _log = LogManager.GetLogger(nameof(RadioTests));
        private static BootstersDbClient _dbClient = new BootstersDbClient(_log);

        [Test]
        public async Task GetSongsTest()
        {
            var songs = await _dbClient.GetSongs("TestDJ", 24);
            Assert.That(songs.titles.Length > 0);
            songs.titles.ToList().ForEach(title => Console.WriteLine(title));
        }

        [Test]
        public async Task PostSongTest()
        {
            var post = new TriggerlessRadioSong { 
                djName = "TestDJ",
                title = "Some Artist - Some Title" 
            };
            var response = await _dbClient.PostSong(post);
            Assert.That(response.status.StartsWith("success"));
            Console.WriteLine(response.status);
            response = await _dbClient.PostSong(post);
            Assert.That(response.status.StartsWith("success"));
            Console.WriteLine(response.status);
        }
    }
}
