using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using static System.Math;
using System.IO;
using JA.Geometry;

namespace JA.Scenes
{

    public abstract class Shape
    {
        public abstract IEnumerable<Vector3[]> GetFaces();

        protected Shape(ConsoleColor color)
        {
            this.InnerVertices = new List<Vector3>();
            this.Color = color;
            this.CullBackFaces = false;
        }
        public static Shape Segement(Vector3 a, Vector3 b, ConsoleColor color)
            => new Segment3(a, b, color);
        public static Shape Triangle(Vector3 a, Vector3 b, Vector3 c, ConsoleColor color)
            => new Triangle3(a, b, c, color);
        public static Mesh3 Cuboid(Vector3 center, float Δx, float Δy, float Δz, ConsoleColor color)
            => new Cuboid3(center, Δx, Δy, Δz, color);
        public static Mesh3 Stl(string filename, ConsoleColor color, float scale = 1f)
            => new StlMesh3(filename, color, scale);

        protected List<Vector3> InnerVertices { get; }
        public IReadOnlyList<Vector3> Vertices => InnerVertices.AsReadOnly();
        public ConsoleColor Color { get; set; }
        public bool CullBackFaces { get; set; }


        protected void AddVertex(Vector3 vector)
            => InnerVertices.Add(vector);

        public void RotateX(float angle)
        {
            for (int i = 0; i < InnerVertices.Count; i++)
            {
                InnerVertices[i] = InnerVertices[i].RotateX(angle);
            }
        }
        public void RotateY(float angle)
        {
            for (int i = 0; i < InnerVertices.Count; i++)
            {
                InnerVertices[i] = InnerVertices[i].RotateY(angle);
            }
        }
        public void RotateZ(float angle)
        {
            for (int i = 0; i < InnerVertices.Count; i++)
            {
                InnerVertices[i] = InnerVertices[i].RotateZ(angle);
            }
        }
        public void RotateAbout(Vector3 axis, float angle)
        {
            for (int i = 0; i < InnerVertices.Count; i++)
            {
                InnerVertices[i] = InnerVertices[i].RotateAbout(axis, angle);
            }
        }

        public void Translate(Vector3 delta)
        {
            for (int i = 0; i < InnerVertices.Count; i++)
            {
                InnerVertices[i] += delta;
            }
        }

        public void Scale(float factor) 
            => Scale(factor, Vector3.Zero);

        public void Scale(float factor, Vector3 center)
        {
            if (factor != 1)
            {
                for (int i = 0; i < InnerVertices.Count; i++)
                {
                    InnerVertices[i] = center +  factor * (InnerVertices[i]-center);
                }
            }
        }

        public void GetBounds(out Vector3 from, out Vector3 to)
        {
            if (InnerVertices.Count == 0)
            {
                to = from = Vector3.Zero;
                return;
            }
            to = from = InnerVertices[0];
            for (int i = 1; i < InnerVertices.Count; i++)
            {
                var vertex = InnerVertices[i];
                from = new Vector3(
                    Min(from.X, vertex.X),
                    Min(from.Y, vertex.Y),
                    Min(from.Z, vertex.Z));

                to = new Vector3(
                    Max(to.X, vertex.X),
                    Max(to.Y, vertex.Y),
                    Max(to.Z, vertex.Z));
            }
        }

        public virtual void Draw(Scene scene)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            foreach (var item in scene.Project(GetFaces(), CullBackFaces))
            {
                item.Draw();
            }
            Console.ForegroundColor = old;
        }

