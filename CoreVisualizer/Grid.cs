using SharpGL;
using SharpGL.SceneGraph.Shaders;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gl = SharpGL.OpenGL;
using GlmSharp;
using SharpGL.SceneGraph;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Forms;

namespace CoreVisualizer
{
    public class Grid : IDisposable
    {
        private static OpenGL Gl = RenderControl.Gl;

        private static Color ThinColor { get; set; }  = Color.DarkSlateGray;
        private static Color ThickColor { get; set; } = Color.LightSlateGray;

        private static float ThinThickness { get; set; } = 1f;
        private static float ThickThickness { get; set; } = 2.0f;

        private int ThinStart { get; set; }
        private int ThinCount { get; set; }
        private int ThickStart { get; set; }
        private int ThickCount { get; set; }

        private uint[] VAO {  get; set; }
        private uint[] VertexBuffer { get; set; }
        private uint[] ColorBuffer { get; set; }
        private enum LineLayout 
        { 
            Vertical,
            Horizontal
        }
        
        public Grid(float aspectRatio, uint gridSize = 200) 
        {
            if (gridSize != 0)
            {
                var modelHalfHeight = Math.Sqrt(3) / 3 * 0.1f;
                var modelHalfWidth = aspectRatio * modelHalfHeight;
                CreateLines((float)modelHalfWidth, gridSize);
            }
        }

        public ShaderProgramCreator CreateShaderProgram()
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromString(Gl.GL_VERTEX_SHADER, GridShaders.gridVertex);
            program.CreateShaderFromString(Gl.GL_FRAGMENT_SHADER, GridShaders.gridFragment);
            program.Link();
            return program;
        }

        public void Draw(ShaderProgramCreator program, float aRatio)
        {
            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);
            
            var proj = mat4.Perspective((float)Math.PI / 3, aRatio, 0.1f, 100f).ToArray();
            var view = mat4.LookAt(new vec3(0.5f, 0.25f, 0.2f), new vec3(0, 0, 0), new vec3(0, 1, 0)).ToArray();
            program.SetUniform("perspective", proj);
            program.SetUniform("view", view);

            Gl.LineWidth(ThinThickness);
            Gl.DrawArrays(Gl.GL_LINES, ThinStart, ThinCount);

            Gl.LineWidth(ThickThickness);
            Gl.DrawArrays(Gl.GL_LINES, ThickStart, ThickCount);

            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(1, ColorBuffer);
            Gl.DeleteBuffers(1, VertexBuffer);
            Gl.DeleteVertexArrays(1, VAO);
        }

        private void CreateLines(float modelSize, uint gridSize)
        {
            var thinCoords = new List<float>();
            var thickCoords = new List<float>();
            var maxCoord = ((gridSize >> 1) + (gridSize & 1)) * modelSize;
            for (var i = 0; i <= gridSize; ++i)
            {
                var parity = i & 1;
                var index = (i >> 1) + parity;
                index = parity == 1 ? index : -index;
                var thickStatus = index % 10 == 0;
                var storage = thickStatus ? thickCoords : thinCoords;
                CreateLineCoords(storage, index, modelSize, maxCoord, LineLayout.Vertical);
                CreateLineCoords(storage, index, modelSize, maxCoord, LineLayout.Horizontal);
            }
            ThinStart = 0;
            ThinCount = thinCoords.Count / 3;

            ThickStart = ThinCount;
            ThickCount = thickCoords.Count / 3;

            var thinColors = CreateColors(ThinColor);
            var thickColors = CreateColors(ThickColor);

            thinCoords.AddRange(thickCoords);
            var coordsData = thinCoords.ToArray();       
            var colorsData = thinColors.Concat(thickColors).ToArray();
            CreateVertexArray(coordsData, colorsData);
        }

        private void CreateLineCoords(List<float> storage, int index, float modelSize, float maxCoord, LineLayout layout)
        {
            var line = new float[6];
            if (layout == LineLayout.Vertical)
            {
                line[0] = index * modelSize;
                line[2] = maxCoord;

                line[3] = index * modelSize;
                line[5] = -maxCoord;
            }
            else
            {
                line[0] = -maxCoord;
                line[2] = index * modelSize;

                line[3] = maxCoord;
                line[5] = index * modelSize;
            }
            storage.AddRange(line);
        }

        private float[] CreateColors(Color color)
        {
            return Enumerable.Range(0, ThinCount).Select(v => color)
                             .Select(v => new float[] { v.R / 255f, v.G / 255f, v.B / 255f, v.A / 255f })
                             .SelectMany(v => v).ToArray();
        }

        private void CreateVertexArray(float[] coords, float[] colors)
        {
            VAO = new uint[1];
            VertexBuffer = new uint[1];
            ColorBuffer = new uint[1];

            Gl.GenVertexArrays(1, VAO);
            Gl.GenBuffers(1, VertexBuffer);
            Gl.GenBuffers(1, ColorBuffer);

            Gl.BindVertexArray(VAO[0]);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, VertexBuffer[0]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, coords, Gl.GL_STATIC_DRAW);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, ColorBuffer[0]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, colors, Gl.GL_STATIC_DRAW);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 4, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

            Gl.BindVertexArray(0);
        }
    }

    public class GridShaders
    {
        public static string[] gridVertex = new string[]
        {
            "#version 330 core\n",
            "layout (location = 0) in vec3 position;\n",
            "layout(location = 1) in vec4 color;\n",
            "uniform mat4 perspective;\n",
            "uniform mat4 view;\n",
            "flat out vec4 inColor;\n",
            "void main()\n",
            "{\n",
                "gl_Position = perspective * view * vec4(position, 1.0);\n",
                "inColor = color;\n",
            "}\n",
        };

        public static string[] gridFragment = new string[]
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
