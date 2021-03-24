using System;
using System.Collections.Generic;

namespace Triggerless.XAFLib
{

    public class Animation {
        public Animation() {
            Tracks = new List<Track>();
        }

        public int NumTracks { get; set; }
        public float Duration { get; set; }
        public List<Track> Tracks { get; }

        public void Save(string filename) {
            new XAFFile().Save(filename, this);
        }
    }

    public class Track {
        public Track() {
            Keyframes = new List<Keyframe>();
        }

        public int BoneID { get; set; }
        public bool? TranslationRequired { get; set; }
        public bool? TranslationDynamic { get; set; }
        public bool? HighRangeRequired { get; set; }
        public int NumKeyFrames { get; set; }
        public List<Keyframe> Keyframes { get; }
    }

    public class Keyframe {
        public Keyframe() {
            Rotation = new Quaternion();
            Translation = new Vector();
        }

        public float Time { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector Translation { get; set; }

        public override string ToString() {
            return $"[{Time} {Translation} {Rotation}]";
        }
    }

}