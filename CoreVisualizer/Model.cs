using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using System.Runtime.InteropServices;
using SharpGL;
using Gl = SharpGL.OpenGL;
using System.Drawing;
using System.IO;
using Assimp.Configs;

namespace CoreVisualizer
{
    public class Model : IVaoSurface
    {
        private static OpenGL Gl = RenderControl.Gl;
        public uint[] EBO { get ; set ; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }
        public int[] Indices { get; set; }
        public int[] Points { get; set; }
        public mat4[] ModelMatrix { get; set; }

        public string[] Names { get; private set; }

        public Model(string path)
        {
            var context = new AssimpContext();
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate /*| PostProcessSteps.JoinIdenticalVertices*/);
            var filename = Path.GetFileNameWithoutExtension(path);

            VAO = new uint[scene.MeshCount];
            EBO = new uint[scene.MeshCount];
            VertexBuffer = new uint[scene.MeshCount];
            ColorBuffer = new uint[scene.MeshCount];
            Indices = new int[scene.MeshCount];
            ModelMatrix = new mat4[scene.MeshCount];
            Names = new string[scene.MeshCount];
            Points = new int[scene.MeshCount];

            Gl.GenVertexArrays(scene.MeshCount, VAO);
            Gl.GenBuffers(scene.MeshCount, EBO);
            Gl.GenBuffers(scene.MeshCount, VertexBuffer);
            Gl.GenBuffers(scene.MeshCount, ColorBuffer);

            for (var i = 0; i < scene.MeshCount; ++i)
            {
                ModelMatrix[i] = mat4.Identity;
                Names[i] = $"{filename}_{scene.Meshes[i].Name}_{i}";
                var indices = scene.Meshes[i].GetIndices();
                Indices[i] = indices.Length;
                Points[i] = scene.Meshes[i].Vertices.Count;
                //var faces = scene.Meshes[i].Faces;
                var coords = scene.Meshes[i].Vertices.SelectMany(v => new float[] {v.X, v.Y, v.Z }).ToArray();
                var colors = CreateColors(Color.Gray, Points[i]);
                CreateVertexArray(indices, coords, colors, i);
            }
        }

        public void CreateVertexArray(int[] indices, float[] coords, float[] colors, int index = 0)
        {
            Gl.BindVertexArray(VAO[index]);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, EBO[index]);

            IntPtr intPtr = Marshal.AllocHGlobal(indices.Length * sizeof(int));
            Marshal.Copy(indices, 0, intPtr, indices.Length);
            Gl.BufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(int), intPtr, Gl.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, VertexBuffer[index]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, coords.ToArray(), Gl.GL_STATIC_DRAW);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

            if (colors != null)
            {
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, ColorBuffer[index]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, colors, Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(1);
                Gl.VertexAttribPointer(1, 4, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            Gl.BindVertexArray(0);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, 0);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(ColorBuffer.Length, ColorBuffer);
            Gl.DeleteBuffers(VertexBuffer.Length, VertexBuffer);
            Gl.DeleteBuffers(EBO.Length, EBO);
            Gl.DeleteVertexArrays(VAO.Length, VAO);
        }

        public void Draw(ShaderProgramCreator program)
        {
            while (Gl.GetError() != Gl.GL_NO_ERROR);
            if (VAO != null)
            {
                for (var i = 0; i < VAO.Length; i++)
                {
                    if (VAO[i] != 0)
                    {
                        Gl.UseProgram(program.Program);
                        Gl.BindVertexArray(VAO[i]);

                        program.SetUniform("projection", Camera.Projection.ToArray());
                        program.SetUniform("view", Camera.View.ToArray());
                        program.SetUniform("model", ModelMatrix[i].ToArray());

                        Gl.DrawElements(Gl.GL_TRIANGLES, Indices[i], Gl.GL_UNSIGNED_INT, IntPtr.Zero);

                        Gl.BindVertexArray(0);
                        Gl.UseProgram(0);
                    }
                }
            }
        }

        private float[] CreateColors(Color color, int count)
        {
            float[] colors = new float[count * 4];
            for (var i = 0; i < count; i+=4)
            {
                colors[i] = color.R / 255f;
                colors[i + 1] = color.R / 255f;
                colors[i + 2] = color.R / 255f;
                colors[i + 3] = color.R / 255f;
            }
            return colors;
        }
    }
}
