using System;

namespace XAFLib {
    public class Matrix3D {
        private const float TO_DEGREES = (float) (180 / Math.PI);
        private const float TO_RADIANS = (float) (Math.PI / 180);
        private static readonly Matrix3D _temp = Identity;
        private static Vector _n3Di = Vector.Zero;
        private static Vector _n3Dj = Vector.Zero;
        private static Vector _n3Dk = Vector.Zero;

        public Matrix3D() { N11 = N22 = N33 = N44 = 1; }

        public Matrix3D(params float[] args) : this() {
            if (args.Length >= 12) {
                N11 = args[0];
                N12 = args[1];
                N13 = args[2];
                N14 = args[3];
                N21 = args[4];
                N22 = args[5];
                N23 = args[6];
                N24 = args[7];
                N31 = args[8];
                N32 = args[9];
                N33 = args[10];
                N34 = args[11];
                N41 = N42 = N43 = 0;
                N44 = 1;
                if (args.Length == 16) {
                    N41 = args[12];
                    N42 = args[13];
                    N43 = args[14];
                    N44 = args[15];
                }
            }
        }

        public float N11 { get; set; }
        public float N12 { get; set; }
        public float N13 { get; set; }
        public float N14 { get; set; }
        public float N21 { get; set; }
        public float N22 { get; set; }
        public float N23 { get; set; }
        public float N24 { get; set; }
        public float N31 { get; set; }
        public float N32 { get; set; }
        public float N33 { get; set; }
        public float N34 { get; set; }
        public float N41 { get; set; }
        public float N42 { get; set; }
        public float N43 { get; set; }
        public float N44 { get; set; }

        public static Matrix3D Identity {
            get {
                return new Matrix3D(
                    1, 0, 0, 0, 
                    0, 1, 0, 0, 
                    0, 0, 1, 0, 
                    0, 0, 0, 1);
            }
        }

        public float Determinant {
            get {
                return (N11 * N22 - N21 * N12) * N33 -
                       (N11 * N32 - N31 * N12) * N23 +
                       (N21 * N32 - N31 * N22) * N13;
            }
        }

        // sets the properties of the Matrix without creating a new one. 
        public void Reset() { Reset(new float[] {}); }

        public void Reset(float[] args) {
            if (args == null || args.Length < 12) {
                N11 = N22 = N33 = N44 = 1;
                N12 = N13 = N14 = N21 = N23 = N24 = N31 = N32 = N34 = N41 = N42 = N43 = 0;
            } else {
                N11 = args[0];
                N12 = args[1];
                N13 = args[2];
                N14 = args[3];
                N21 = args[4];
                N22 = args[5];
                N23 = args[6];
                N24 = args[7];
                N31 = args[8];
                N32 = args[9];
                N33 = args[10];
                N34 = args[11];

                if (args.Length == 16) {
                    N41 = args[12];
                    N42 = args[13];
                    N43 = args[14];
                    N44 = args[15];
                } else {
                    N41 = N42 = N43 = 0;
                    N44 = 1;
                }
            }
        }

        public override string ToString() {
            string s = "";

            s += Math.Round(N11 * 1000) / 1000 + "\t\t" + Math.Round(N12 * 1000) / 1000 + "\t\t" +
                 Math.Round(N13 * 1000) / 1000 + "\t\t" + Math.Round(N14 * 1000) / 1000 + "\n";
            s += Math.Round(N21 * 1000) / 1000 + "\t\t" + Math.Round(N22 * 1000) / 1000 + "\t\t" +
                 Math.Round(N23 * 1000) / 1000 + "\t\t" + Math.Round(N24 * 1000) / 1000 + "\n";
            s += Math.Round(N31 * 1000) / 1000 + "\t\t" + Math.Round(N32 * 1000) / 1000 + "\t\t" +
                 Math.Round(N33 * 1000) / 1000 + "\t\t" + Math.Round(N34 * 1000) / 1000 + "\n";
            s += Math.Round(N41 * 1000) / 1000 + "\t\t" + Math.Round(N42 * 1000) / 1000 + "\t\t" +
                 Math.Round(N43 * 1000) / 1000 + "\t\t" + Math.Round(N44 * 1000) / 1000 + "\n";

            return s;
        }

