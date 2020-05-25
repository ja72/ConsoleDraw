using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{

    public struct Point2 
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

    public struct Line2 
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

    public struct Triangle2
    {
        public Triangle2(Vector2 a, Vector2 b, Vector2 c)
        {
            this.A = a;
            this.B = b;
            this.C = c;

            this.Edges = new Line2[] {
                new Line2(a, b),
                new Line2(b, c),
                new Line2(c, a),
            };
        }
        public Vector2 A { get; }
        public Vector2 B { get; }
        public Vector2 C { get; }

        public Line2[] Edges { get; }

        public static Triangle2 operator +(Triangle2 triangle, Vector2 delta)
            => new Triangle2(
                triangle.A + delta,
                triangle.B + delta,
                triangle.C + delta);

        public override string ToString() => $"Triangle:{A},{B},{C}";
    }

    public struct Polygon2 
    {
        public Polygon2(params Vector2[] vertices)
        {
            this.Vertices = vertices;
            this.Edges  = GetEdges(vertices);
        }
        public Vector2[] Vertices { get; }
        public Line2[] Edges { get; }

        static Line2[] GetEdges(Vector2[] vertices)
        {
            List<Line2> edges = new List<Line2>();
            if (vertices.Length > 1)
            {
                Vector2 point = vertices[0];
                for (int i = 1; i < vertices.Length; i++)
                {
                    var next = vertices[i];
                    edges.Add(point.To(next));
                    point = next;
                }
                edges.Add(point.To(vertices[0]));
            }
            return edges.ToArray();
        }

        public static Polygon2 operator +(Polygon2 polygon, Vector2 delta)
            => new Polygon2(polygon.Vertices.Select((v) => v + delta).ToArray());

        public override string ToString() => $"Poly({Vertices.Length}):{string.Join(",", Vertices.Select((v)=>v.ToString()))}";
    }
}
