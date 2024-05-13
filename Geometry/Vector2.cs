using System;

using static System.Math;

namespace JA.Geometry
{
    public readonly struct Vector2 
        : IEquatable<Vector2>
    {
        #region Factory
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 UnitX = new Vector2(1, 0);
        public static readonly Vector2 UnitY = new Vector2(0, 1);
        public Point2 Here() => new Point2(this);
        public Line2 To(Vector2 end) => new Line2(this, end);
        public static Vector2 Cartesian(float x, float y) => new Vector2(x, y);
        public static Vector2 Polar(float r, float θ) => new Vector2((float)(r * Cos(θ)), (float)(r * Sin(θ)));
        #endregion

        #region Properties
        public float X { get; }
        public float Y { get; }
        public float SumSquares => (float)(X * X + Y * Y);
        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);
        public Vector2 Normalized()
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
        public Vector2 Add(Vector2 other) => new Vector2(X + other.X, Y + other.Y);
        public Vector2 Scale(float factor) => new Vector2(factor * X, factor * Y);
        public float Dot(Vector2 other) => X * other.X + Y * other.Y;
        public float Cross(Vector2 other) => X * other.Y - Y * other.X;
        public Vector2 Cross(float other) => new Vector2(Y * other, -X * other);
        public Vector2 Rotate(float angle) => new Vector2(
            (float)(X * Cos(angle) - Y * Sin(angle)),
            (float)(X * Sin(angle) + Y * Cos(angle)));
        #endregion

        #region Operators
        public static Vector2 operator +(Vector2 a, Vector2 b) => a.Add(b);
        public static Vector2 operator -(Vector2 a) => a.Scale(-1);
        public static Vector2 operator -(Vector2 a, Vector2 b) => a.Add(-b);
        public static Vector2 operator *(float a, Vector2 b) => b.Scale(a);
        public static Vector2 operator *(Vector2 a, float b) => a.Scale(b);
        public static Vector2 operator /(Vector2 a, float b) => a.Scale(1 / b);
        public static float operator *(Vector2 a, Vector2 b) => a.Dot(b);
        public static float operator ^(Vector2 a, Vector2 b) => a.Cross(b);
        public static Vector2 operator ^(Vector2 a, float b) => a.Cross(b);
        public static Vector2 operator ^(float a, Vector2 b) => -b.Cross(a);
        #endregion

        #region IEquatable Members
        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Vector2)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2 other)
            {
                return Equals(other);
            }
            return false;
        }

        public static bool operator ==(Vector2 target, Vector2 other) { return target.Equals(other); }
        public static bool operator !=(Vector2 target, Vector2 other) { return !(target == other); }


        /// <summary>
        /// Checks for equality among <see cref="Vector2"/> classes
        /// </summary>
        /// <param name="other">The other <see cref="Vector2"/> to compare it to</param>
        /// <returns>True if equal</returns>
        public bool Equals(Vector2 other)
        {
            return X == other.X
                && Y == other.Y;
        }

        /// <summary>
        /// Calculates the hash code for the <see cref="Vector2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = -1817952719;
                hc = (-1521134295) * hc + X.GetHashCode();
                hc = (-1521134295) * hc + Y.GetHashCode();
                return hc;
            }
        }

        #endregion

        public override string ToString() => ToString("g");
        public string ToString(string formatting) => $"({X.ToString(formatting)},{Y.ToString(formatting)})";
    }
}
