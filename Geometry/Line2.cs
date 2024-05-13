using System;

namespace JA.Geometry
{
    public readonly struct Line2 
    {
        public Line2(float x1, float y1, float x2, float y2)
        {
            this.From = new Vector2(x1, y1);
            this.To = new Vector2(x2, y2);
        }
        public Line2(Vector2 from, Vector2 to)
        {
            this.From = from;
            this.To = to;
        }

        public static Line2 Horizontal(float y)
            => new Line2(0, y, Console.WindowWidth, y);

        public static Line2 Vertical(float x)
            => new Line2(x, 0, x, Console.WindowHeight);

        public static Line2 Between(Point2 A, Point2 B)
            => new Line2(A.Position, B.Position);

        public static Line2 MainDiagonal() 
            => new Line2(
                Point2.TopLeft().Position, 
                Point2.BottomRight().Position);

        public static Line2 SecondDiagonal() 
            => new Line2(
                Point2.TopRight().Position, 
                Point2.BottomLeft().Position);

        public Vector2 From { get; }
        public Vector2 To { get; }

        public Vector2 Direction => (To - From).Normalized();
        public Vector2 Normal => new Vector2(-(To.Y - From.Y), (To.X - From.X)).Normalized();

        public static Line2 operator +(Line2 line, Vector2 delta)
            => new Line2(line.From + delta, line.To + delta);

        public Line2 Offset(float distance) => this + Normal * distance;

        public override string ToString() => $"Line:{From}->{To}";
    }
}
