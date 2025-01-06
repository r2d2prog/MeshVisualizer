using GlmSharp;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public class Arrows : IVaoSurface
    {
        private static OpenGL Gl = RenderControl.Gl;
        public uint[] EBO { get; set; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }
        public uint[] MatrixBuffer { get; set; }
        public int Indices { get; set; }
        public mat4[] ModelMatrix { get; set; }

        public Arrows()
        {
            var rX = mat4.RotateZ(glm.Radians(-90f));
            var rY = mat4.Identity;
            var rZ = mat4.RotateX(glm.Radians(90f));

            var sX = mat4.Scale(0.05f, 0.05f, 0.05f);

            var tX = mat4.Translate(new vec3(0.04f, 0.0f, 0.0f));
            var tY = mat4.Translate(new vec3(0.0f, 0.034f, 0.0f));
            var tZ = mat4.Translate(new vec3(0.0f, 0.00f, 0.04f));

            var xAxis = tX * rX * sX;
            var yAxis = tY * rY * sX;
            var zAxis = tZ * rZ * sX;
            ModelMatrix = new mat4[] { xAxis, yAxis, zAxis };
            List<int> indices = null, conIndices = null;
            List<float> coords = null, conCoords = null;
            List<float> colors = null;

            Action<List<int>> getCylIndices = (l) => { indices = l; };
            Action<List<float>> getCylCoords = (l) => { coords = l; };

            Action<List<int>> getConIndices = (l) => { conIndices = l; };
            Action<List<float>> getConCoords = (l) => { conCoords = l; };

            var cyl = new Cylinder(32, 1, 0.1f, 1.5f, vec3.Zero, Color.Red);
            cyl.getIndicesArray += getCylIndices;
            cyl.getCoordsArray += getCylCoords;
            cyl.Create(CreateFlags.None | CreateFlags.NoColor);

            var con = new Cone(32, 1, 0.25f, 0.5f, new vec3(0, 1.0f, 0), Color.Red);
            con.getIndicesArray += getConIndices;
            con.getCoordsArray += getConCoords;
            con.Create(CreateFlags.None | CreateFlags.NoColor);

            var maxIndex = indices.Max() + 1;
            conIndices = conIndices.Select(v => v + maxIndex).ToList();
            indices.AddRange(conIndices);
            Indices = indices.Count;

            coords.AddRange(conCoords);
            CreateVertexArray(indices, coords, colors);
        }

        public void Dispose()
        {
            if(ColorBuffer != null)
                Gl.DeleteBuffers(1, ColorBuffer);
            if (MatrixBuffer != null)
                Gl.DeleteBuffers(1, MatrixBuffer);
            Gl.DeleteBuffers(1, VertexBuffer);
            Gl.DeleteBuffers(1, EBO);
            Gl.DeleteVertexArrays(1, VAO);
        }

        public void Draw(ShaderProgramCreator program)
        {
            if (VAO == null || VAO[0] == 0)
                return;
            var vp = new int[4];
            Gl.GetInteger(Gl.GL_VIEWPORT, vp);
            var koef = (float)vp[2] / vp[3];
            var oldProj = Camera.Projection;
            var oldView = Camera.View;
            var currentView = oldView;
            currentView.Column3 = new vec4 (new vec3(0.11f,0.11f,0), 1); 

            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);

            Camera.View = currentView;
            Camera.Projection = mat4.Ortho(0.0f, koef, 0.0f, 1f);
            program.SetUniform("projection", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());

            Gl.DrawElementsInstanced(Gl.GL_TRIANGLES, Indices, Gl.GL_UNSIGNED_INT, IntPtr.Zero, ModelMatrix.Length);

            Camera.View = oldView;
            Camera.Projection = oldProj;
            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public void CreateVertexArray(List<int> indices, List<float> coords, List<float> colors)
        {
            VAO = new uint[1];
            EBO = new uint[1];
            VertexBuffer = new uint[1];
            
            Gl.GenVertexArrays(1, VAO);
            Gl.GenBuffers(1, EBO);
            Gl.GenBuffers(1, VertexBuffer);

            Gl.BindVertexArray(VAO[0]);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, EBO[0]);

            var indArray = indices.ToArray();
            IntPtr intPtr = Marshal.AllocHGlobal(indArray.Length * sizeof(int));
            Marshal.Copy(indArray, 0, intPtr, indArray.Length);
            Gl.BufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, indArray.Length * sizeof(int), intPtr, Gl.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, VertexBuffer[0]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, coords.ToArray(), Gl.GL_STATIC_DRAW);
            
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

            if (colors != null)
            {
                ColorBuffer = new uint[1];
                Gl.GenBuffers(1, ColorBuffer);
                
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, ColorBuffer[0]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, colors.ToArray(), Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(1);
                Gl.VertexAttribPointer(1, 4, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            MatrixBuffer = new uint[1];
            Gl.GenBuffers(1, MatrixBuffer);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, MatrixBuffer[0]);
            var matrixSize = Marshal.SizeOf(typeof(mat4));
            var vec4Size = Marshal.SizeOf(typeof(vec4));
            var size = matrixSize * ModelMatrix.Length;

            var modelMatrices = ModelMatrix.SelectMany(v => v.ToArray()).ToArray();
            IntPtr modelPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(modelMatrices, 0, modelPtr, modelMatrices.Length);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, size, modelPtr, Gl.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(modelPtr);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 4, Gl.GL_FLOAT, false, matrixSize, IntPtr.Zero);
            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 4, Gl.GL_FLOAT, false, matrixSize, (IntPtr)(vec4Size * 1));
            Gl.EnableVertexAttribArray(3);
            Gl.VertexAttribPointer(3, 4, Gl.GL_FLOAT, false, matrixSize, (IntPtr)(vec4Size * 2));
            Gl.EnableVertexAttribArray(4);
            Gl.VertexAttribPointer(4, 4, Gl.GL_FLOAT, false, matrixSize, (IntPtr)(vec4Size * 3));
            Gl.VertexAttribDivisor(1, 1);
            Gl.VertexAttribDivisor(2, 1);
            Gl.VertexAttribDivisor(3, 1);
            Gl.VertexAttribDivisor(4, 1);

            Gl.BindVertexArray(0);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, 0);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, 0);
        }
    }

    public class ArrowShaders
    {
        public static string[] arrowsVertex = new string[]
        {
            "#version 330 core\n",
            "layout (location = 0) in vec3 position;\n",
            "layout (location = 1) in mat4 model;\n",
            "uniform mat4 projection;\n",
            "uniform mat4 view;\n",
            "flat out vec4 inColor;\n",
            "void main()\n",
            "{\n",
                "vec4 color[3];\n",
                "color[0] = vec4(1.0, 0.0, 0.0, 1.0);\n",
                "color[1] = vec4(0.0, 1.0, 0.0, 1.0);\n",
                "color[2] = vec4(0.0, 0.0, 1.0, 1.0);\n",
                "gl_Position = projection * view * model * vec4(position, 1.0);\n",
                "inColor = color[gl_InstanceID];\n",
            "}\n",
        };

        public static string[] arrowsFragment = new string[]
        {
            "#version 330 core\n",
            "flat in vec4 inColor;\n",
            "void main()\n",
            "{\n",
                "gl_FragColor = inColor;\n",
            "}\n"
        };
    }
}
