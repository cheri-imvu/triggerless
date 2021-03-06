using System.Collections.Generic;
using System.Xml;
// ReSharper disable PossibleNullReferenceException

namespace XAFLib
{
    public class IAFFile {

        public SettingsClass Settings { get; private set; }
        public List<Skel> SkeletalAnimation { get;  }


        public IAFFile() {
            Settings = new SettingsClass();
            SkeletalAnimation = new List<Skel>();
        }


        public static IAFFile Open(string filename) {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            IAFFile result = new IAFFile();

            XmlElement settingsEl = doc.DocumentElement.SelectSingleNode("settings") as XmlElement;
            result.Settings.BodyMesh = bool.Parse((settingsEl.SelectSingleNode("bodyMesh") as XmlElement).InnerText);
            result.Settings.HideBody = bool.Parse((settingsEl.SelectSingleNode("hideBody") as XmlElement).InnerText);
            result.Settings.Gizmo = bool.Parse((settingsEl.SelectSingleNode("gizmo") as XmlElement).InnerText);
            result.Settings.CenterBone = bool.Parse((settingsEl.SelectSingleNode("centerBone") as XmlElement).InnerText);
            result.Settings.Zoom = bool.Parse((settingsEl.SelectSingleNode("zoom") as XmlElement).InnerText);
            result.Settings.FreeJoints = bool.Parse((settingsEl.SelectSingleNode("freeJoints") as XmlElement).InnerText);
            result.Settings.Idle = double.Parse((settingsEl.SelectSingleNode("idle") as XmlElement).InnerText);
            result.Settings.Play = double.Parse((settingsEl.SelectSingleNode("play") as XmlElement).InnerText);
            result.Settings.Duration = double.Parse((settingsEl.SelectSingleNode("durantion") as XmlElement).InnerText);

            XmlElement skelAnimNode = doc.DocumentElement.SelectSingleNode("skeletalAnimation") as XmlElement;
            foreach (XmlElement skelNode in skelAnimNode.ChildNodes) {
                result.SkeletalAnimation.Add(new Skel {
                    Frame = int.Parse(skelNode.GetAttribute("frame")),
                    Bone = int.Parse(skelNode.GetAttribute("bone")),
                    N11 = float.Parse((skelNode.SelectSingleNode("n11") as XmlElement).InnerText),
                    N12 = float.Parse((skelNode.SelectSingleNode("n12") as XmlElement).InnerText),
                    N13 = float.Parse((skelNode.SelectSingleNode("n13") as XmlElement).InnerText),
                    N14 = float.Parse((skelNode.SelectSingleNode("n14") as XmlElement).InnerText),
                    N21 = float.Parse((skelNode.SelectSingleNode("n21") as XmlElement).InnerText),
                    N22 = float.Parse((skelNode.SelectSingleNode("n22") as XmlElement).InnerText),
                    N23 = float.Parse((skelNode.SelectSingleNode("n23") as XmlElement).InnerText),
                    N24 = float.Parse((skelNode.SelectSingleNode("n24") as XmlElement).InnerText),
                    N31 = float.Parse((skelNode.SelectSingleNode("n31") as XmlElement).InnerText),
                    N32 = float.Parse((skelNode.SelectSingleNode("n32") as XmlElement).InnerText),
                    N33 = float.Parse((skelNode.SelectSingleNode("n33") as XmlElement).InnerText),
                    N34 = float.Parse((skelNode.SelectSingleNode("n34") as XmlElement).InnerText)
                });
            }
            return result;
        }

        public class SettingsClass {
            public bool BodyMesh { get; set; }
            public bool HideBody { get; set; }
            public bool Gizmo { get; set; }
            public bool CenterBone { get; set; }
            public bool Zoom { get; set; }
            public bool FreeJoints { get; set; }
            public double Idle { get; set; }
            public double Play { get; set; }
            public double Duration { get; set; }
        }

        public class Skel : Matrix3D {
            public int Frame { get; set; }
            public int Bone { get; set; }
        }
    }

    
}
