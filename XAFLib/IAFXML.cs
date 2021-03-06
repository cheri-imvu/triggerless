using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace XAFLib {
    // ReSharper disable InconsistentNaming
    public class IAFXML {
        public static imvu Parse(string filename) {
            var stream = File.OpenRead(filename);
            XmlReader xmlReader = XmlReader.Create(stream,
                                                new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Document});
            return new XmlSerializer(typeof (imvu)).Deserialize(xmlReader) as imvu;
        }

        public static void Save(imvu imvu, string filename) {
            if (File.Exists(filename)) File.Delete(filename);
            var stream = File.Create(filename);
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = true,
                Encoding = Encoding.ASCII
            }) ;
            new XmlSerializer(typeof(imvu)).Serialize(writer, imvu);
        }

        [XmlType(AnonymousType = true)]
        [XmlRoot(Namespace = "", IsNullable = false)]
        public class imvu {
            public imvuSettings settings { get; set; }
            [XmlArrayItem("skel", IsNullable = false)] public imvuSkel[] skeletalAnimation { get; set; }
            public imvuMorphAnimation morphAnimation { get; set; }
            public void Save(string filename) {IAFXML.Save(this, filename);}
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimation {
            public imvuMorphAnimationXpf xpf { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpf {
            public imvuMorphAnimationXpfXml xml { get; set; }
            [XmlAttribute] public string name { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpfXml {
            public imvuMorphAnimationXpfXmlHEADER HEADER { get; set; }
            public imvuMorphAnimationXpfXmlANIMATION ANIMATION { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpfXmlANIMATION {
            [XmlElement("TRACK")]
            public imvuMorphAnimationXpfXmlANIMATIONTRACK[] TRACK { get; set; }
            [XmlAttribute] public ushort NUMTRACKS { get; set; }
            [XmlAttribute] public double DURATION { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpfXmlANIMATIONTRACK {
            public imvuMorphAnimationXpfXmlANIMATIONTRACKKEYFRAME KEYFRAME { get; set; }
            [XmlAttribute] public ushort NUMKEYFRAMES { get; set; }
            [XmlAttribute] public string MORPHNAME { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpfXmlANIMATIONTRACKKEYFRAME {
            public int WEIGHT { get; set; }
            [XmlAttribute] public double TIME { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuMorphAnimationXpfXmlHEADER {
            [XmlAttribute] public string MAGIC { get; set; }
            [XmlAttribute] public ushort VERSION { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuSettings {
            public bool bodyMesh { get; set; }
            public bool hideBody { get; set; }
            public bool gizmo { get; set; }
            public bool centerBone { get; set; }
            public bool zoom { get; set; }
            public bool freeJoints { get; set; }
            public double idle { get; set; }
            public double play { get; set; }
            public double duration { get; set; }
        }

        [XmlType(AnonymousType = true)]
        public class imvuSkel {
            public Matrix3D GetMatrix() {
                return new Matrix3D(new[] {n11,n12,n13,n14,n21,n22,n23,n24,n31,n32,n33,n34});
            }

            public void SetMatrix(Matrix3D m) {
                n11 = m.N11; n12 = m.N12; n13 = m.N13; n14 = m.N14;
                n21 = m.N21; n22 = m.N22; n23 = m.N23; n24 = m.N24;
                n31 = m.N31; n32 = m.N32; n33 = m.N33; n34 = m.N34;
            }

            public imvuSkel Clone() {
                return new imvuSkel {
                    bone = bone, frame = frame,
                    n11 = n11, n12 = n12, n13 = n13, n14 = n14,
                    n21 = n21, n22 = n22, n23 = n23, n24 = n24,
                    n31 = n31, n32 = n32, n33 = n33, n34 = n34
                };
            }

            public float n11 { get; set; }
            public float n12 { get; set; }
            public float n13 { get; set; }
            public float n14 { get; set; }
            public float n21 { get; set; }
            public float n22 { get; set; }
            public float n23 { get; set; }
            public float n24 { get; set; }
            public float n31 { get; set; }
            public float n32 { get; set; }
            public float n33 { get; set; }
            public float n34 { get; set; }
            [XmlAttribute] public int frame { get; set; }
            [XmlAttribute] public int bone { get; set; }
        }
    }
}