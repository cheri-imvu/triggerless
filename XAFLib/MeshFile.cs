using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace XAFLib
{
    public class MeshFile
    {
        public const string HEADER = "<HEADER MAGIC=\"XMF\" VERSION=\"919\"/>";

        public static Mesh LoadBinary(byte[] bytes)
        {
            return LoadBinaryStream(new MemoryStream(bytes));
        }

        public static Mesh LoadBinaryStream(Stream s)
        {
            Mesh mesh = new Mesh();
            s.Position = 0;
            const int MAGIC = 0x00464D43;
            const int VERSION = 0x00000397;
            BinaryReader br = new BinaryReader(s);
            int magic = br.ReadInt32();
            int version = br.ReadInt32();

            if (magic != MAGIC || version != VERSION) throw new ApplicationException("Improper magic or version");

            mesh.NumSubMesh = br.ReadInt32();

            for (int iSubMesh = 0; iSubMesh < mesh.NumSubMesh; iSubMesh++)
            {
                SubMesh subMesh = new SubMesh {
                    Material = br.ReadInt32(),
                    NumVertices = br.ReadInt32(),
                    NumFaces = br.ReadInt32(),
                    NumLodSteps = br.ReadInt32(),
                    NumSprings = br.ReadInt32(),
                    NumTexCoords = br.ReadInt32(),
                    NumMorphs = br.ReadInt32()
                };


                for (int iVertex = 0; iVertex < subMesh.NumVertices; iVertex++)
                {
                    Vertex v = new Vertex();
                    v.Pos = new Point3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    v.Norm = new Vector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    v.Color = new Point3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    v.CollapseID = br.ReadInt32();
                    v.FaceCollapseCount = br.ReadInt32();
                    v.ID = iVertex;
                    v.TexCoord = new Point2D(br.ReadSingle(), br.ReadSingle());
                    v.NumInfluences = br.ReadInt32();

                    for (int iInf = 0; iInf < v.NumInfluences; iInf++)
                    {
                        Influence inf = new Influence(br.ReadInt32(), br.ReadSingle());
                        v.Influences.Add(inf);
                    }

                    subMesh.Vertices.Add(v);
                    if (subMesh.NumSprings > 0) br.ReadSingle();  // I don't know why.
                }

                for (int iSpring = 0; iSpring < subMesh.NumSprings; iSpring++)
                {
                    // Read springs, but ditch them, we don't use them here
                    br.ReadInt32();
                    br.ReadInt32();
                    br.ReadSingle();
                    br.ReadSingle();
                }


                // I have no idea what I'm doing here.
                // How does one read a string here?
                for (int iMorph = 0; iMorph < subMesh.NumMorphs; iMorph++)
                {
                    string morphTarget = br.ReadString();

                    Morph m = new Morph {MorphID = iMorph};

                    int blendVertID = br.ReadInt32();

                    for (int iVertex = 0; iVertex < subMesh.NumVertices; iVertex++)
                    {
                        if (iVertex >= blendVertID)
                        {
                            BlendVertex bv = new BlendVertex {
                                VertexID = br.ReadInt32(),
                                Position = new Point3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
                                Normal = new Vector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle())
                            };

                            for (int iTexCoord = 0; iTexCoord < subMesh.NumTexCoords; iTexCoord++)
                            {
                                bv.TexCoords.Add(new Point2D(br.ReadSingle(), br.ReadSingle()));
                            }
                            m.BlendVertices.Add(bv);
                            blendVertID = br.ReadInt32();
                        }

                    }
                    subMesh.Morphs.Add(m);
                }

                for (int iFace = 0; iFace < subMesh.NumFaces; iFace++)
                {
                    subMesh.Faces.Add(new Face(br.ReadInt32(), br.ReadInt32(), br.ReadInt32()));
                }

                mesh.SubMeshes.Add(subMesh);
            }

            return mesh;
        }

        public static Mesh LoadBinaryFile(string filename)
        {
            return LoadBinaryStream(File.Open(filename, FileMode.Open));
        }

        public static Mesh LoadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<xml>" + xml + "</xml>");  // Cal3D files aren't valid xml, no root node
            return LoadXmlDocument(doc);
        }

        public static Mesh LoadXmlFile(string filename)
        {
            return LoadXml(File.ReadAllText(filename));
        }

        public static Mesh LoadXmlStream(Stream s)
        {
            s.Position = 0;
            int length = (int)s.Length;
            var buffer = new byte[length];
            s.Read(buffer, 0, length);
            return LoadXml(Encoding.UTF8.GetString(buffer));
        }

        protected static Mesh LoadXmlDocument(XmlDocument doc) {
            if (doc.DocumentElement == null) throw new XmlException("No doc element found");
            if (doc.DocumentElement.ChildNodes.Count != 2) throw new XmlException("Wrong number of XML fragments");
            if (doc.DocumentElement.Name != "xml") throw new XmlException("WTF I don't know what this is");
            XmlElement rootEl = doc.DocumentElement.ChildNodes[1] as XmlElement;
            if (rootEl == null) throw new XmlException("WTF I don't know what to do.");
            if (rootEl.Name != "MESH") throw new XmlException("Is this even an XMF mesh file?");
            Mesh result = new Mesh();
            foreach (XmlElement submeshEl in rootEl.ChildNodes) {
                SubMesh subMesh = new SubMesh();
                subMesh.Material = int.Parse(submeshEl.GetAttribute("MATERIAL"));
                subMesh.NumTexCoords = Int32.Parse(submeshEl.GetAttribute("NUMTEXCOORDS"));

                foreach (XmlElement childEl in submeshEl.ChildNodes) {
                    if (childEl.Name == "VERTEX") {
                        Vertex vx = new Vertex();

                        if (childEl.SelectSingleNode("POS") is XmlElement posEl) {
                            var bits = posEl.InnerText.Split(' ').Cast<float>().ToArray();
                            vx.Pos = new Point3D(bits[0], bits[1], bits[2]);
                        }

                        if (childEl.SelectSingleNode("NORM") is XmlElement normEl)
                        {
                            var bits = normEl.InnerText.Split(' ').Cast<float>().ToArray();
                            vx.Norm = new Vector(bits[0], bits[1], bits[2]);
                        }

                        if (childEl.SelectSingleNode("COLOR") is XmlElement colorEl)
                        {
                            var bits = colorEl.InnerText.Split(' ').Cast<float>().ToArray();
                            vx.Color = new Point3D(bits[0], bits[1], bits[2]);
                        }

                        if (childEl.SelectSingleNode("TEXCOORD") is XmlElement txcoordEl)
                        {
                            var bits = txcoordEl.InnerText.Split(' ').Cast<float>().ToArray();
                            vx.TexCoord = new Point2D(bits[0], bits[1]);
                        }

                        // ReSharper disable once PossibleNullReferenceException
                        foreach (XmlElement inflEl in childEl.SelectNodes("INFLUENCE"))
                        {
                            float weight = float.Parse(inflEl.InnerText);
                            int boneID = Int32.Parse(inflEl.GetAttribute("ID"));
                            vx.Influences.Add(new Influence(boneID, weight));
                        }

                        subMesh.Vertices.Add(vx);
                    }

                    if (childEl.Name == "FACE") {
                        var bits = childEl.InnerText.Split(' ').Cast<int>().ToArray();
                        subMesh.Faces.Add(new Face(bits[0], bits[1], bits[2]));
                    }

                    
                }
                result.SubMeshes.Add(subMesh);
            }
            return result;
        }

        public static void SaveXmlFile(Mesh mesh, string filePath) {
            var dir = Path.GetDirectoryName(filePath) ?? ".";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(filePath)) File.Delete(filePath);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<MESH />");
            var root = doc.DocumentElement;
            if (root == null) throw new XmlException("WTF I'm going to cry");

            root.SetAttribute("NUMSUBMESH", mesh.SubMeshes.Count.ToString());

            foreach (SubMesh subMesh in mesh.SubMeshes) {
                XmlElement subMeshEl = doc.CreateElement("SUBMESH");

                subMeshEl.SetAttribute("NUMVERTICES", subMesh.Vertices.Count.ToString());
                subMeshEl.SetAttribute("NUMFACES", subMesh.Faces.Count.ToString());
                subMeshEl.SetAttribute("MATERIAL", subMesh.Material.ToString());
                subMeshEl.SetAttribute("NUMLODSTEPS", subMesh.LodSteps.Count.ToString());
                subMeshEl.SetAttribute("NUMSPRINGS", subMesh.Springs.Count.ToString());
                subMeshEl.SetAttribute("NUMMORPHS", subMesh.Morphs.Count.ToString());
                subMeshEl.SetAttribute("NUMTEXCOORDS", subMesh.NumTexCoords.ToString());

                foreach (Vertex v in subMesh.Vertices) {
                    XmlElement vxEl = doc.CreateElement("VERTEX");
                    vxEl.SetAttribute("ID", v.ID.ToString());
                    vxEl.SetAttribute("NUMINFLUENCES", v.Influences.Count.ToString());

                    XmlElement posEl = doc.CreateElement("POS");
                    posEl.InnerText = $"{v.Pos.X} {v.Pos.Y} {v.Pos.Z}";
                    vxEl.AppendChild(posEl);

                    XmlElement normEl = doc.CreateElement("NORM");
                    normEl.InnerText = $"{v.Norm.X} {v.Norm.Y} {v.Norm.Z}";
                    vxEl.AppendChild(normEl);

                    XmlElement colorEl = doc.CreateElement("COLOR");
                    colorEl.InnerText = $"{v.Color.X} {v.Color.Y} {v.Color.Z}";
                    vxEl.AppendChild(colorEl);

                    XmlElement texcoordEl = doc.CreateElement("TEXCOORD");
                    texcoordEl.InnerText = $"{v.TexCoord.X} {v.TexCoord.Y}";
                    vxEl.AppendChild(texcoordEl);

                    foreach (Influence i in v.Influences) {
                        XmlElement inflEl = doc.CreateElement("INFLUENCE");
                        inflEl.SetAttribute("ID", i.BoneID.ToString());
                        inflEl.InnerText = $"{i.Weight}";
                        vxEl.AppendChild(inflEl);
                    }

                    subMeshEl.AppendChild(vxEl);
                }

                foreach (Face f in subMesh.Faces) {
                    XmlElement faceEl = doc.CreateElement("FACE");
                    faceEl.SetAttribute("VERTEXID", $"{f.VertexIDs[0]} {f.VertexIDs[1]} {f.VertexIDs[2]}");
                    subMeshEl.AppendChild(faceEl);
                }

                root.AppendChild(subMeshEl);

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                    Encoding = Encoding.UTF8
                };
                XmlWriter xtw = XmlWriter.Create(sb, settings);
                doc.Save(xtw);
                string contents = HEADER + Environment.NewLine + sb;
                File.WriteAllText(filePath, contents);

            }
        }

        public static async void SaveObjFiles(Mesh mesh, string folderPath, string filenameTemplate) {

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            for (int i = 0; i < mesh.SubMeshes.Count; i++) {
                var subMesh = mesh.SubMeshes[i];
                var sbVertex = new StringBuilder();
                var sbTexture = new StringBuilder();
                var sbNormal = new StringBuilder();
                var sbFace = new StringBuilder();

                foreach (var vertex in subMesh.Vertices) {
                    sbVertex.AppendLine($"v {vertex.Pos.X} {vertex.Pos.Y} {vertex.Pos.Z}");
                    sbTexture.AppendLine($"vt {vertex.TexCoord.X} {vertex.TexCoord.Y}");
                    sbNormal.AppendLine($"vn {vertex.Norm.X} {vertex.Norm.Y} {vertex.Norm.Z}");
                }

                foreach (var face in subMesh.Faces) {
                    sbFace.AppendLine($"f {face.VertexIDs[0]} {face.VertexIDs[1]} {face.VertexIDs[2]}");
                }

                string filename = $@"{folderPath}\{filenameTemplate}-{i:00}.obj";
                if (File.Exists(filename)) File.Delete(filename);
                using (var sw = File.CreateText(filename)) {
                    await sw.WriteLineAsync(sbVertex.ToString());
                    await sw.WriteLineAsync(sbTexture.ToString());
                    await sw.WriteLineAsync(sbNormal.ToString());
                    await sw.WriteLineAsync(sbFace.ToString());
                }
            }

        }

    }
}