        public void CalculateMultiply(Matrix3D a, Matrix3D b) {
            float a11 = a.N11;
            float b11 = b.N11;
            float a21 = a.N21;
            float b21 = b.N21;
            float a31 = a.N31;
            float b31 = b.N31;
            float a12 = a.N12;
            float b12 = b.N12;
            float a22 = a.N22;
            float b22 = b.N22;
            float a32 = a.N32;
            float b32 = b.N32;
            float a13 = a.N13;
            float b13 = b.N13;
            float a23 = a.N23;
            float b23 = b.N23;
            float a33 = a.N33;
            float b33 = b.N33;
            float a14 = a.N14;
            float b14 = b.N14;
            float a24 = a.N24;
            float b24 = b.N24;
            float a34 = a.N34;
            float b34 = b.N34;

            N11 = a11 * b11 + a12 * b21 + a13 * b31;
            N12 = a11 * b12 + a12 * b22 + a13 * b32;
            N13 = a11 * b13 + a12 * b23 + a13 * b33;
            N14 = a11 * b14 + a12 * b24 + a13 * b34 + a14;

            N21 = a21 * b11 + a22 * b21 + a23 * b31;
            N22 = a21 * b12 + a22 * b22 + a23 * b32;
            N23 = a21 * b13 + a22 * b23 + a23 * b33;
            N24 = a21 * b14 + a22 * b24 + a23 * b34 + a24;

            N31 = a31 * b11 + a32 * b21 + a33 * b31;
            N32 = a31 * b12 + a32 * b22 + a33 * b32;
            N33 = a31 * b13 + a32 * b23 + a33 * b33;
            N34 = a31 * b14 + a32 * b24 + a33 * b34 + a34;
        }

        public static Matrix3D Multiply(Matrix3D a, Matrix3D b) {
            //trace("matrix.multiply"); 
            var m = new Matrix3D();
            m.CalculateMultiply(a, b);
            return m;
        }

        public void CalculateMultiply3X3(Matrix3D a, Matrix3D b) {
            float a11 = a.N11;
            float b11 = b.N11;
            float a21 = a.N21;
            float b21 = b.N21;
            float a31 = a.N31;
            float b31 = b.N31;
            float a12 = a.N12;
            float b12 = b.N12;
            float a22 = a.N22;
            float b22 = b.N22;
            float a32 = a.N32;
            float b32 = b.N32;
            float a13 = a.N13;
            float b13 = b.N13;
            float a23 = a.N23;
            float b23 = b.N23;
            float a33 = a.N33;
            float b33 = b.N33;

            N11 = a11 * b11 + a12 * b21 + a13 * b31;
            N12 = a11 * b12 + a12 * b22 + a13 * b32;
            N13 = a11 * b13 + a12 * b23 + a13 * b33;

            N21 = a21 * b11 + a22 * b21 + a23 * b31;
            N22 = a21 * b12 + a22 * b22 + a23 * b32;
            N23 = a21 * b13 + a22 * b23 + a23 * b33;

            N31 = a31 * b11 + a32 * b21 + a33 * b31;
            N32 = a31 * b12 + a32 * b22 + a33 * b32;
            N33 = a31 * b13 + a32 * b23 + a33 * b33;
        }

