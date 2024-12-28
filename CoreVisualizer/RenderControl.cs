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
        private static OpenGL Gl { get; set; }
        private Dictionary<string, Shader> Shaders { get; set; }
        private Dictionary<string, ShaderProgram> Programs { get; set; }
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


        }

        private void DrawGrid(object sender, RenderEventArgs args)
        {

        }

        private void OnInit(object sender, EventArgs e)
        {
            Gl = glControl.OpenGL;
            glControl.OpenGLDraw += DrawScene;
        }

        

        private void DrawScene(object sender, RenderEventArgs args)
        {
            Gl.ClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
        }
    }
}
