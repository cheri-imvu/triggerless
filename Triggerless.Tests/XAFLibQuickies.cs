using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.XAFLib;
using System;
using System.IO;

namespace Triggerless.Tests
{
    [TestFixture]
    public class XAFLibQuickies
    {
        [Test]
        public void TestSound()
        {
            var a = new XAFLib.Action();
            a.Sound.Name = "mysound.ogg";
            a.Name = "mysound";

            var t = new Template();
            t.Actions.Add(a);

            t.Actions.Add(new XAFLib.Action { Sound = new Sound { Name = "thissound.ogg" }, Name = "thissound" });

            string xml = t.GetIndexXml();
            File.WriteAllText(@"D:\Temp\index00.xml", xml);
            Console.Write(xml);

        }

        [Test]
        public void CreateChkn()
        {
            var path = @"D:\DEV\Accessorie\Products\Sound\SoundsHarley\Thinned\coll-2";
            var temp = new Template();

            var filenames = Directory.GetFiles(path, "*.ogg").Select(fn => {
                return Path.GetFileNameWithoutExtension(fn);
            });

            temp.Actions.AddRange(filenames.Select(fn => new XAFLib.Action { Name = fn, Sound = new Sound { Name = fn + ".ogg" } }));
            ChknFile.CreateChkn(path, temp);
        }

    }
}
