using System;

namespace JA.Geometry
{
    public readonly struct Point2 
    {
        public Point2(Vector2 position)
        {
            this.Position = position;
        }
        public Point2(float x, float y)
        {
            this.Position = new Vector2(x, y);
        }
        public static implicit operator Point2(Vector2 position) => new Point2(position);
        public Line2 LineTo(Point2 other) => new Line2(this.Position, other.Position);
        public static Point2 TopLeft() => new Point2(0, 0);
        public static Point2 BottomRight() => new Point2(Console.WindowWidth - 1, Console.WindowHeight - 1);
        public static Point2 TopRight() => new Point2(Console.WindowWidth - 1, 0);
        public static Point2 BottomLeft() => new Point2(0, Console.WindowHeight - 1);
        public static Point2 Center() => new Point2(Console.WindowWidth / 2, Console.WindowHeight / 2);

        public Vector2 Position { get; }        

        public static Point2 operator +(Point2 point, Vector2 delta)
            => new Point2(point.Position + delta);

        public static Vector2 operator -(Point2 to, Point2 from)
            => to.Position - from.Position;

        public override string ToString() => $"Point:{Position}";

    }
}
