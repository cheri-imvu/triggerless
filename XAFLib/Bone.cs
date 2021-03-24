using System.Collections.Generic;
using System.Linq;

namespace Triggerless.XAFLib
{
    public class Bone {
        public Bone(Skeleton owner) {
            Owner = owner;
            ChildIDs = new List<int>();
        }

        public int BoneID { get; internal set; }
        public string Name { get; internal set; }
        public int NumChilds { get; internal set; }
        public Vector Translation { get; internal set; }
        public Quaternion Rotation { get; internal set; }
        public Vector LocalTranslation { get; internal set; }
        public Quaternion LocalRotation { get; internal set; }
        public int ParentID { get; internal set; }
        public List<int> ChildIDs { get; internal set; }
        public BoneRegion Region { get; internal set; }
        public BoneTags Tags { get; internal set; }

        internal Skeleton Owner { get; set; }

        public Bone Parent {
            get {
                return ParentID == -1 ? null : Owner.Bones.First(b => b.BoneID == ParentID);
            }
        }

        public IEnumerable<Bone> Children {
            get {
                return Owner.Bones.Where(b => ChildIDs.Contains(b.BoneID));
            }
        }

        public override string ToString() {
            return string.Format("{{{0} ID:{1}}}", Name, BoneID);
        }

        // ReSharper disable InconsistentNaming
        public const int Female03MasterRoot = 0;
        public const int PelvisNode = 1;
        public const int lfHip = 2;
        public const int lfThigh = 3;
        public const int lfCalf = 4;
        public const int lfFoot = 5;
        public const int lfToes = 6;
        public const int xTipBone86 = 7;
        public const int rtHip = 8;
        public const int rtThigh = 9;
        public const int rtCalf = 10;
        public const int rtFoot = 11;
        public const int rtToes = 12;
        public const int xTipBone38 = 13;
        public const int Spine01 = 14;
        public const int Spine02 = 15;
        public const int Spine03 = 16;
        public const int Spine04 = 17;
        public const int Neck01 = 18;
        public const int Neck02 = 19;
        public const int Neck03 = 20;
        public const int Neck04 = 21;
        public const int Head = 22;
        public const int xTipBone33 = 23;
        public const int lfClavicle = 24;
        public const int lfShoulder = 25;
        public const int lfbicep = 26;
        public const int lfElbow = 27;
        public const int lfWrist = 28;
        public const int lfHand = 29;
        public const int lfmetaCarpal03 = 30;
        public const int lfFingerMiddle01 = 31;
        public const int lfFingerMiddle02 = 32;
        public const int lfFingerMiddle03 = 33;
        public const int xTipBone87 = 34;
        public const int lfmetaCarpal01 = 35;
        public const int lfThumb01d = 36;
        public const int lfThumb02 = 37;
        public const int lfThumb03 = 38;
        public const int xTipBone88 = 39;
        public const int lfmetaCarpal05 = 40;
        public const int lfFingerPinky01 = 41;
        public const int lfFingerPinky02 = 42;
        public const int lfFingerPinky03 = 43;
        public const int xTiprtFingerne83 = 44;
        public const int lfmetaCarpal02 = 45;
        public const int lfFingerIndex01 = 46;
        public const int lfFingerIndex02 = 47;
        public const int lfFingerIndex03 = 48;
        public const int xTipBone89 = 49;
        public const int lfmetaCarpal04 = 50;
        public const int lfFingerRing01 = 51;
        public const int lfFingerRing02 = 52;
        public const int lfFingerRing03 = 53;
        public const int xTipBone90 = 54;
        public const int rtClavicle = 55;
        public const int rtShoulder = 56;
        public const int rtBicep = 57;
        public const int rtElbow = 58;
        public const int rtWrist = 59;
        public const int rtHand = 60;
        public const int rtmetaCarpal03 = 61;
        public const int rtFingerMiddle01 = 62;
        public const int rtFingerMiddle02 = 63;
        public const int rtFingerMiddle03 = 64;
        public const int xTipBone74 = 65;
        public const int rtmetaCarpal01 = 66;
        public const int rtThumb01 = 67;
        public const int rtThumb02 = 68;
        public const int rtThumb03 = 69;
        public const int xTipBone66 = 70;
        public const int rtmetaCarpal05 = 71;
        public const int rtFingerPinky01 = 72;
        public const int rtFingerPinky02 = 73;
        public const int rtFingerPinky03 = 74;
        public const int xTiprtFingerne82 = 75;
        public const int rtmetaCarpal02 = 76;
        public const int rtFingerIndex01 = 77;
        public const int rtFingerIndex02 = 78;
        public const int rtFingerIndex03 = 79;
        public const int xTipBone70 = 80;
        public const int rtmetaCarpal04 = 81;
        public const int rtFingerRing01 = 82;
        public const int rtFingerRing02 = 83;
        public const int rtFingerRing03 = 84;
        public const int xTipBone78 = 85;
        public const int zHead = 86;
    }

    public enum BoneRegion {
        Unknown,
        Root,
        Pelvis,
        Head,
        HandRight,
        HandLeft,
        ArmRight,
        ArmLeft,
        LegRight,
        LegLeft,
        FootRight,
        FootLeft,
        Spine,
    }
}