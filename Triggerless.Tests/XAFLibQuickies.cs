using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.XAFLib;
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;

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

        [Test]
        public void ConvertCAF2XAF()
        {
            var path = @"D:\DEV\Accessorie\Rips\lush-60";
            var xaflistfile = @"D:\temp\xaflist.txt";
            var lines = File.ReadAllLines(xaflistfile);
            foreach(string line in lines)
            {
                var assetName = line.Split('\t')[1].ToLower();

                var filename = Path.Combine(path, assetName);
                XAFFile xaf = new XAFFile();
                var anim = xaf.LoadBinary(filename.Replace(".xaf", ".caf"));
                Console.Write(filename);
                xaf.Save(filename, anim);
                
            }


        }

        private class TriggerEntry
        {
            public string Name { get; set; }
            public string AssetName { get; set; }
            public override string ToString() => $"{Name}\t{AssetName}";
        }

        [Test]
        public void TriggerNamesAndFiles()
        {
            var path = @"D:\DEV\Accessorie\Rips\lush-60";
            var filename = Path.Combine(path, "index.xml");

            var doc = new XmlDocument();
            doc.Load(filename);
            Console.WriteLine("Loaded");
            var list = new List<TriggerEntry>();

            var root = doc.DocumentElement;
            foreach (var sibling in root.ChildNodes)
            {
                var sib = sibling as XmlElement;
                if (!sib.Name.StartsWith("Action")) continue;

                var elName = sib.SelectSingleNode("Name");
                if (elName == null) continue;

                var name = elName.InnerText;
                var entry = new TriggerEntry { Name = name };

                var elDef = sib.SelectSingleNode("Definition");
                if (elDef == null) continue;

                var elActionDef = elDef.SelectSingleNode("ActionDefinition");
                if (elActionDef == null) continue;

                var elEnsemble = elActionDef.SelectSingleNode("EnsembleDefinition0");
                if (elEnsemble == null) continue;

                var elSkel = elEnsemble.SelectSingleNode("SkeletalAnimationEffect0");
                if (elSkel == null) continue;

                var elAssetName = elSkel.SelectSingleNode("AssetName");
                if (elAssetName == null) continue;

                var assetName = elAssetName.InnerText;
                entry.AssetName = assetName;
                list.Add(entry);

            }

            foreach (var entry in list.OrderBy(e => e.Name))
            {
                Console.WriteLine(entry.ToString());
            }
            
        }

        private class HeadBobTrigger
        {
            public string Trigger { get; set; }
            public Quaternion Quat21_1 { get; set; }
            public Quaternion Quat21_2 { get; set; }
            public Quaternion Quat22_1 { get; set; }
            public Quaternion Quat22_2 { get; set; }

            public HeadBobTrigger(string trigger, Quaternion quat211, Quaternion quat221, Quaternion quat212, Quaternion quat222)
            {
                Trigger = trigger;
                Quat21_1 = quat211;
                Quat21_2 = quat212;
                Quat22_1 = quat221;
                Quat22_2 = quat222;
            }
        }

        [Test]
        public void CreateHeadBobAnimations()
        {
            #region Quaternions
            var hu5_b21 = new Quaternion("0 -1.70937E-05 0.179954 0.983675");
            var hu4_b21 = new Quaternion("0 -1.51529E-05 0.102674 0.994715");
            var hu3_b21 = new Quaternion("0 -1.39588E-05 0.0592574 0.998243");
            var hu2_b21 = new Quaternion("0 -1.3353E-05 0.0331086 0.999452");
            var hu1_b21 = new Quaternion("0 -1.24266E-05 0.00038009 1");
            var hd1_b21 = new Quaternion("0 -1.01635E-05 -0.0760392 0.997105");
            var hd2_b21 = new Quaternion("0 0 -0.10419 0.994557");
            var hd3_b21 = new Quaternion("0 0 -0.128023 0.991771");
            var hd4_b21 = new Quaternion("0 0 -0.166887 0.985976");
            var hd5_b21 = new Quaternion("0 0 -0.196608 0.980482");

            var hu5_b22 = new Quaternion("0 0 0.217993 0.97595");
            var hu4_b22 = new Quaternion("0 0 0.1412 0.989981");
            var hu3_b22 = new Quaternion("0 0 0.0979529 0.995191");
            var hu2_b22 = new Quaternion("0 0 0.0718708 0.997414");
            var hu1_b22 = new Quaternion("0 0 0.0391882 0.999232");
            var hd1_b22 = new Quaternion("0 0 -0.0372858 0.999305");
            var hd2_b22 = new Quaternion("0 0 -0.0655141 0.997852");
            var hd3_b22 = new Quaternion("0 0 -0.0894374 0.995992");
            var hd4_b22 = new Quaternion("0 0 -0.128497 0.99171");
            var hd5_b22 = new Quaternion("0 0 -0.158409 0.987374");
            #endregion

            var triggers = new List<HeadBobTrigger>() {
                new HeadBobTrigger("bh01", hd5_b21, hd5_b22, hd4_b21, hd4_b22),
                new HeadBobTrigger("bh02", hd4_b21, hd4_b22, hd3_b21, hd3_b22),
                new HeadBobTrigger("bh03", hd3_b21, hd3_b22, hd2_b21, hd2_b22),
                new HeadBobTrigger("bh04", hd2_b21, hd2_b22, hd1_b21, hd1_b22),
                new HeadBobTrigger("bh05", hd1_b21, hd1_b22, hu1_b21, hu1_b22),
                new HeadBobTrigger("bh06", hu1_b21, hu1_b22, hu2_b21, hu2_b22),
                new HeadBobTrigger("bh07", hu2_b21, hu2_b22, hu3_b21, hu3_b22),
                new HeadBobTrigger("bh08", hu3_b21, hu3_b22, hu4_b21, hu4_b22),
                new HeadBobTrigger("bh09", hu4_b21, hu4_b22, hu5_b21, hu5_b22),

                new HeadBobTrigger("bh10", hd5_b21, hd5_b22, hd3_b21, hd3_b22),
                new HeadBobTrigger("bh11", hd4_b21, hd4_b22, hd2_b21, hd2_b22),
                new HeadBobTrigger("bh12", hd3_b21, hd3_b22, hd1_b21, hd1_b22),
                new HeadBobTrigger("bh13", hd2_b21, hd2_b22, hu1_b21, hu1_b22),
                new HeadBobTrigger("bh14", hd1_b21, hd1_b22, hu2_b21, hu2_b22),
                new HeadBobTrigger("bh15", hu1_b21, hu1_b22, hu3_b21, hu3_b22),
                new HeadBobTrigger("bh16", hu2_b21, hu2_b21, hu4_b21, hu4_b22),
                new HeadBobTrigger("bh17", hu3_b21, hu3_b21, hu5_b21, hu5_b22),

                new HeadBobTrigger("bh18", hd5_b21, hd5_b22, hd2_b21, hd2_b22),
                new HeadBobTrigger("bh19", hd4_b21, hd4_b22, hd1_b21, hd1_b22),
                new HeadBobTrigger("bh20", hd3_b21, hd3_b22, hu1_b21, hu1_b22),
                new HeadBobTrigger("bh21", hd2_b21, hd2_b22, hu2_b21, hu2_b22),
                new HeadBobTrigger("bh22", hd1_b21, hd1_b22, hu3_b21, hu3_b22),
                new HeadBobTrigger("bh23", hu1_b21, hu1_b22, hu4_b21, hu4_b22),
                new HeadBobTrigger("bh24", hu2_b21, hu2_b22, hu5_b21, hu5_b22),
            };

            var path = @"D:\DEV\Accessorie\Products\PoseAction\Headbob";
            var durations = new[] { 2.5f, 1.5f, 0.85f, 0.5f };
            var letters = new[] { "s", "m", "f", "v" };

            for (int i = 0; i < durations.Length; i++)
            {
                foreach (var trig in triggers)
                {
                    var anim = new Animation
                    {
                        Duration = durations[i],
                    };

                    var track1 = new Track { BoneID = 21 };
                    track1.Keyframes.AddRange(new[] {
                    new Keyframe { Time = 0, Rotation = trig.Quat21_1 },
                    new Keyframe { Time = anim.Duration/2, Rotation = trig.Quat21_2 },
                    new Keyframe { Time = anim.Duration, Rotation = trig.Quat21_1 },
                });

                    var track2 = new Track { BoneID = 22 };
                    track2.Keyframes.AddRange(new[] {
                    new Keyframe { Time = 0, Rotation = trig.Quat22_1 },
                    new Keyframe { Time = anim.Duration/2, Rotation = trig.Quat22_2 },
                    new Keyframe { Time = anim.Duration, Rotation = trig.Quat22_1 },
                });

                    anim.Tracks.AddRange(new[] { track1, track2 });

                    var filename = trig.Trigger.Replace("bh", "bh" + letters[i]) + ".xaf";
                    var filepath = Path.Combine(path, filename);
                    if (File.Exists(filepath)) File.Delete(filepath);

                    anim.Save(filepath);
                    Console.WriteLine(filepath);
                }

            }


            Template t = new Template { ParentProductID = 48704863 };
            var files = Directory.GetFiles(path, "*.xaf", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var triggerName = Path.GetFileNameWithoutExtension(file);
                var filename = Path.GetFileName(file);

                var a = new XAFLib.Action { Name = triggerName };
                a.Definition.ActionType = ActionType.Avatar;

                var ed = new EnsembleDefintion();
                ed.EnsembleAttributes.disablesGaze = true;

                var sk = new SkeletalAnimationEffect { AssetName = filename };
                sk.EffectControls.EffectCompositionFunction = EffectCompositionFunction.EffectCompositionFunctionReplace;
                sk.EffectControls.LoopIterations = 0;
                sk.EffectControls.RampUpFrames = 20;
                sk.EffectControls.PlaybackSpeedScale = 1;

                ed.SkeletalAnimationEffects.Add(sk);
                a.Definition.ActionDefinition.Ensembles.Add(ed);
                t.Actions.Add(a);

            }

            ChknFile.CreateChkn(path, t);


        }

    }
}
