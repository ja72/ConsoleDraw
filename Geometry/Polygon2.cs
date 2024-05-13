using System.Collections.Generic;
using System.Linq;

namespace JA.Geometry
{
    public readonly struct Polygon2
    {
        public Polygon2(params Vector2[] vertices)
        {
            Vertices = vertices;
            Edges = GetEdges(vertices);
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

        public override string ToString() => $"Poly({Vertices.Length}):{string.Join(",", Vertices.Select((v) => v.ToString()))}";
    }
}
