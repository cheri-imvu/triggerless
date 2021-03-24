using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Triggerless.XAFLib
{

    public class Pid {
        public const int EmptyDerivableByTriggers = 38766202;
    }

    [Serializable]


    public class Template {
        public int ParentProductID { get; set; }
        public List<Action> Actions { get; }
        public Template() {
            ParentProductID = 38766202; //default Empty Accessory by Triggers
            Actions = new List<Action>();
        }

        public static Template Load(string filename) {
            if (!File.Exists(filename)) throw new FileNotFoundException();
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement rootElement = doc.DocumentElement;
            if (rootElement == null || rootElement.Name != "Template") throw new XmlException("Invalid document");
            Template result = new Template();

            result.ParentProductID = 
                int.Parse(rootElement.ChildNodes[0].InnerText
                .Replace("product://","")
                .Replace("/index.xml",""));

            for (int i = 1; i < rootElement.ChildNodes.Count; i++) {
                XmlNode aNode = rootElement.ChildNodes[i];
                if (aNode.Name.StartsWith("Action")) {
                    Action a = new Action();
                    a.LoadXml(rootElement.ChildNodes[i]);
                    result.Actions.Add(a);
                }
            }
            return result;
        }

        public void AddSingleAction(int i, Quaternion q, char c, int boneId, string targetDir, int offset = 0)
        {
            var actionName = c + "h" + (i + offset).ToString("00");
            int q1 = c == 'l' ? 1 : -1;
            var quat = new Quaternion(q1 * q.X, q.Y, q1 * q.Z, q.W);

            var anim = new Animation { Duration = 10, NumTracks = 1 };
            Track track = new Track { BoneID = boneId };
            track.Keyframes.Add(new Keyframe { Rotation = quat, Time = 0 });
            anim.Tracks.Add(track);
            anim.Save(Path.Combine(targetDir, $"{actionName}.xaf"));

            Action action = new Action { Name = actionName };
            action.Definition.ActionDefinition.ActionAttributes.ActionEnsemblePicking =
                ActionEnsemblePicking.ActionEnsemblePickingOncePerIteration;
            EnsembleDefintion ensemble = new EnsembleDefintion();
            ensemble.SkeletalAnimationEffects.Add(
                new SkeletalAnimationEffect
                {
                    AssetName = $"{actionName}.xaf",
                    EffectControls = {
                        EffectCompositionFunction = EffectCompositionFunction.EffectCompositionFunctionReplace,
                        LoopIterations = 0,
                        RampUpFrames = 20
                    }
                }
            );
            action.Definition.ActionType = ActionType.Avatar;
            action.Definition.ActionDefinition.Ensembles.Add(ensemble);
            this.Actions.Add(action);
        }


        public string GetIndexXml() {
            StringBuilder sb = new StringBuilder();
            sb.AppendUnixLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendUnixLine("<Template>");
            sb.Append("<__DATAIMPORT>");
            sb.Append("product://");
            sb.Append(ParentProductID);
            sb.Append("/index.xml");
            sb.Append("</__DATAIMPORT>");
            for (int i = 0; i < Actions.Count; i++ ) {
                Actions[i].AppendText(sb, i);
            }
            sb.AppendUnixLine("</Template>");
            return sb.ToString();
        }

        public XmlDocument GetXml() {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Template />");
            XmlElement di = doc.CreateElement("__DATAIMPORT");
            di.InnerText = "product://" + ParentProductID + "/index.xml";
            if (doc.DocumentElement != null) {
                doc.DocumentElement.AppendChild(di);
                for (int i = 0; i < Actions.Count; i++) {
                    Action a = Actions[i];
                    a.AddXml(doc.DocumentElement, i);
                }
            }
            return doc;
        }
    }

}