        public void CalculateMultiply4X4(Matrix3D a, Matrix3D b) {
            float a11 = a.N11;
            float b11 = b.N11;
            float a21 = a.N21;
            float b21 = b.N21;
            float a31 = a.N31;
            float b31 = b.N31;
            float a41 = a.N41; //float b41 = b.n41;

            float a12 = a.N12;
            float b12 = b.N12;
            float a22 = a.N22;
            float b22 = b.N22;
            float a32 = a.N32;
            float b32 = b.N32;
            float a42 = a.N42; //float b42 = b.n42;

            float a13 = a.N13;
            float b13 = b.N13;
            float a23 = a.N23;
            float b23 = b.N23;
            float a33 = a.N33;
            float b33 = b.N33;
            float a43 = a.N43; //float b43 = b.n43;

            float a14 = a.N14;
            float b14 = b.N14;
            float a24 = a.N24;
            float b24 = b.N24;
            float a34 = a.N34;
            float b34 = b.N34;
            float a44 = a.N44; //float b44 = b.n44;

            N11 = a11 * b11 + a12 * b21 + a13 * b31;
            N12 = a11 * b12 + a12 * b22 + a13 * b32;
            N13 = a11 * b13 + a12 * b23 + a13 * b33;
            N14 = a11 * b14 + a12 * b24 + a13 * b34 + a14;

            N21 = a21 * b11 + a22 * b21 + a23 * b31;
            N22 = a21 * b12 + a22 * b22 + a23 * b32;
            N23 = a21 * b13 + a22 * b23 + a23 * b33;
            N24 = a21 * b14 + a22 * b24 + a23 * b34 + a24;

            N31 = a31 * b11 + a32 * b21 + a33 * b31;
            N32 = a31 * b12 + a32 * b22 + a33 * b32;
            N33 = a31 * b13 + a32 * b23 + a33 * b33;
            N34 = a31 * b14 + a32 * b24 + a33 * b34 + a34;

            N41 = a41 * b11 + a42 * b21 + a43 * b31;
            N42 = a41 * b12 + a42 * b22 + a43 * b32;
            N43 = a41 * b13 + a42 * b23 + a43 * b33;
            N44 = a41 * b14 + a42 * b24 + a43 * b34 + a44;
        }

        public void CalculateSkewSymmetric(Vector a) {
            N11 = 0;
            N12 = -a.Z;
            N13 = a.Y;
            N21 = a.Z;
            N22 = 0;
            N23 = -a.X;
            N31 = -a.Y;
            N32 = a.X;
            N33 = 0;
        }

        public static Matrix3D Multiply3X3(Matrix3D a, Matrix3D b) {
            //trace("Multiply3x3"); 
            var m = new Matrix3D();
            m.CalculateMultiply3X3(a, b);
            return m;
        }

        public void CalculateAdd(Matrix3D a, Matrix3D b) {
            N11 = a.N11 + b.N11;
            N12 = a.N12 + b.N12;
            N13 = a.N13 + b.N13;
            N14 = a.N14 + b.N14;

            N21 = a.N21 + b.N21;
            N22 = a.N22 + b.N22;
            N23 = a.N23 + b.N23;
            N24 = a.N24 + b.N24;

            N31 = a.N31 + b.N31;
            N32 = a.N32 + b.N32;
            N33 = a.N33 + b.N33;
            N34 = a.N34 + b.N34;
        }

        public static Matrix3D Add(Matrix3D a, Matrix3D b) {
            //trace("matrix.add"); 
            var m = new Matrix3D();

            m.CalculateAdd(a, b);

            return m;
        }

        public void CalculateInverse(Matrix3D m) {
            float d = m.Determinant;

            if (Math.Abs(d) > 0.001) {
                d = 1 / d;

                float m11 = m.N11;
                float m21 = m.N21;
                float m31 = m.N31;
                float m12 = m.N12;
                float m22 = m.N22;
                float m32 = m.N32;
                float m13 = m.N13;
                float m23 = m.N23;
                float m33 = m.N33;
                float m14 = m.N14;
                float m24 = m.N24;
                float m34 = m.N34;

                N11 = d * (m22 * m33 - m32 * m23);
                N12 = -d * (m12 * m33 - m32 * m13);
                N13 = d * (m12 * m23 - m22 * m13);
                N14 = -d *
                      (m12 * (m23 * m34 - m33 * m24) - m22 * (m13 * m34 - m33 * m14) + m32 * (m13 * m24 - m23 * m14));

                N21 = -d * (m21 * m33 - m31 * m23);
                N22 = d * (m11 * m33 - m31 * m13);
                N23 = -d * (m11 * m23 - m21 * m13);
                N24 = d *
                      (m11 * (m23 * m34 - m33 * m24) - m21 * (m13 * m34 - m33 * m14) + m31 * (m13 * m24 - m23 * m14));

                N31 = d * (m21 * m32 - m31 * m22);
                N32 = -d * (m11 * m32 - m31 * m12);
                N33 = d * (m11 * m22 - m21 * m12);
                N34 = -d *
                      (m11 * (m22 * m34 - m32 * m24) - m21 * (m12 * m34 - m32 * m14) + m31 * (m12 * m24 - m22 * m14));
            }
        }

