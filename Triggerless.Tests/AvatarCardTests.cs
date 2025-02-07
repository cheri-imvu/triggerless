using System;
using System.Reflection;
using Newtonsoft.Json;
using NUnit.Framework;
using Triggerless.API.Controllers;
using Triggerless.Models;
using AvCard = Triggerless.Models.AvatarCard;

namespace Triggerless.Tests
{
    [TestFixture]
    public class AvatarCardTests
    {
        [Test]
        public void SimpleTest()
        {
            AvatarCardController ctrl = new AvatarCardController();
            //var msg = ctrl.InnerGet("Triggers");
        }

        [Test]
        public void TestDeserialize() {
            var responseJson = "[{\"original_dimensions\":\"244,350\",\"name\":\"01.jpg\",\"tags\":[\"original\",\"thumbnail\"]},{\"original_dimensions\":\"350,350\",\"name\":\"02.jpg\",\"tags\":[\"original\",\"thumbnail\"]},{\"name\":\"1.xrf\"},{\"name\":\"2.xrf\"},{\"name\":\"index.xml\"},{\"name\":\"l.xmf\"},{\"original_dimensions\":\"244,350\",\"name\":\"opac1.png\",\"tags\":[\"original\",\"thumbnail\"]},{\"name\":\"r.xmf\"}]";
            var json = $"{{productArray: {responseJson}}}";


            var list = JsonConvert.DeserializeObject<ProductContentList>(json);
            Console.WriteLine(list.productArray.Length);

        }

        [Test]
        public void AvatarCard_TryParse()
        {
            var assy = Assembly.GetExecutingAssembly();
            var name = assy.ManifestModule.Name.Replace(".dll", "");
            var resName = $"{name}.avcard-sample.json";
            string json;
            using (var stream = assy.GetManifestResourceStream(resName))
            {
                using (var sr = new System.IO.StreamReader(stream))
                {
                    json = sr.ReadToEnd();
                }
            }

            AvCard result;
            Assert.That(AvCard.TryParse(json, out result));
            Assert.Equals("Cheri", result.Name);
            Assert.Equals(4, result.PublicRooms.Count);
            Assert.That(result.BadgeLayout.Count > 50);
        }
    }
}
