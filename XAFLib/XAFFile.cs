using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XAFLib {
    public class XAFFile {
        public const string HEADER = "<HEADER MAGIC=\"XAF\" VERSION=\"919\"/>";

        public Animation Load(string filename) {
            if (!File.Exists(filename)) {
                throw new FileNotFoundException();
            }

            string allText = File.ReadAllText(filename).Trim();
            return LoadFromString(allText);
        }

        public Animation Load(Stream stream) {
            long size = stream.Length;
            byte[] bytes = new byte[size];
            stream.Read(bytes, 0, (int)size);
            string allText = System.Text.Encoding.ASCII.GetString(bytes);
            return LoadFromString(allText);
        }

        public Animation LoadFromString(string allText) {

            string pattern = string.Format(@"<HEADER\s+MAGIC\s*=\s*{0}XAF{0}\s+VERSION\s*=\s*{0}919{0}\s*/>", "\"");
            string snippet = allText.Substring(0, 50);
            Match match = Regex.Match(snippet, pattern);

            if (!match.Success) throw new XAFFileFormatException("XAF Header is missing from file");
            string xml = allText.Replace(match.Value, string.Empty);
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlElement root = doc.DocumentElement;
            if (root == null) throw new XAFFileFormatException("The XML from this XAF could not be parsed correctly");
            if (root.Name != "ANIMATION") throw new XAFFileFormatException("XML document element is not an ANIMATION element");

            var anim = new Animation {
                NumTracks = root.GetAttribute("NUMTRACKS").ParseInt32(),
                Duration = root.GetAttribute("DURATION").ParseFloat()
            };

            XmlNodeList trackNodes = root.SelectNodes("TRACK");
            if (trackNodes == null) throw new XAFFileFormatException("XAF Animation contains no Tracks");

            foreach (XmlElement trackEl in trackNodes) {
                var track = new Track {
                    BoneID = trackEl.GetAttribute("BONEID").ParseInt32(),
                    NumKeyFrames = trackEl.GetAttribute("NUMKEYFRAMES").ParseInt32(),
                    TranslationRequired = trackEl.GetAttributeBoolean("TRANSLATIONREQUIRED"),
                    TranslationDynamic = trackEl.GetAttributeBoolean("TRANSLATIONDYNAMIC"),
                    HighRangeRequired = trackEl.GetAttributeBoolean("HIGHRANGEREQUIRED")
                };

                XmlNodeList keyframeNodes = trackEl.SelectNodes("KEYFRAME");
                if (keyframeNodes != null) {
                    foreach (XmlElement keyframeEl in keyframeNodes) {
                        track.Keyframes.Add(new Keyframe {
                            Time = keyframeEl.GetAttribute("TIME").ParseFloat(),
                            Rotation = keyframeEl.SelectSingleNode("ROTATION").ParseQuaternion(),
                            Translation = keyframeEl.SelectSingleNode("TRANSLATION").ParseVector()
                        });
                    }
                }
                anim.Tracks.Add(track);
            }

            return anim;
        }

        public Animation LoadBinary(string filename) {

            const int MAGIC = 0x00464143;
            const int VERSION = 0x00000397;
            if (!File.Exists(filename)) throw new FileNotFoundException();

            Animation anim = new Animation();
            using (BinaryReader file = new BinaryReader(File.OpenRead(filename))) {
                try {
                    int magic = file.ReadInt32();
                    if (magic != MAGIC) throw new XAFFileFormatException("Binary header is not CAF\\0");

                    int ver = file.ReadInt32();
                    if (ver != VERSION) throw new XAFFileFormatException("Unknown version, must be 919");

                    /* int compressionOptions = NOT USED */ file.ReadInt32();
                    anim.Duration = file.ReadSingle();
                    anim.NumTracks = file.ReadInt32();

                    for (int iTrack = 0; iTrack < anim.NumTracks; iTrack++) {
                        Track track = new Track {
                            BoneID = file.ReadInt32(), 
                            NumKeyFrames = file.ReadInt32()
                        };

                        for (int iFrame = 0; iFrame < track.NumKeyFrames; iFrame++) {
                            Keyframe frame = new Keyframe {Time = file.ReadSingle()};
                            Vector translation = new Vector {
                                X = file.ReadSingle(),
                                Y = file.ReadSingle(),
                                Z = file.ReadSingle()
                            };
                            frame.Translation = translation;
                            Quaternion rotation = new Quaternion {
                                X = file.ReadSingle(),
                                Y = file.ReadSingle(),
                                Z = file.ReadSingle(),
                                W = file.ReadSingle()
                            };
                            frame.Rotation = rotation;
                            track.Keyframes.Add(frame);
                        }
                        anim.Tracks.Add(track);
                    }
                } catch (Exception e) {
                    throw new XAFFileFormatException("An error occurred while reading binary CAF file", e);
                }
            }
            return anim;
        }

        public void Save(string filename, Animation animation)
        {
            if (animation == null) return;
            if (File.Exists(filename)) File.Delete(filename);
            File.WriteAllText(filename, AsXml(animation));
        }

        public string AsXml(Animation animation) { 

            var doc = new XmlDocument();
            doc.LoadXml("<ANIMATION></ANIMATION>");
            XmlElement root = doc.DocumentElement;
            if (root == null) throw new ApplicationException("Resharper sucks");

            root.SetAttribute("NUMTRACKS", animation.Tracks.Count.ToString());
            root.SetAttribute("DURATION", animation.Duration.ToString(CultureInfo.InvariantCulture));

            foreach (Track track in animation.Tracks) {
                XmlElement trackEl = doc.CreateElement("TRACK");
                trackEl.SetAttribute("BONEID", track.BoneID.ToString());
                trackEl.SetAttribute("NUMKEYFRAMES", track.Keyframes.Count.ToString());
                if (track.TranslationRequired.HasValue) {
                    trackEl.SetAttribute("TRANSLATIONREQUIRED", track.TranslationRequired.Value ? "1" : "0");
                }
                if (track.TranslationDynamic.HasValue) {
                    trackEl.SetAttribute("TRANSLATIONDYNAMIC", track.TranslationDynamic.Value ? "1" : "0");
                }
                if (track.HighRangeRequired.HasValue) {
                    trackEl.SetAttribute("HIGHRANGEREQUIRED", track.HighRangeRequired.Value ? "1" : "0");
                }

                foreach (Keyframe keyframe in track.Keyframes) {
                    XmlElement keyframeEl = doc.CreateElement("KEYFRAME");
                    keyframeEl.SetAttribute("TIME", keyframe.Time.ToString(CultureInfo.CurrentCulture));

                    if (!keyframe.Translation.IsZero) {
                        XmlElement transEl = doc.CreateElement("TRANSLATION");
                        Vector t = keyframe.Translation;
                        transEl.InnerXml = t.X + " " + t.Y + " " + t.Z;
                        keyframeEl.AppendChild(transEl);
                    }

                    XmlElement rotEl = doc.CreateElement("ROTATION");
                    Quaternion r = keyframe.Rotation;
                    rotEl.InnerXml = r.X.Nudge() + " " + r.Y.Nudge() + " " + r.Z.Nudge() + " " + r.W.Nudge();
                    keyframeEl.AppendChild(rotEl);

                    trackEl.AppendChild(keyframeEl);
                }

                root.AppendChild(trackEl);
            }

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };
            XmlWriter xtw = XmlWriter.Create(sb, settings);
            doc.Save(xtw);
            string contents = HEADER + Environment.NewLine + sb;
            return contents;
        }
    }

    public class XAFFileFormatException : Exception {
        public XAFFileFormatException() {}
        public XAFFileFormatException(string message) : base(message) {}
        public XAFFileFormatException(string message, Exception innerException) : base(message, innerException) {}
    }
}