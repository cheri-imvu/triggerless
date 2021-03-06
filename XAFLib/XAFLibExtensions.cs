using System;
using System.Text;
using System.Xml;

namespace XAFLib
{
    public static class XAFLibExtensions {
        public static bool IsNearlyEqual(this float x, float y) {
            if (x == 0 && y == 0) return true;
            float a;
            float b;
            if (x == 0) {
                b = y;
                a = x;
            } else {
                a = y;
                b = x;
            }
            float diff = Math.Abs((b - a) / b);
            return (diff < 0.0001);
        }


        public static bool? GetAttributeBoolean(this XmlElement el, string attrName) {
            if (!el.HasAttribute(attrName)) return null;
            bool? result = null;
            int intValue;
            string attrValue = el.GetAttribute(attrName);
            if (int.TryParse(attrValue, out intValue)) {
                if (intValue == 0) result = false;
                if (intValue == 1) result = true;
            }
            return result;
        }

        public static float ParseFloat(this string value) {
            float f;
            return float.TryParse(value, out f) ? f : 0;
        }

        public static int ParseInt32(this string value) {
            int intValue;
            return int.TryParse(value, out intValue) ? intValue : 0;
        }

        public static BoneRegion ParseBoneRegion(this string value) {
            BoneRegion brValue;
            return Enum.TryParse(value, out brValue) ? brValue : BoneRegion.Unknown;
        }

        public static BoneTags ParseBoneTags(this string value) {
            BoneTags btValue;
            return BoneTags.TryParse(value, out btValue) ? btValue : new BoneTags();
        }

        public static Vector ParseVector(this XmlNode value) {
            return (value as XmlElement) == null ? new Vector() : value.InnerText.ParseVector();
        }

        public static Quaternion ParseQuaternion(this XmlNode value) {
            if ((value as XmlElement) == null) return new Quaternion();
            Quaternion q;
            return Quaternion.TryParse(value.InnerText, out q) ? q : new Quaternion();
        }

        public static int ParseIntOrDefault(this XmlNode value, int defaultValue) {
            XmlElement el = (value as XmlElement);
            return el == null ? defaultValue : el.InnerText.ParseIntOrDefault(defaultValue);
        }

        public static Vector ParseVector(this string value) {
            Vector v;
            return Vector.TryParse(value, out v) ? v : new Vector();
        }

        public static int ParseIntOrDefault(this string value, int defaultValue) {
            int intValue;
            return int.TryParse(value, out intValue) ? intValue : defaultValue;
        }

        public static void AppendUnixLineChar(this StringBuilder value) {
            value.Append((char)10);
        }

        public static void AppendUnixLine(this StringBuilder value, string line) {
            value.Append(line);
            value.AppendUnixLineChar();
        }

        
        public static float Nudge(this float d) {
            if (Math.Abs(d) < 0.00001) return 0;
            return 1 - Math.Abs(d) < 0.00001 ? 1 : d;
        }
    }
}
