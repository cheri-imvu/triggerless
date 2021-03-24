using System;
using System.Collections;
using System.Collections.Generic;

namespace Triggerless.XAFLib
{
    [Serializable]
    public struct Quaternion
    {
        private float _x;
        private float _y;
        private float _z;
        private float _w;

        public Quaternion(float x, float y, float z, float w) {
            _x = x; _y = y; _z = z; _w = w;
        }

        public Quaternion(double x, double y, double z, double w) {
            _x = (float)x;
            _y = (float)y;
            _z = (float)z;
            _w = (float)w;

        }

        public static Quaternion Zero => new Quaternion(0,0,0,0);
        public override bool Equals(object obj) {
            if (!(obj is Quaternion)) throw new ArgumentException("Only compare Quaternions");
            var other = (Quaternion) obj;
            return StringRep(_x) == StringRep(other._x) &&
                   StringRep(_y) == StringRep(other._y) &&
                   StringRep(_z) == StringRep(other._z) &&
                   StringRep(_w) == StringRep(other._w);
        }

        public static bool operator ==(Quaternion q1, Quaternion q2) {
            return q1.Equals(q2);
        }

        public static bool operator !=(Quaternion q1, Quaternion q2) {
            return !(q1 == q2);
        }


        private string StringRep(float f) => f.ToString("0.000000E+00");

        public float X { get { return _x; } set { _x = value; }}
        public float Y { get { return _y; } set { _y = value; }}
        public float Z { get { return _z; } set { _z = value; }}
        public float W { get { return _w; } set { _w = value; }}

        public static bool TryParse(string text, out Quaternion result) {
            result = new Quaternion();
            if (string.IsNullOrWhiteSpace(text)) return false;
            string[] pieces = text.Trim().Split(new[] {' '}, 4);
            if (pieces.Length != 4) return false;

            if (float.TryParse(pieces[0], out var floatValue)) {
                result.X = floatValue;
            } else {
                return false;
            }

            if (float.TryParse(pieces[1], out floatValue)) {
                result.Y = floatValue;
            } else {
                return false;
            }

            if (float.TryParse(pieces[2], out floatValue)) {
                result.Z = floatValue;
            } else {
                return false;
            }

            if (float.TryParse(pieces[3], out floatValue)) {
                result.W = floatValue;
            } else {
                return false;
            }

            return true;
        }

        public override string ToString() {
            return $"{{{X}, {Y}, {Z}, {W}}}";
        }

        public EulerAngles ToEulerAngles() {
            return new EulerAngles(this);
        }
    }

    public struct Vector {
        private float _x;
        private float _y;
        private float _z;

        public Vector(float x, float y, float z) {
            _x = x; _y = y; _z = z;
        }

        public float X {
            get => _x;
            set => _x = value;
        }
        public float Y { get { return _y; } set { _y = value; }}
        public float Z { get { return _z; } set { _z = value; }}

        public bool IsZero {
            get { return (X == 0 && Y == 0 & Z == 0) || (X == 1E+10 && Y == 1E+10 && Z == 1E+10); }
        }

        public static bool TryParse(string text, out Vector result) {
            result = new Vector();
            if (string.IsNullOrWhiteSpace(text)) return false;
            string[] pieces = text.Trim().Split(new[] {' '}, 3);
            if (pieces.Length != 3) return false;
            float floatValue;

            if (float.TryParse(pieces[0], out floatValue)) {
                result.X = floatValue;
            } else {
                return false;
            }

            if (float.TryParse(pieces[1], out floatValue)) {
                result.Y = floatValue;
            } else {
                return false;
            }

            if (float.TryParse(pieces[2], out floatValue)) {
                result.Z = floatValue;
            } else {
                return false;
            }

            return true;
        }

        public override string ToString() {
            return "<" + X + ", " + Y + ", " + Z + ">";
        }

        public static Vector Zero { get; } = new Vector(0,0,0);

        public void Normalize() {
		float mod = (float)Math.Sqrt( X*X + Y*Y + Z*Z );

		    if( mod != 0 && mod != 1) {
			    mod = 1 / mod; // mults are cheaper then divs
			    X *= mod;
			    Y *= mod;
			    Z *= mod;
		    }
	    }

        public void Reset(float newx = 0, float newy = 0, float newz = 0) {
		    X = newx; 
		    Y = newy; 
		    Z = newz; 
	    }

    }

    [Serializable]
    public struct EulerAngles {
        private float _yaw;
        public float Yaw {
            get { return _yaw; }
            set { _yaw = value; }
        }

        private float _pitch;
        public float Pitch {
            get { return _pitch; }
            set { _pitch = value; }
        }

        private float _roll;
        public float Roll {
            get { return _roll; }
            set { _roll = value; }
        }

        public EulerAngles(float yaw, float pitch, float roll) {
            _roll = roll;
            _pitch = pitch;
            _yaw = yaw;
        }