        // ReSharper disable LocalVariableHidesMember
        public void CalculateTranspose() {
            float n11 = N11;
            float n12 = N21;
            float n13 = N31;
            float n14 = N41;

            float n21 = N12;
            float n22 = N22;
            float n23 = N32;
            float n24 = N42;

            float n31 = N13;
            float n32 = N23;
            float n33 = N33;
            float n34 = N43;

            float n41 = N14;
            float n42 = N24;
            float n43 = N34;
            float n44 = N44;

            N11 = n11;
            N12 = n12;
            N13 = n13;
            N14 = n14;
            N21 = n21;
            N22 = n22;
            N23 = n23;
            N24 = n24;
            N31 = n31;
            N32 = n32;
            N33 = n33;
            N34 = n34;
            N41 = n41;
            N42 = n42;
            N43 = n43;
            N44 = n44;
        }

        // ReSharper restore LocalVariableHidesMember

        public static Matrix3D Inverse(Matrix3D m) {
            var inv = new Matrix3D();
            inv.CalculateInverse(m);
            return inv;
        }

        public void Invert() {
            _temp.Copy(this);
            CalculateInverse(_temp);
        }

        public Matrix3D Copy(Matrix3D m) {
            N11 = m.N11;
            N12 = m.N12;
            N13 = m.N13;
            N14 = m.N14;

            N21 = m.N21;
            N22 = m.N22;
            N23 = m.N23;
            N24 = m.N24;

            N31 = m.N31;
            N32 = m.N32;
            N33 = m.N33;
            N34 = m.N34;

            return this;
        }

        public Matrix3D Copy3X3(Matrix3D m) {
            N11 = m.N11;
            N12 = m.N12;
            N13 = m.N13;
            N21 = m.N21;
            N22 = m.N22;
            N23 = m.N23;
            N31 = m.N31;
            N32 = m.N32;
            N33 = m.N33;

            return this;
        }

        public static Matrix3D Clone(Matrix3D m) {
            //trace("matrix3D.clone"); 
            return new Matrix3D (
                m.N11, m.N12, m.N13, m.N14, 
                m.N21, m.N22, m.N23, m.N24, 
                m.N31, m.N32, m.N33, m.N34);
        }

        public static void MultiplyVector(Matrix3D m, Vector v) {
            float vx = v.X;
            float vy = v.Y;
            float vz = v.Z;

            v.X = vx * m.N11 + vy * m.N12 + vz * m.N13 + m.N14;
            v.Y = vx * m.N21 + vy * m.N22 + vz * m.N23 + m.N24;
            v.Z = vx * m.N31 + vy * m.N32 + vz * m.N33 + m.N34;
        }

        public static void MultiplyVector3X3(Matrix3D m, Vector v) {
            float vx = v.X;
            float vy = v.Y;
            float vz = v.Z;

            v.X = vx * m.N11 + vy * m.N12 + vz * m.N13;
            v.Y = vx * m.N21 + vy * m.N22 + vz * m.N23;
            v.Z = vx * m.N31 + vy * m.N32 + vz * m.N33;
        }

        public static void MultiplyVector4X4(Matrix3D m, Vector v) {
            float vx = v.X;
            float vy = v.Y;
            float vz = v.Z;
            var vw = (float) (1.0 / (vx * m.N41 + vy * m.N42 + vz * m.N43 + m.N44));

            v.X = vx * m.N11 + vy * m.N12 + vz * m.N13 + m.N14;
            v.Y = vx * m.N21 + vy * m.N22 + vz * m.N23 + m.N24;
            v.Z = vx * m.N31 + vy * m.N32 + vz * m.N33 + m.N34;

            v.X *= vw;
            v.Y *= vw;
            v.Z *= vw;
        }

        public static void RotateAxis(Matrix3D m, Vector v) {
            float vx = v.X;
            float vy = v.Y;
            float vz = v.Z;

            v.X = vx * m.N11 + vy * m.N12 + vz * m.N13;
            v.Y = vx * m.N21 + vy * m.N22 + vz * m.N23;
            v.Z = vx * m.N31 + vy * m.N32 + vz * m.N33;

            v.Normalize();
        }

