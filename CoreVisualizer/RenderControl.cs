using CoreVisualizer.Interfaces;
using GlmSharp;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public partial class RenderControl : UserControl, IRenderControl
    {
        internal static OpenGL Gl { get; set; }
        private Dictionary<string, ShaderProgramCreator> Programs { get; set; }

        private vec3 lastWorldPos;
        private Point lastMousePos;
        private Camera camera;
        private Grid grid;
        private Arrows arrow;

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

        public void AlignCamera(ViewPlane plane)
        {
            camera.SetViewPlane(plane);
            DoRender();
        }

        private void DrawGrid(object sender, RenderEventArgs args)
        {
            var backColor = new float[] { BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f };
            grid.Draw(Programs["Grid"]);
        }

        private void CreateShaderProgram(string key, string[] vSource, string[] fSource)
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromString(Gl.GL_VERTEX_SHADER, vSource);
            program.CreateShaderFromString(Gl.GL_FRAGMENT_SHADER, fSource);
            program.Link();
            Programs.Add(key, program);
        }

        private void OnInit(object sender, EventArgs e)
        {
            Gl = glControl.OpenGL;
            glControl.MouseWheel += OnMouseWheel;
            glControl.OpenGLDraw += DrawScene;
            ShowGrid(true);

            Programs = new Dictionary<string, ShaderProgramCreator>();

            Gl.Enable(Gl.GL_DEPTH_TEST);
            Gl.DepthFunc(Gl.GL_LEQUAL);
            Gl.Enable(Gl.GL_LINE_SMOOTH);

            camera = new Camera(new vec3(0.0f, 0.0f, 1.0f), new vec3(0.0f, 0.0f, 0.0f), (float)glControl.Width / glControl.Height);
            grid = new Grid(Camera.AspectRatio);
            arrow = new Arrows();
            CreateShaderProgram("Grid", GridShaders.gridVertex, GridShaders.gridFragment);
            CreateShaderProgram("Surface", ArrowShaders.surfaceVertex, ArrowShaders.surfaceFragment);
            Disposed += OnDisposed;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            grid.Dispose();
            arrow.Dispose();
            foreach (var program in Programs)
                program.Value.Dispose();
        }

        private void DrawScene(object sender, RenderEventArgs args)
        {
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            Gl.ClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //Gl.PolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);//Режимы отображения будут позже
            arrow?.Draw(Programs["Surface"]);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var aspectRatio = (float)glControl.Width / glControl.Height;
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            camera.ChangePerspectiveProjection((float)Math.PI / 3, aspectRatio, 0.1f, 100f);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lastWorldPos = camera.GetWorldPosition();
            lastMousePos = e.Location;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var dir = Camera.Target - lastWorldPos;
                Camera.Target = camera.GetWorldPosition() + dir;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var newMouseCoords = new Point(e.X - (glControl.Width / 2), -e.Y + glControl.Height / 2);
            if (e.Button == MouseButtons.Middle)
            {
                var deltaY = (lastMousePos.X - e.X) / (float)glControl.Width * Camera.MouseSensX;
                var deltaX = (e.Y - lastMousePos.Y) / (float)glControl.Height * Camera.MouseSensY;
                camera.Rotate(deltaX, deltaY, 0);
                DoRender();
            }
            if (e.Button == MouseButtons.Right)
            {
                var dX = (lastMousePos.X - e.X) / (float)glControl.Width * Camera.MouseSensX;
                var dY = (e.Y - lastMousePos.Y) / (float)glControl.Height * Camera.MouseSensY;
                camera.Translate(dX, dY, 0);
                DoRender();
            }
            lastMousePos = e.Location;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var deltaZ = e.Delta / 1000f * Camera.MouseWheelSens;
            camera.Translate(0, 0, -deltaZ);
            DoRender();
        }
    }
}
