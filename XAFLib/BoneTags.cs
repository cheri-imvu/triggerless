using System;

namespace Triggerless.XAFLib
{
    [Flags]
    public enum BoneTagValue : long {
        None = 0,
        Root = 0x1,
        Pelvis = 0x2,
        Left = 0x4,
        Right = 0x8,
        Hip = 0x10,
        Thigh = 0x20,
        Calf = 0x40,
        Foot = 0x80,
        Toes = 0x100,
        Spine = 0x200,
        Back = 0x400,
        Neck = 0x800,
        Head = 0x1000,
        Arm = 0x2000,
        Clavicle = 0x4000,
        Shoulder = 0x8000,
        Bicep = 0x10000,
        Elbow = 0x20000,
        Wrist = 0x40000,
        Hand = 0x80000,
        Finger = 0x100000,
        Metacarpal = 0x200000,
        Bone1 = 0x400000,
        Bone2 = 0x800000,
        Bone3 = 0x1000000,
        Tip = 0x2000000,
        Middle = 0x4000000,
        Thumb = 0x8000000,
        Pinky = 0x10000000,
        Index = 0x20000000,
        Ring = 0x40000000,
        Leg = 0x80000000,
    }

    public struct BoneTags {
        private BoneTagValue _value;

        public bool Contains(BoneTagValue test) {
            return (_value & test) == test;
        }

        public void Include(BoneTagValue add) {
            _value |= add;
        }

        public void Exclude(BoneTagValue remove) {
            _value &= ~remove;
        }

        public static bool TryParse(string input, out BoneTags value) {
            value = new BoneTags();

            if (string.IsNullOrWhiteSpace(input)) return false;
            string[] pieces = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (pieces.Length == 0) return false;
            bool foundOne = false;

            foreach (string piece in pieces) {
                BoneTagValue btValue;
                if (Enum.TryParse(piece.Trim(), out btValue)) {
                    value.Include(btValue);
                    foundOne = true;
                }
            }

            return foundOne;
        }

        public override string ToString() {
            Array values = Enum.GetValues(typeof (BoneTagValue));
            string result = "{";
            string delimiter = string.Empty;
            bool foundOne = false;
            foreach (BoneTagValue value in values) {
                if (value == BoneTagValue.None) continue;
                if (!Contains(value)) continue;
                foundOne = true;
                result += delimiter + value;
                delimiter = " ";
            }
            if (!foundOne) result += "None";
            result += "}";
            return result;
        }
    }
}