        public static void ProjectVector(Matrix3D m, Vector v) {
            float c = 1 / (v.X * m.N41 + v.Y * m.N42 + v.Z * m.N43 + 1);
            MultiplyVector(m, v);
            v.X = v.X * c;
            v.Y = v.Y * c;
            v.Z = 0;
        }

        public static Vector Matrix2EulerOld(Matrix3D m) {
            var angle = new Vector();

            var d = (float) (-Math.Asin(Math.Max(-1, Math.Min(1, m.N13)))); // Calculate Y-axis angle
            var c = (float) Math.Cos(d);
            angle.Y = d * TO_DEGREES;

            float trX, trY;

            if (Math.Abs(c) > 0.005) {
                // Gimball lock?
                trX = m.N33 / c; // No, so get X-axis angle
                trY = -m.N23 / c;
                angle.X = (float) (Math.Atan2(trY, trX) * TO_DEGREES);
                trX = m.N11 / c; // Get Z-axis angle
                trY = -m.N12 / c;
                angle.Z = (float) (Math.Atan2(trY, trX) * TO_DEGREES);
            } else {
                // Gimball lock has occurred
                angle.X = 0; // Set X-axis angle to zero
                trX = m.N22; // And calculate Z-axis angle
                trY = m.N21;
                angle.Z = (float) (Math.Atan2(trY, trX) * TO_DEGREES);
            }
            // TODO: Clamp all angles to range
            return angle;
        }

        public Vector ToEuler2() {
            return Matrix2Euler2(this);
        }
        public static Vector Matrix2Euler2(Matrix3D t) { return Matrix2Euler2(t, Vector.Zero, Vector.Zero); }
        public static Vector Matrix2Euler2(Matrix3D t, Vector rot) { return Matrix2Euler2(t, rot, Vector.Zero); }

        public static Vector Matrix2Euler2(Matrix3D t, Vector rot, Vector scale) {
            // Normalize the local x, y and z axes to remove scaling.
            _n3Di.Reset(t.N11, t.N21, t.N31);
            _n3Dj.Reset(t.N12, t.N22, t.N32);
            _n3Dk.Reset(t.N13, t.N23, t.N33);

            _n3Di.Normalize();
            _n3Dj.Normalize();
            _n3Dk.Normalize();

            _temp.Reset(new[] {
                                 _n3Di.X, _n3Dj.X, _n3Dk.X, 0,
                                 _n3Di.Y, _n3Dj.Y, _n3Dk.Y, 0,
                                 _n3Di.Z, _n3Dj.Z, _n3Dk.Z, 0
                             });

            Matrix3D m = _temp;

            // Extract the first angle, rot.x
            rot.X = (float) Math.Atan2(m.N23, m.N33); // rot.x = Math<T>::atan2 (M[1][2], M[2][2]);

            // Remove the rot.x rotation from M, so that the remaining
            // rotation, N, is only around two axes, and gimbal lock
            // cannot occur.
            Matrix3D rx = RotationX(-rot.X);
            Matrix3D n = Multiply(rx, m);

            // Extract the other two angles, rot.y and rot.z, from N.
            var cy = (float) Math.Sqrt(n.N11 * n.N11 + n.N21 * n.N21);
                // T cy = Math<T>::sqrt (N[0][0]*N[0][0] + N[0][1]*N[0][1]);
            rot.Y = (float) Math.Atan2(-n.N31, cy); // rot.y = Math<T>::atan2 (-N[0][2], cy);
            rot.Z = (float) Math.Atan2(-n.N12, n.N11); //rot.z = Math<T>::atan2 (-N[1][0], N[1][1]);

            // Fix angles
            const float PI = (float) Math.PI;
            if (rot.X == PI) {
                if (rot.Y > 0)
                    rot.Y -= PI;
                else
                    rot.Y += PI;

                rot.X = 0;
                rot.Z += PI;
            }

            // Convert to degrees if needed 
            // Shouldn't this have a check for Papervision3D.useDEGREES? 

            rot.X *= TO_DEGREES;
            rot.Y *= TO_DEGREES;
            rot.Z *= TO_DEGREES;

            return rot;
        }

