using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    public class MorphAnimation
    {
        public float Duration { get; set; } 
        public List<MorphTrack> Tracks { get; set; }

        public MorphAnimation() {
            Tracks = new List<MorphTrack>();
        }

        public static MorphAnimation LoadBinary(Stream stream) {
            MorphAnimation result = new MorphAnimation();

            using (BinaryReader rdr = new BinaryReader(stream)) {
                var header = rdr.ReadBytes(4);
                var version = rdr.ReadSingle();
                result.Duration = rdr.ReadSingle();

                int trackCount = rdr.ReadInt32();
                for (int iTrack = 0; iTrack < trackCount; iTrack++) {
                    MorphTrack track = new MorphTrack();
                    int nameLen = rdr.ReadInt32();
                    byte[] nameBytes = rdr.ReadBytes(nameLen - 1);

                    track.Name = Encoding.ASCII.GetString(nameBytes);
                    byte toss = rdr.ReadByte();

                    int keyFrameCount = rdr.ReadInt32();
                    for (int iFrame = 0; iFrame < keyFrameCount; iFrame++) {
                        MorphKeyFrame frame = new MorphKeyFrame();
                        frame.Time = rdr.ReadSingle();
                        frame.Weight = rdr.ReadSingle();
                        track.KeyFrames.Add(frame);
                    }
                    result.Tracks.Add(track);
                }
            }

            return result;
        }

        public static MorphAnimation LoadBinary(string fileName) {
            using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
                return LoadBinary(fs);
            }
        }

        public static MorphAnimation LoadBinary(byte[] bytes) {
            using (MemoryStream ms = new MemoryStream(bytes)) {
                return LoadBinary(ms);
            }
        }

        public string WriteXml() {
            string header = "<HEADER MAGIC=\"XPF\" VERSION=\"919\" />\n";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<ANIMATION />");
            doc.DocumentElement.SetAttribute("NUMTRACKS", Tracks.Count.ToString());
            doc.DocumentElement.SetAttribute("DURATION", Duration.ToString());

            foreach (var track in Tracks) {
                XmlElement elTrack = doc.CreateElement("TRACK");
                elTrack.SetAttribute("NUMKEYFRAMES", track.KeyFrames.Count.ToString());
                elTrack.SetAttribute("MORPHNAME", track.Name);
                foreach (var kf in track.KeyFrames) {
                    XmlElement elFrame = doc.CreateElement("KEYFRAME");
                    elFrame.SetAttribute("TIME", kf.Time.ToString());
                    XmlElement elWeight = doc.CreateElement("WEIGHT");
                    elWeight.InnerText = kf.Weight.ToString();
                    elFrame.AppendChild(elWeight);
                    elTrack.AppendChild(elFrame);
                }
                doc.DocumentElement.AppendChild(elTrack);
            }

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings {
                Encoding = Encoding.ASCII,
                Indent = true,
                OmitXmlDeclaration = true,
            };
            XmlWriter xw = XmlWriter.Create(sb, settings);
            doc.Save(xw);
            xw.Dispose();

            return header + sb.ToString();
        }

        public void WriteXml(string filename) {
            File.WriteAllText(filename, WriteXml());
        }

        public byte[] WriteBytes() {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[]{0x43, 0x50, 0x46, 0x00, 0x97, 0x03, 0x00, 0x00});
            bytes.AddRange(BitConverter.GetBytes(Duration));
            bytes.AddRange(BitConverter.GetBytes(Tracks.Count));

            foreach (var track in Tracks) {
                
            }


            return bytes.ToArray();
        }

        private static Mapper _namesMap;

        public static Mapper NamesMap
        {
            get {
                if (_namesMap == null) {
                    _namesMap = new Mapper();
                    _namesMap["kll"] = "blink.Left.Lower.Clamped";
                    _namesMap["klu"] = "blink.Left.Upper.Clamped";
                    _namesMap["krl"] = "blink.Right.Lower.Clamped";
                    _namesMap["kru"] = "blink.Right.Upper.Clamped";
                    _namesMap["bla"] = "eyebrow.Left.Angry.Average";
                    _namesMap["bld"] = "eyebrow.Left.Down.Average";
                    _namesMap["bra"] = "eyebrow.Right.Angry.Average";
                    _namesMap["brd"] = "eyebrow.Right.Down.Average";
                    _namesMap["blc"] = "eyebrows.Left.Center.Up.Average";
                    _namesMap["bli"] = "eyebrows.Left.In.Average";
                    _namesMap["blm"] = "eyebrows.Left.Mad.Average";
                    _namesMap["blo"] = "eyebrows.Left.OuterDown.Average";
                    _namesMap["blp"] = "eyebrows.Left.OuterUp.Average";
                    _namesMap["bls"] = "eyebrows.Left.Sad.Average";
                    _namesMap["blu"] = "eyebrows.Left.Up.Average";
                    _namesMap["brc"] = "eyebrows.Right.Center.Up.Average";
                    _namesMap["bri"] = "eyebrows.Right.In.Average";
                    _namesMap["brm"] = "eyebrows.Right.Mad.Average";
                    _namesMap["bro"] = "eyebrows.Right.OuterDown.Average";
                    _namesMap["brp"] = "eyebrows.Right.OuterUp.Average";
                    _namesMap["brs"] = "eyebrows.Right.Sad.Average";
                    _namesMap["bru"] = "eyebrows.Right.Up.Average";
                    _namesMap["ibl"] = "Eyes.Bugged.Left.Average";
                    _namesMap["ibr"] = "Eyes.Bugged.Right.Average";
                    _namesMap["ipl"] = "Eyes.POP.Left.Average";
                    _namesMap["ipr"] = "Eyes.POP.Right.Average";
                    _namesMap["isl"] = "Eyes.Scale.Left.Average";
                    _namesMap["isr"] = "Eyes.Scale.Right.Average";
                    _namesMap["eld"] = "left.Eye.Down.Clamped";
                    _namesMap["ell"] = "left.Eye.Left.Clamped";
                    _namesMap["elr"] = "left.Eye.Right.Clamped";
                    _namesMap["elu"] = "left.Eye.Up.Clamped";
                    _namesMap["mbc"] = "mouth.BigSmile.Exclusive";
                    _namesMap["mbl"] = "mouth.BigSmileLEFT.Exclusive";
                    _namesMap["mbr"] = "mouth.BigSmileRIGHT.Exclusive";
                    _namesMap["mcl"] = "mouth.CheekLEFT.Exclusive";
                    _namesMap["mcr"] = "mouth.CheekRIGHT.Exclusive";
                    _namesMap["mcc"] = "mouth.Cheeks.Exclusive";
                    _namesMap["mdn"] = "mouth.Down.Exclusive";
                    _namesMap["mfc"] = "mouth.Frown.Exclusive";
                    _namesMap["mfl"] = "mouth.FrownLEFT.Exclusive";
                    _namesMap["mfr"] = "mouth.FrownRIGHT.Exclusive";
                    _namesMap["mgc"] = "mouth.Grin.Exclusive";
                    _namesMap["mgl"] = "mouth.GrinLeft.Exclusive";
                    _namesMap["mgr"] = "mouth.GrinRight.Exclusive";
                    _namesMap["mjc"] = "mouth.JawDown.Exclusive";
                    _namesMap["mkl"] = "mouth.LeftSmirk.Exclusive";
                    _namesMap["mlc"] = "mouth.MadLip.Exclusive";
                    _namesMap["mll"] = "mouth.MadLipLEFT.Exclusive";
                    _namesMap["mlr"] = "mouth.MadLipRIGHT.Exclusive";
                    _namesMap["mnc"] = "mouth.Narrow.Exclusive";
                    _namesMap["mnl"] = "mouth.NarrowLeft.Exclusive";
                    _namesMap["mnr"] = "mouth.NarrowRight.Exclusive";
                    _namesMap["moh"] = "mouth.OH.Exclusive";
                    _namesMap["mop"] = "mouth.Open.Exclusive";
                    _namesMap["mpk"] = "mouth.Pucker.Exclusive";
                    _namesMap["mkr"] = "mouth.RightSmirk.Exclusive";
                    _namesMap["msr"] = "mouth.Scream.Exclusive";
                    _namesMap["msc"] = "mouth.Smile.Exclusive";
                    _namesMap["msl"] = "mouth.SmileLeft.Exclusive";
                    _namesMap["msr"] = "mouth.SmileRight.Exclusive";
                    _namesMap["mtd"] = "mouth.TongueDown.Exclusive";
                    _namesMap["mto"] = "mouth.TongueOut.Exclusive";
                    _namesMap["mtr"] = "mouth.TongueRIGHT.Exclusive";
                    _namesMap["mtt"] = "mouth.TongueTHIN.Exclusive";
                    _namesMap["mtu"] = "mouth.TongueUP.Exclusive";
                    _namesMap["mup"] = "mouth.Up.Exclusive";
                    _namesMap["mwc"] = "mouth.Wide.Exclusive";
                    _namesMap["mwl"] = "mouth.WideLeft.Exclusive";
                    _namesMap["mwr"] = "mouth.WideRight.Exclusive";
                    _namesMap["nfc"] = "nose.Flare.Exclusive";
                    _namesMap["nfl"] = "nose.FlareLeft.Exclusive";
                    _namesMap["nfr"] = "nose.FlareRight.Exclusive";
                    _namesMap["nlg"] = "nose.Long.Exclusive";
                    _namesMap["erd"] = "right.Eye.Down.Clamped";
                    _namesMap["erl"] = "right.Eye.Left.Clamped";
                    _namesMap["err"] = "right.Eye.Right.Clamped";
                    _namesMap["eru"] = "right.Eye.Up.Clamped";

                }
                return _namesMap;
            }
        }
    }

    public class MorphTrack {
        public string Name { get; set; }
        public List<MorphKeyFrame> KeyFrames { get; set; }

        public MorphTrack() {
            KeyFrames = new List<MorphKeyFrame>();
        }
    }

    public class MorphKeyFrame {
        public float Time { get; set; }
        public float Weight { get; set; }
    }
}
