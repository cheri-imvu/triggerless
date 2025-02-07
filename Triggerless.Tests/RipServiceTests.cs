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
    public class RipServiceTests
    {
        [Test]
        public void Test_GetLength()
        {

            // 375 ms
            var pid = 45010057;
            var location = "7eec5d2a729f4539fdfd78542f61ef7b.ogg";
            var length = new NVorbisService(null).GetLength(pid, location);
            Assert.That(length != 0);
            Console.WriteLine($"Song is {length} ms long");
        }

        private string[] TestLocations => new string[] {
            "7eec5d2a729f4539fdfd78542f61ef7b.ogg",
            "195224514d48b74908e152e689b1d4c7.ogg",
            "7f0b6d2cdd488b29bf28a07262b2a5d2.ogg",
            "abcd9296f345faeedfbdb73528f62bc9.ogg",
            "6bffa1f1bd0c50ffda482545c998e8be.ogg",
            "1cf83a400924e5aea38d5d999099250c.ogg",
            "c3fde99dc736e121ab1b12756842dec7.ogg",
            "d47e3cacfd7da7cb1a8bed6b773fc1ac.ogg",
            "33214dc18d632b78429df96fc8facecc.ogg",
            "d361762b5c022a2571dc8622cb333b3c.ogg",
            "de5da99dff2f4104059a4f6429fa2dcb.ogg",
            "a88e9f1b4fe60a322197d9e08f96a7fe.ogg",
            "9d0d54508e9af775b3a17cfe7a409f94.ogg",
            "a8893d2ca3c83e95ca1e21993aacdb73.ogg",
            "1e3fef4e25e31e34915f852b5c36bb85.ogg",
            "18db851ac5ffe18577e60325e66206ab.ogg"
        };

        [Test]
        public void Test_GetLengths()
        {
            // 800 ms
            var pid = 45010057;
            var locations = TestLocations;

            var request = new NVorbisService.GetLengthsRequest { PID = pid, Locations = locations };
            var result = new NVorbisService().GetLengths(request);

            Assert.That(result.Results.Count() > 0);
            foreach (var entry in result.Results)
            {
                Console.WriteLine($"{entry.Item1}\t{entry.Item2}");
            }
        }

    }
}
