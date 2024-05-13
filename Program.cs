using JA.Geometry;
using JA.Scenes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace JA
{

    class Program
    {
        static readonly float pi = (float)PI;

        readonly CBuffer buffer;
        readonly Scene scene;
        bool cancel = false;

        static void Main(string[] args)
        {
            // check \x0000 and \u0000 codes

#pragma warning disable S1848 // Objects should not be created to be dropped immediately without being used
            new Program();
#pragma warning restore S1848 // Objects should not be created to be dropped immediately without being used
        }

        public Program()
        {
            //Console.SetWindowSize(90, 45);
            //Console.CancelKeyPress += (s, ev) => { cancel=true; };

            this.scene = new Scene(3f, 8f);
            this.buffer = new CBuffer();
            Setup();
            Run();
        }

        private Shape AddPlane()
        {
            var shape = new Rectangle3(Vector3.UnitZ, 2, 2, ConsoleColor.Blue)
            {
                CullBackFaces = false
            };            
            shape.RotateX(-pi / 2);
            return shape;
        }

        private Shape AddTetrahedron()
        {
            Shape shape = new Tetrahedron3(Vector3.Zero, 0.6f, 1, ConsoleColor.Blue)
            {
                CullBackFaces = true
            };
            shape.Scale(2f);
            shape.RotateY(3 * pi / 12);
            return shape;
        }
        private Shape AddCube()
        {
            Shape shape = new Cuboid3(Vector3.Zero, 1f, 1f, 1f, ConsoleColor.Blue)
            {
                CullBackFaces = true
            };
            shape.Scale(2f);
            //shape.RotateY(pi / 3);
            return shape;
        }
        enum StlMeshModel
        {
            Sphere,
            Bottle,
            Teapot,
            Cube
        }
        private Mesh3 AddStlMesh(StlMeshModel model)
        {
            Mesh3 stl = null;
            switch (model)
            {
                case StlMeshModel.Sphere:
                    stl = Shape.Stl(@"Scenes\Meshes\sphere.stl", ConsoleColor.Blue, 3);
                    break;
                case StlMeshModel.Bottle:
                    stl = Shape.Stl(@"Scenes\Meshes\bottle.stl", ConsoleColor.Blue, 60);
                    break;
                case StlMeshModel.Teapot:
                    stl = Shape.Stl(@"Scenes\Meshes\teapot.stl", ConsoleColor.Blue, 30);
                    break;
                case StlMeshModel.Cube:
                    stl = Shape.Stl(@"Scenes\Meshes\cube.stl", ConsoleColor.Blue, 1);
                    break;
            }
            return stl;
        }


        private Shape AddCsys()
        {
            return new Csys3(1);
        }

        public void Setup()
        {
            Console.SetBufferSize(
                Console.WindowWidth,
                Console.WindowHeight);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            buffer.AddText(0, 0, new string('=', buffer.Width));
            buffer.AddText(0, buffer.Height-1, new string('=', buffer.Width));
            for (int i = 0; i < buffer.Height; i++)
            {
                buffer[0, i] = '#';
                buffer[buffer.Width-1, i] = '#';
            }
            buffer.AddText(buffer.Width-12, 1, "3D Wireframe of mesh and/or STL file.", 10);
            Console.Clear();

            scene.AddShape(
                //AddPlane()
                //AddTetrahedron()
                AddCube()
                //AddStlMesh(StlMeshModel.Sphere)
            );

            scene.AddShape(
                AddCsys()
            );

            scene.RotateY(pi / 6);
            //scene.RotateY(3*pi / 12);
        }

        public void Run()
        {
            const float minTimeStep = 0.05f;
            var sw = Stopwatch.StartNew();
            int frames = 0;
            float timeStep = 0;
            bool rotate = true;
            do
            {

                if (rotate)
                {
                    var tic = (float)sw.Elapsed.TotalSeconds;
                    frames++;                    
                    //Console.ForegroundColor = ConsoleColor.Gray;
                    //Console.Write(AnsiCodes.TextColor((byte)(frames % 256)));
                    Console.Write(AnsiCodes.TextColor(Color.White, false));
                    buffer.Render();
                    Console.SetCursorPosition(2, 1);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Frame={frames,-4}, FPS={frames / sw.Elapsed.TotalSeconds,-6:F2}");
                    int px = Console.CursorLeft, py = Console.CursorTop;
                    scene.Draw();
                    Console.SetCursorPosition(px, py);
                    scene.RotateX(pi / 32);
                    timeStep = (float)sw.Elapsed.TotalSeconds - tic;

                    if (timeStep < minTimeStep)
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(minTimeStep - timeStep));
                    }
                }
                else
                {
                    frames = 0;
                    sw.Restart();
                }
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape) 
                    {
                        cancel = true;
                        break;
                    }
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        rotate = !rotate;
                    }
                }
            } while (!cancel);

        }

    }


}
