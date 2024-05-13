using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using static System.Math;

namespace JA.Geometry
{

    public readonly struct Vector3 
        : IEquatable<Vector3>
    {
        #region Factory
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public static readonly Vector3 Zero =  new Vector3(0, 0, 0);
        public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
        public static Vector3 Cartesian(float x, float y, float z) => new Vector3(x, y, z);
        public static Vector3 Cylindrical(float r, float θ, float z) => new Vector3(
            (float)(r * Cos(θ)), 
            (float)(r * Sin(θ)), 
            z);

        #endregion

        #region Properties
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float SumSquares => (float)(X * X + Y * Y + Z * Z);
        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        public Vector3 Normalized()
        {
            var m2 = SumSquares;
            if (m2 > 0 && m2 != 1)
            {
                return this / (float)Sqrt(m2);
            }
            return this;
        }
        #endregion

        #region Algebra
        public Vector3 Add(Vector3 other) => new Vector3(X + other.X, Y + other.Y, Z + other.Z);
        public Vector3 Scale(float factor) => new Vector3(factor * X, factor * Y, factor * Z);
        public float Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;
        public Vector3 Cross(Vector3 other)
          => new Vector3(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);

        public Vector3 RotateX(float angle) 
            => new Vector3(
                X,
                (float)(Y * Cos(angle) - Z * Sin(angle)),
                (float)(Y * Sin(angle) + Z * Cos(angle)));
        public Vector3 RotateY(float angle)
            => new Vector3(
                (float)(X * Cos(angle) + Z * Sin(angle)),
                Y,
                (float)(-X * Sin(angle) + Z * Cos(angle)));
        public Vector3 RotateZ(float angle)
            => new Vector3(
                (float)(X * Cos(angle) - Y * Sin(angle)),
                (float)(X * Sin(angle) + Y * Cos(angle)),
                Z);

        public Vector3 RotateAbout(Vector3 axis, float angle)
        {
            axis = axis.Normalized();
            float k_1 = axis.X, k_2 = axis.Y, k_3 = axis.Z;
            float c = (float)Cos(angle), s = (float)Sin(angle), v = 1-c;

            return new Vector3(
                X + s * (Z * k_2 - Y * k_3) + v * (k_1 * (Y * k_2 + Z * k_3) - X * (k_2 * k_2 + k_3 * k_3)),
                Y + s * (X * k_3 - Z * k_1) + v * (X * k_1 * k_2 - Y * (k_1 * k_1 + k_3 * k_3) + Z * k_2 * k_3),
                Z + s * (Y * k_1 - X * k_2) + v * (X * k_1 * k_3 + Y * k_2 * k_3 - Z * (k_1 * k_1 + k_2 * k_2)));
        }
        #endregion

        #region Operators
        public static Vector3 operator +(Vector3 a, Vector3 b) => a.Add(b);
        public static Vector3 operator -(Vector3 a) => a.Scale(-1);
        public static Vector3 operator -(Vector3 a, Vector3 b) => a.Add(-b);
        public static Vector3 operator *(float a, Vector3 b) => b.Scale(a);
        public static Vector3 operator *(Vector3 a, float b) => a.Scale(b);
        public static Vector3 operator /(Vector3 a, float b) => a.Scale(1 / b);
        public static float operator *(Vector3 a, Vector3 b) => a.Dot(b);
        public static Vector3 operator ^(Vector3 a, Vector3 b) => a.Cross(b);
        #endregion


        #region IEquatable Members
        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Vector3)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector3 other)
            {
                return Equals(other);
            }
            return false;
        }

        public static bool operator ==(Vector3 target, Vector3 other) { return target.Equals(other); }
        public static bool operator !=(Vector3 target, Vector3 other) { return !(target == other); }


        /// <summary>
        /// Checks for equality among <see cref="Vector3"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Vector3"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Vector3 other)
        {
            return X == other.X
                && Y == other.Y
                && Z == other.Z;
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Vector3"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = -1817952719;
                hc = (-1521134295) * hc + X.GetHashCode();
                hc = (-1521134295) * hc + Y.GetHashCode();
                hc = (-1521134295) * hc + Z.GetHashCode();
                return hc;
            }
        }

        #endregion
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => $"({X.ToString(formatting)},{Y.ToString(formatting)},{Z.ToString(formatting)})";
    }
}
