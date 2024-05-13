using JA.Geometry;

using System;

using static System.Math;

namespace JA
{
    public static class CRender
    {
        public static bool SetCursor(float x, float y)
        {
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                return false;
            }
            if (float.IsInfinity(x) || float.IsInfinity(y))
            {
                return false;
            }
            if (float.IsNegativeInfinity(x) || float.IsNegativeInfinity(y))
            {
                return false;
            }
            int px = (int)(x + 0.5f), py = (int)(y + 0.5f);
            int wt = Console.WindowWidth, ht = Console.WindowHeight;

            if (px>=0 && px<wt && py>=0 && py<ht)
            {
                Console.SetCursorPosition(px, py);
                return true;
            }
            return false;
        }
        public static bool SetCursor(Vector2 position)
        {
            return SetCursor(position.X, position.Y);
        }
        public static void DrawDot(bool big = false)
        {
            // // █ ▀ ▄ ■ ▪ ·
            if (big)
            {
                Console.Write('■');
            }
            else
            {
                Console.Write('·');
            }
        }
        public static bool DrawDot(float x, float y, bool big = false)
        {
            if (SetCursor(x, y))
            {
                DrawDot(big);
                return true;
            }
            return false;
        }

        public static bool Draw(this Vector2 position, bool big = false)
            => DrawDot(position.X, position.Y, big);

        public static void Draw(this Line2 line)
        {
            float del = Max(Abs(line.To.X - line.From.X), Abs(line.To.Y - line.From.Y));
            if (del < 0.5)
            {
                return;
            }
            int n = (int)del;
            if (n > 0)
            {
                for (int i = 0; i <= n; i++)
                {
                    DrawDot(
                        line.From.X + ((line.To.X - line.From.X) * i) / n,
                        line.From.Y + ((line.To.Y - line.From.Y) * i) / n,
                        i == 0 || i == n);
                }
            }
            else
            {
                DrawDot(line.From.X, line.From.Y, true);
            }
        }
        public static void Draw(this Triangle2 triangle)
        {
            new Line2(triangle.A, triangle.B).Draw();
            new Line2(triangle.B, triangle.C).Draw();
            new Line2(triangle.C, triangle.A).Draw();
        }

        public static void Draw(this Polygon2 polygon)
        {
            var edges = polygon.Edges;
            if (edges.Length>0)
            {
                edges.Draw();
            }
            else if (polygon.Vertices.Length == 1)
            {
                Draw(polygon.Vertices[0], true);
            }
        }

        public static void Draw(this Line2[] edges)
        {
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i].Draw();
            }
        }
    }
}
