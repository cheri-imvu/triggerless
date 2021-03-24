using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triggerless.XAFLib
{
    public class Mesh {
        public int NumSubMesh { get; set; }
        public List<SubMesh> SubMeshes { get; }

        public Mesh() {
            SubMeshes = new List<SubMesh>();
        }
    }

    public struct Point2D {
        public float X { get; set; }
        public float Y { get; set; }

        public Point2D(float x, float y) {
            X = x;
            Y = y;
        }
    }

    public struct Point3D {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3D(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class SubMesh {
        public int NumVertices { get; set; }
        public int NumFaces { get; set; }
        public int Material { get; set; }
        public int NumLodSteps { get; set; }
        public int NumSprings { get; set; }
        public int NumTexCoords { get; set; }
        public int NumMorphs { get; set; }
        public List<Vertex> Vertices { get; private set; }
        public List<Face> Faces { get; private set; }
        public List<LodStep> LodSteps { get; private set; }
        public List<Spring> Springs { get; private set; }
        public List<Morph> Morphs { get; private set; }

        public SubMesh() {
            Vertices = new List<Vertex>();
            Faces = new List<Face>();
            LodSteps = new List<LodStep>();
            Springs = new List<Spring>();
            Morphs = new List<Morph>();
        }
    }

    public class Spring {}

    public class LodStep {}

    public class Face {
        public int[] VertexIDs { get; private set; }

        public Face() {
            VertexIDs = new int[3];
        }

        public Face(int id0, int id1, int id2) {
            VertexIDs = new int[3];
            VertexIDs[0] = id0;
            VertexIDs[1] = id1;
            VertexIDs[2] = id2;
        }
    }

    public class Morph {
        public string Name { get; set; }
        public int MorphID { get; set; }
        public List<BlendVertex> BlendVertices { get; private set; }

        public Morph() {
            BlendVertices = new List<BlendVertex>();
        }
    }

    public class BlendVertex {
        public int VertexID { get; set; }
        public double PosDiff { get; set; }
        public Point3D Position { get; set; }
        public Vector Normal { get; set; }
        public List<Point2D> TexCoords { get; set; }

        public BlendVertex() {
            TexCoords = new List<Point2D>();
        }
    }

    public class Vertex {
        public int ID { get; set; }
        public Point3D Pos { get; set; }
        public Vector Norm { get; set; }
        public Point3D Color { get; set; }
        public int CollapseID { get; set; }
        public int FaceCollapseCount { get; set; }
        public Point2D TexCoord { get; set; }
        public int NumInfluences { get; set; }
        public List<Influence> Influences { get; set; }

        public Vertex() {
            Influences = new List<Influence>();
        }
    }

    public class Influence {
        public int BoneID { get; set; }
        public float Weight { get; set; }

        public Influence(int boneID, float weight) {
            BoneID = boneID;
            Weight = weight;
        }
    }
}
