using JA.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace JA.Scenes
{
    public class Scene
    {
        public Scene(float modelSize, float cameraDistance)
        {
            this.InnerShapes = new List<Shape>();
            this.ModelSize = modelSize;
            this.CameraDistance = cameraDistance;
            this.Aspect = 2;
        }
        public float Aspect { get; set; }
        public float ModelSize { get; set; }
        public float CameraDistance { get; set; }
        protected List<Shape> InnerShapes { get; }
        public IReadOnlyList<Shape> Shapes => InnerShapes.AsReadOnly();

        public void Draw()
        {
            foreach (var item in Shapes)
            {
                item.Draw(this);
            }
        }
        public void RotateX(float angle)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].RotateX(angle);
            }
        }
        public void RotateY(float angle)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].RotateY(angle);
            }
        }
        public void RotateZ(float angle)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].RotateZ(angle);
            }
        }
        public void RotateAbout(Vector3 axis, float angle)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].RotateAbout(axis, angle);
            }
        }

        public void Translate(Vector3 delta)
        {
            for (int i = 0; i < Shapes.Count; i++)
            {
                Shapes[i].Translate(delta);
            }
        }

        public void AddShape(Shape shape)
        {
            InnerShapes.Add(shape);
        }
        public void AddShapes(params Shape[] shapes)
        {
            foreach (var item in shapes)
            {
                AddShape(item);
            }
        }

        public IEnumerable<Polygon2> Project(IEnumerable<Vector3[]> faces, bool cullBackFaces)
        {
            var wt = Console.WindowWidth;
            var ht = Console.WindowHeight;
            var screenSize = Min(wt, ht);

            List<Polygon2> polygons = new List<Polygon2>();
            foreach (var nodes in faces)
            {
                if (cullBackFaces && nodes.Length >= 3)
                {
                    Vector3 normal = ((nodes[0] ^ nodes[1]) + (nodes[1] ^ nodes[2]) + (nodes[2] ^ nodes[0])).Normalized();
                    Vector3 center = Shape.GetCenterOfFace(nodes);
                    Vector3 eye = center - CameraDistance * Vector3.UnitZ;

                    if (eye.Dot(normal) > 0)
                    {
                        continue;
                    }
                }
                // Project here
                Vector2[] points = new Vector2[nodes.Length];
                for (int j = 0; j < points.Length; j++)
                {
                    points[j] = new Vector2(
                        wt / 2 + Aspect * screenSize / 2 * CameraDistance * nodes[j].X / (ModelSize * (CameraDistance - nodes[j].Z)),
                        ht / 2 - screenSize / 2 * CameraDistance * nodes[j].Y / (ModelSize * (CameraDistance - nodes[j].Z)));
                }
                yield return new Polygon2(points);
            }
        }
    }
}
