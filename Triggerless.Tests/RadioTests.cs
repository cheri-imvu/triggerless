using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services.Server;
using Triggerless.Models;

namespace Triggerless.Tests
{
    [TestFixture]
    public class RadioTests
    {
        [Test]
        public async Task GetSongsTest()
        {
            var songs = await BootstersDbService.GetSongs(24);
            Assert.That(songs.titles.Length > 0);
            songs.titles.ToList().ForEach(title => Console.WriteLine(title));
        }

        [Test]
        public async Task PostSongTest()
        {
            var post = new TriggerlessRadioSong { title = "Roxy Music - Love is the Drug" };
            var response = await BootstersDbService.PostSong(post);
            Assert.That(response.status.StartsWith("success"));
            Console.WriteLine(response.status);
            response = await BootstersDbService.PostSong(post);
            Assert.That(response.status.StartsWith("success"));
            Console.WriteLine(response.status);
        }
    }
}
