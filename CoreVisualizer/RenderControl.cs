using CoreVisualizer.Interfaces;
using GlmSharp;
using SharpGL;
using SharpGL.SceneGraph.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public partial class RenderControl : UserControl, IRenderControl
    {
        internal static OpenGL Gl { get; set; }
        //private Dictionary<string, Shader> Shaders { get; set; }
        private Dictionary<string, ShaderProgramCreator> Programs { get; set; }

        private Grid grid;

        public RenderControl()
        {
            InitializeComponent();
        }

        public void DoRender()
        {
            glControl.Invalidate();
        }

        public void ShowGrid(bool show)
        {
            if (show)
                glControl.OpenGLDraw += DrawGrid;
            else
                glControl.OpenGLDraw -= DrawGrid;
        }

        private void CreateGrid()
        {
            grid = new Grid(glControl.Width / (float)glControl.Height);
            var program = grid.CreateShaderProgram();
            Programs.Add("Grid", program);
        }

        private void DrawGrid(object sender, RenderEventArgs args)
        {
            var backColor = new float[] { BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f };
            grid.Draw(Programs["Grid"], (float)glControl.Width / glControl.Height);
        }

        private void OnInit(object sender, EventArgs e)
        {
            Gl = glControl.OpenGL;
            glControl.OpenGLDraw += DrawScene;
            Programs = new Dictionary<string, ShaderProgramCreator>();

            ShowGrid(true);
            Gl.Enable(Gl.GL_DEPTH_TEST);
            Gl.DepthFunc(Gl.GL_LEQUAL);
            Gl.Enable(Gl.GL_LINE_SMOOTH);

            CreateGrid();
            Disposed += OnDisposed;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            grid.Dispose();
            foreach(var program in Programs)
                program.Value.Dispose();
        }

        private void DrawScene(object sender, RenderEventArgs args)
        {
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            Gl.ClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
        }
    }
}