        public static Matrix3D Euler2Matrix(Vector deg) {
            //trace("euler2matrix"); 

            _temp.Reset();
            Matrix3D m = _temp;

            float ax = deg.X * TO_RADIANS;
            float ay = deg.Y * TO_RADIANS;
            float az = deg.Z * TO_RADIANS;

            var a = (float) Math.Cos(ax);
            var b = (float) Math.Sin(ax);
            var c = (float) Math.Cos(ay);
            var d = (float) Math.Sin(ay);
            var e = (float) Math.Cos(az);
            var f = (float) Math.Sin(az);

            float ad = a * d;
            float bd = b * d;

            m.N11 = c * e;
            m.N12 = -c * f;
            m.N13 = d;
            m.N21 = bd * e + a * f;
            m.N22 = -bd * f + a * e;
            m.N23 = -b * c;
            m.N31 = -ad * e + b * f;
            m.N32 = ad * f + b * e;
            m.N33 = a * c;

            return m;
        }

        public static Matrix3D RotationX(float rad) {
            //trace("rotationX"); 
            Matrix3D m = Identity;
            var c = (float) Math.Cos(rad);
            var s = (float) Math.Sin(rad);

            m.N22 = c;
            m.N23 = -s;
            m.N32 = s;
            m.N33 = c;

            return m;
        }

        public static Matrix3D RotationY(float rad) {
            //trace("rotationY"); 
            Matrix3D m = Identity;
            var c = (float) Math.Cos(rad);
            var s = (float) Math.Sin(rad);

            m.N11 = c;
            m.N13 = -s;
            m.N31 = s;
            m.N33 = c;

            return m;
        }

        public static Matrix3D RotationZ(float rad) {
            //trace("rotationZ"); 
            Matrix3D m = Identity;
            var c = (float) Math.Cos(rad);
            var s = (float) Math.Sin(rad);

            m.N11 = c;
            m.N12 = -s;
            m.N21 = s;
            m.N22 = c;

            return m;
        }

        public static Matrix3D RotationMatrix(float x, float y, float z, float rad, Matrix3D targetmatrix = null) {
            Matrix3D m = targetmatrix ?? Identity;

            var nCos = (float) Math.Cos(rad);
            var nSin = (float) Math.Sin(rad);
            float scos = 1 - nCos;

            float sxy = x * y * scos;
            float syz = y * z * scos;
            float sxz = x * z * scos;
            float sz = nSin * z;
            float sy = nSin * y;
            float sx = nSin * x;

            m.N11 = nCos + x * x * scos;
            m.N12 = -sz + sxy;
            m.N13 = sy + sxz;
            m.N14 = 0;

            m.N21 = sz + sxy;
            m.N22 = nCos + y * y * scos;
            m.N23 = -sx + syz;
            m.N24 = 0;

            m.N31 = -sy + sxz;
            m.N32 = sx + syz;
            m.N33 = nCos + z * z * scos;
            m.N34 = 0;

            return m;
        }

        public static Matrix3D RotationMatrixWithReference(Vector axis, float rad, Vector reference) {
            Matrix3D m = TranslationMatrix(reference.X, -reference.Y, reference.Z);
            m.CalculateMultiply(m, RotationMatrix(axis.X, axis.Y, axis.Z, rad));
            m.CalculateMultiply(m, TranslationMatrix(-reference.X, reference.Y, -reference.Z));
            return m;
        }

        public static Matrix3D TranslationMatrix(float x, float y, float z) {
            //trace("translation matrix"); 
            Matrix3D m = Identity;
            m.N14 = x;
            m.N24 = y;
            m.N34 = z;
            return m;
        }

        public static Matrix3D ScaleMatrix(float x, float y, float z) {
            //trace("scalematrix"); 
            Matrix3D m = Identity;
            m.N11 = x;
            m.N22 = y;
            m.N33 = z;
            return m;
        }

        // _________________________________________________________________________________ QUATERNIONS

        public static float MagnitudeQuaternion(Quaternion q) { return (float) Math.Sqrt(q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z); }