        public EulerAngles(Quaternion q) {
            _roll = (float)Math.Atan2(2 * (q.X * q.Y + q.Z * q.W), 1 - 2 * (q.Y * q.Y + q.Z * q.Z));
            _pitch = (float)Math.Asin(2 * (q.X * q.Z - q.W * q.Y));
            _yaw = (float)Math.Atan2(2 * (q.X * q.W + q.Z * q.Y), 1 - 2 * (q.W * q.W + q.Z * q.Z));
        }

        public static EulerAngles FromMatrixInDeg(Matrix3D m) {
            Vector v = Matrix3D.Matrix2Euler2(m);
            return new EulerAngles(v.X, v.Y, v.Z);
        }

        public static EulerAngles FromMatrixInRad(Matrix3D m) { return FromMatrixInDeg(m).DegToRad(); }

        public Quaternion ToQuaternion() {
            var q = new Quaternion {
                X = (float)
                    (Math.Cos(Roll / 2) * Math.Cos(Pitch / 2) * Math.Cos(Yaw / 2) +
                     Math.Sin(Roll / 2) * Math.Sin(Pitch / 2) * Math.Sin(Yaw / 2)),
                Y = (float)
                    (Math.Sin(Roll / 2) * Math.Cos(Pitch / 2) * Math.Cos(Yaw / 2) -
                     Math.Cos(Roll / 2) * Math.Sin(Pitch / 2) * Math.Sin(Yaw / 2)),
                Z = (float)
                    (Math.Cos(Roll / 2) * Math.Sin(Pitch / 2) * Math.Cos(Yaw / 2) +
                     Math.Sin(Roll / 2) * Math.Cos(Pitch / 2) * Math.Sin(Yaw / 2)),
                W = (float)
                    (Math.Cos(Roll / 2) * Math.Cos(Pitch / 2) * Math.Sin(Yaw / 2) -
                     Math.Sin(Roll / 2) * Math.Sin(Pitch / 2) * Math.Cos(Yaw / 2))
            };




            return q;
        }

        public EulerAngles DegToRad() {
            return  new EulerAngles((float)(Roll * Math.PI / 180), (float)(Pitch * Math.PI / 180), (float)(Yaw * Math.PI / 180));
        }

        public EulerAngles RadToDeg() {
            return new EulerAngles((float)(Roll * 180 / Math.PI), (float)(Pitch * 180 / Math.PI), (float)(Yaw * 180 / Math.PI));
        }

        public override string ToString() {
            return string.Format("{{Yaw: {2:0.000}  Pitch:{0:0.000} Roll:{1:0.000} }}", Yaw, Pitch, Roll);
        }

    }

    public class Mapper: IDictionary<string, string> {
        private Dictionary<string, string> _forwardMap;
        private Dictionary<string, string> _backMap;

        public Mapper() {
            _forwardMap = new Dictionary<string, string>();
            _backMap = new Dictionary<string, string>();
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _forwardMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _forwardMap.GetEnumerator();

        public void Add(KeyValuePair<string, string> item) {
            _forwardMap.Add(item.Key, item.Value);
            _backMap.Add(item.Value, item.Key);
        }

        public void Clear() {
            _forwardMap.Clear();
            _backMap.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item) {
            return _forwardMap.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item) {
            return _forwardMap.Remove(item.Key) || _backMap.Remove(item.Value);
        }

        public int Count => _forwardMap.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key) {
            return _forwardMap.ContainsKey(key);
        }

        public bool ContainsValue(string value) {
            return _backMap.ContainsKey(value);
        }

        public void Add(string key, string value) {
            _forwardMap.Add(key, value);
            _backMap.Add(value, key);
        }

        public bool Remove(string key) {
            string value = _forwardMap[key];
            _backMap.Remove(value);
            return _forwardMap.Remove(key);
        }

        public bool TryGetValue(string key, out string value) {
            if (!_forwardMap.ContainsKey(key)) {
                value = null;
                return false;
            }
            value = _forwardMap[key];
            return true;
        }

        public bool TryGetKey(string value, out string key) {
            if (!_backMap.ContainsKey(value)) {
                key = null;
                return false;
            }
            key = _backMap[value];
            return true;
        }

        public string this[string key]
        {
            get { return _forwardMap[key]; }
            set {
                if (_forwardMap.ContainsKey(key)) {
                    _backMap.Remove(_forwardMap[key]);
                    _forwardMap[key] = value;
                    _backMap[value] = key;
                } else {
                    if (_backMap.ContainsKey(value)) {
                        throw new ArgumentException($"Map already contains value '{value}'");
                    }
                    _forwardMap[key] = value;
                    _backMap[value] = key;
                }

            }
        }

        public ICollection<string> Keys => _forwardMap.Keys;
        public ICollection<string> Values => _forwardMap.Values;
    }
}