        public static Vector3 GetCenterOfFace(Vector3[] nodes)
        {
            Vector3 center = Vector3.Zero;
            for (int j = 0; j < nodes.Length; j++)
            {
                center += nodes[j];
            }
            center /= nodes.Length;
            return center;
        }
    }
    public class Segment3 : Shape
    {
        public Segment3(Vector3 from, Vector3 to, ConsoleColor color)
             : base(color)
        {
            AddVertex(from);
            AddVertex(to);
        }
        public Vector3 From { get => InnerVertices[0]; }
        public Vector3 To { get => InnerVertices[1]; }

        public override IEnumerable<Vector3[]> GetFaces()
            => new Vector3[][] { InnerVertices.ToArray() };
    }
    public class Csys3 : Shape
    {
        public Csys3(float size) : this(Vector3.Zero, size) { }
        public Csys3(Vector3 center, float size) : base(ConsoleColor.Black)
        {
            AddVertex(center);
            AddVertex(center + size*Vector3.UnitX);
            AddVertex(center + size*Vector3.UnitY);
            AddVertex(center + size*Vector3.UnitZ);            
        }
        public Vector3 Center { get => InnerVertices[0]; }
        public Vector3 UX { get => InnerVertices[1] - InnerVertices[0]; }
        public Vector3 UY { get => InnerVertices[2] - InnerVertices[0]; }
        public Vector3 UZ { get => InnerVertices[3] - InnerVertices[0]; }

        public override IEnumerable<Vector3[]> GetFaces()
        {
            yield return new Vector3[] { InnerVertices[0], InnerVertices[1] };
            yield return new Vector3[] { InnerVertices[0], InnerVertices[2] };
            yield return new Vector3[] { InnerVertices[0], InnerVertices[3] };
        }
        public override void Draw(Scene scene)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            var faces = GetFaces().ToArray();
            var axes = scene.Project(GetFaces(), false).ToArray();
            Console.ForegroundColor = ConsoleColor.Red;
            axes[0].Draw();
            Console.ForegroundColor = ConsoleColor.Yellow;
            axes[1].Draw();
            Console.ForegroundColor = ConsoleColor.Magenta;
            axes[2].Draw();
            Console.ForegroundColor = old;
        }
    }
    public class Triangle3 : Shape
    {
        public Triangle3(Vector3 a, Vector3 b, Vector3 c, ConsoleColor color)
            : base(color)
        {
            AddVertex(a);
            AddVertex(b);
            AddVertex(c);
        }
        public override IEnumerable<Vector3[]> GetFaces()
            => new Vector3[][] { Vertices.ToArray() };
    }
    public class Mesh3 : Shape
    {
        public Mesh3(ConsoleColor color)
            : base(color)
        {
            this.Faces = new List<int[]>();
        }
        public List<int[]> Faces { get; }

        public void AddFace(params Vector3[] nodes)
        {
            int[] f_index = new int[nodes.Length];

            for (int i = 0; i < f_index.Length; i++)
            {
                int index = InnerVertices.IndexOf(nodes[i]);
                if (index < 0)
                {
                    index = InnerVertices.Count;
                    InnerVertices.Add(nodes[i]);
                }
                f_index[i] = index;
            }

            Faces.Add(f_index);
        }

        public override IEnumerable<Vector3[]> GetFaces()
        {
            //List<Vector3[]> faceList = new List<Vector3[]>();
            for (int i = 0; i < Faces.Count; i++)
            {
                var f_index = Faces[i];

                Vector3[] nodes = new Vector3[f_index.Length];
                for (int j = 0; j < f_index.Length; j++)
                {
                    nodes[j] = InnerVertices[f_index[j]];
                }

                //faceList.Add(nodes);

                yield return nodes;
            }

            //return faceList.ToArray();
        }
    }

    public class Rectangle3 : Mesh3
    {
        public Rectangle3(Vector3 center, float Δx, float Δy, ConsoleColor color)
            : base(color)
        {

            var A = new Vector3(-Δx / 2, -Δy / 2, 0) + center;
            var B = new Vector3( Δx / 2, -Δy / 2, 0) + center;
            var C = new Vector3( Δx / 2,  Δy / 2, 0) + center;
            var D = new Vector3(-Δx / 2,  Δy / 2, 0) + center;

            AddFace(A, B, C, D);
        }
    }

    public class Cuboid3 : Mesh3
    {

        public Cuboid3(Vector3 center, float Δx, float Δy, float Δz, ConsoleColor color)
            : base(color)
        {
            var A = new Vector3(-Δx / 2, -Δy / 2, +Δz / 2) + center;
            var B = new Vector3(+Δx / 2, -Δy / 2, +Δz / 2) + center;
            var C = new Vector3(+Δx / 2, +Δy / 2, +Δz / 2) + center;
            var D = new Vector3(-Δx / 2, +Δy / 2, +Δz / 2) + center;
            var E = new Vector3(-Δx / 2, -Δy / 2, -Δz / 2) + center;
            var F = new Vector3(+Δx / 2, -Δy / 2, -Δz / 2) + center;
            var G = new Vector3(+Δx / 2, +Δy / 2, -Δz / 2) + center;
            var H = new Vector3(-Δx / 2, +Δy / 2, -Δz / 2) + center;

            AddFace(A, B, C, D);
            AddFace(F, G, C, B);
            AddFace(E, H, G, F);
            AddFace(A, D, H, E);
            AddFace(B, A, E, F);
            AddFace(D, C, G, H);

        }
    }

    public class Tetrahedron3 : Mesh3
    {
        static readonly float pi = (float)PI;

        public Tetrahedron3(Vector3 center, float radius, float height, ConsoleColor color)
            : base(color)
        {
            var A = Vector3.Cylindrical(radius, 0, -height / 4);
            var B = Vector3.Cylindrical(radius, -2 * pi / 3, -height / 4);
            var C = Vector3.Cylindrical(radius, 2 * pi / 3, -height / 4);
            var D = Vector3.Cartesian(0, 0, 3 * height / 4);

            AddFace(A, B, C);
            AddFace(B, D, C);
            AddFace(C, D, A);
            AddFace(A, D, B);
        }
    }

    public class StlMesh3 : Mesh3
    {
        public StlMesh3(string filename, ConsoleColor color, float scale = 1f)
             : base(color)
        {
            // Imports a binary STL file
            // Code Taken From:
            // https://sukhbinder.wordpress.com/2013/12/10/new-fortran-stl-binary-file-reader/
            // Aug 27, 2019
            var fs = File.OpenRead(filename);
            var stl = new BinaryReader(fs);

            var header = new string(stl.ReadChars(80));
            var n_elems = stl.ReadInt32();

            for (int i = 0; i < n_elems; i++)
            {
                var normal = new Vector3(
                    stl.ReadSingle(),
                    stl.ReadSingle(),
                    stl.ReadSingle());
                var a = new Vector3(
                    stl.ReadSingle(),
                    stl.ReadSingle(),
                    stl.ReadSingle());
                var b = new Vector3(
                    stl.ReadSingle(),
                    stl.ReadSingle(),
                    stl.ReadSingle());
                var c = new Vector3(
                    stl.ReadSingle(),
                    stl.ReadSingle(),
                    stl.ReadSingle());

                var temp = stl.ReadBytes(2);

                AddFace(a, b, c);
            }

            stl.Close();

            GetBounds(out Vector3 from, out Vector3 to);
            Translate(-(from + to) / 2);
            Scale(scale);
            RotateZ((float)(PI/2));
            CullBackFaces = true;

        }
    }
}
