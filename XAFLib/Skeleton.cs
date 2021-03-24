using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Triggerless.XAFLib
{
    public class Skeleton {

        private List<Bone> _bones;
        public IEnumerable<Bone> Bones {
            get { return _bones; }
        }
        public Vector SceneAmbientColor { get; set; }
        public int NumBones { get; private set; }

        public Skeleton() {
            Initialize();
        }

        public XmlDocument SkeletonXmlDoc {
            get {
                var doc = new XmlDocument();
                const string STREAM_NAME = "XAFLib.Skeleton.xsf";
                using (Stream xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(STREAM_NAME)) {
                    if (xmlStream != null) doc.Load(xmlStream);
                }
                return doc;
            }
        }

        private void Initialize() {
            _bones = new List<Bone>();
            XmlDocument doc = SkeletonXmlDoc;
            XmlElement root = doc.DocumentElement;
            if (root == null) throw new ApplicationException("Skeleton XSF Resource not found");


            NumBones = root.GetAttribute("NUMBONES").ParseInt32();
            SceneAmbientColor = root.GetAttribute("SCENEAMBIENTCOLOR").ParseVector();
            XmlNodeList boneEls = root.SelectNodes("BONE");
            if (boneEls == null) return;

            foreach (XmlElement boneEl in boneEls) {
                if (boneEl == null) continue;
                var bone = new Bone(this);
                const int DEFAULT_VALUE = -1;
                bone.BoneID = boneEl.GetAttribute("ID").ParseIntOrDefault(DEFAULT_VALUE);
                if (bone.BoneID == DEFAULT_VALUE) continue;
                bone.Name = boneEl.GetAttribute("NAME");
                bone.NumChilds = boneEl.GetAttribute("NUMCHILDS").ParseInt32();
                bone.Region = boneEl.GetAttribute("region").ParseBoneRegion();
                bone.Tags = boneEl.GetAttribute("tags").ParseBoneTags();
                bone.Translation = boneEl.SelectSingleNode("TRANSLATION").ParseVector();
                bone.Rotation = boneEl.SelectSingleNode("ROTATION").ParseQuaternion();
                bone.LocalTranslation = boneEl.SelectSingleNode("LOCALTRANSLATION").ParseVector();
                bone.LocalRotation = boneEl.SelectSingleNode("LOCALROTATION").ParseQuaternion();
                bone.ParentID = boneEl.SelectSingleNode("PARENTID").ParseIntOrDefault(DEFAULT_VALUE);

                XmlNodeList childEls = boneEl.SelectNodes("CHILDID");
                if (childEls != null) foreach (XmlElement childIdEl in childEls) {
                        int i = childIdEl.ParseIntOrDefault(DEFAULT_VALUE);
                        if (i != DEFAULT_VALUE) bone.ChildIDs.Add(i);
                }

                _bones.Add(bone);
            }
        }
    }
}