        public static Quaternion NormalizeQuaternion(Quaternion q) {
            float mag = MagnitudeQuaternion(q);
            q.X /= mag;
            q.Y /= mag;
            q.Z /= mag;
            q.W /= mag;
            return q;
        }

        public static Quaternion Axis2Quaternion(float x, float y, float z, float angle) {
            var sin = (float) Math.Sin(angle / 2);
            var cos = (float) Math.Cos(angle / 2);

            var q = new Quaternion {X = x * sin, Y = y * sin, Z = z * sin, W = cos};

            return NormalizeQuaternion(q);
        }

        public static Quaternion Euler2Quaternion(float ax, float ay, float az) { return Euler2Quaternion(ax, ay, az, new Quaternion()); }

        public static Quaternion Euler2Quaternion(float ax, float ay, float az, Quaternion targetquat) {
            var fSinPitch = (float) Math.Sin(ax * 0.5);
            var fCosPitch = (float) Math.Cos(ax * 0.5);
            var fSinYaw = (float) Math.Sin(ay * 0.5);
            var fCosYaw = (float) Math.Cos(ay * 0.5);
            var fSinRoll = (float) Math.Sin(az * 0.5);
            var fCosRoll = (float) Math.Cos(az * 0.5);
            float fCosPitchCosYaw = fCosPitch * fCosYaw;
            float fSinPitchSinYaw = fSinPitch * fSinYaw;

            Quaternion q = targetquat;

            q.X = fSinRoll * fCosPitchCosYaw - fCosRoll * fSinPitchSinYaw;
            q.Y = fCosRoll * fSinPitch * fCosYaw + fSinRoll * fCosPitch * fSinYaw;
            q.Z = fCosRoll * fCosPitch * fSinYaw - fSinRoll * fSinPitch * fCosYaw;
            q.W = fCosRoll * fCosPitchCosYaw + fSinRoll * fSinPitchSinYaw;

            return q;
        }

        public static Matrix3D Quaternion2Matrix(Quaternion q) { return Quaternion2Matrix(q.X, q.Y, q.Z, q.W); }
        public static Matrix3D Quaternion2Matrix(Quaternion q, Matrix3D targetmatrix) { return Quaternion2Matrix(q.X, q.Y, q.Z, q.W, targetmatrix); }
        public static Matrix3D Quaternion2Matrix(float x, float y, float z, float w) { return Quaternion2Matrix(x, y, z, w, null); }

        public static Matrix3D Quaternion2Matrix(float x, float y, float z, float w, Matrix3D targetmatrix) {
            float xx = x * x;
            float xy = x * y;
            float xz = x * z;
            float xw = x * w;

            float yy = y * y;
            float yz = y * z;
            float yw = y * w;

            float zz = z * z;
            float zw = z * w;

            Matrix3D m = targetmatrix ?? Identity;
            m.N11 = 1 - 2 * (yy + zz);
            m.N12 = 2 * (xy - zw);
            m.N13 = 2 * (xz + yw);

            m.N21 = 2 * (xy + zw);
            m.N22 = 1 - 2 * (xx + zz);
            m.N23 = 2 * (yz - xw);

            m.N31 = 2 * (xz - yw);
            m.N32 = 2 * (yz + xw);
            m.N33 = 1 - 2 * (xx + yy);

            return m;
        }

        public Quaternion ToQuaternion() {
            return Matrix2Quaternion(this);
        }
        public static Quaternion Matrix2Quaternion(Matrix3D m) {
            Vector v = Matrix2Euler2(m);
            return Euler2Quaternion(v.X, v.Y, v.Z);
        }

        public static Quaternion MultiplyQuaternion(Quaternion a, Quaternion b) {
            float ax = a.X;
            float ay = a.Y;
            float az = a.Z;
            float aw = a.W;
            float bx = b.X;
            float by = b.Y;
            float bz = b.Z;
            float bw = b.W;

            var q = new Quaternion {
                X = aw * bx + ax * bw + ay * bz - az * by,
                Y = aw * by + ay * bw + az * bx - ax * bz,
                Z = aw * bz + az * bw + ax * by - ay * bx,
                W = aw * bw - ax * bx - ay * by - az * bz
            };

            return q;
        }
    }
}