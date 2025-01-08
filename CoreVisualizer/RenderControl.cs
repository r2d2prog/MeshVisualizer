using CoreVisualizer.Interfaces;
using GlmSharp;
using SharpGL;
using Assimp;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gl = SharpGL.OpenGL;
using System.Resources;
using System.Reflection;
using CoreVisualizer.Properties;

namespace CoreVisualizer
{
    public partial class RenderControl : UserControl , IRenderControl
    {
        internal static OpenGL Gl { get; set; }
        private Dictionary<string, ShaderProgramCreator> Programs { get; set; }
        private Dictionary<string, Model> Models { get; set; }

        private vec3 lastWorldPos;
        private Point lastMousePos;
        private Camera camera;
        private Grid grid;

        public RenderControl()
        {
            InitializeComponent();
        }

        public void DoRender()
        {
            glControl.Invalidate();
        }

        public void AlignCamera(ViewPlane plane)
        {
            camera.SetViewPlane(plane);
            DoRender();
        }

        public void LoadModel(string path)
        {
            var model = new Model(path);
            var modelName = Path.GetFileNameWithoutExtension(path);
            Models.Add(modelName, model);
        }

        private void CreateShaderProgramFromResource(string key, string vertexSource, string fragmentSource)
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromString(Gl.GL_VERTEX_SHADER, vertexSource);
            program.CreateShaderFromString(Gl.GL_FRAGMENT_SHADER, fragmentSource);
            program.Link();
            Programs.Add(key, program);
        }

        private void CreateShaderProgramFromArrays(string key, string[] vSource, string[] fSource)
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromStringArray(Gl.GL_VERTEX_SHADER, vSource);
            program.CreateShaderFromStringArray(Gl.GL_FRAGMENT_SHADER, fSource);
            program.Link();
            Programs.Add(key, program);
        }

        private void CreateMeshTexturedProgram()
        {
            var sbVertex = new StringBuilder(Resources.mesh_vs);
            var sbFragment = new StringBuilder(Resources.mesh_fs);
            sbVertex.Replace("#define USE_MATERIAL", "#define USE_TEXTURES");
            sbFragment.Replace("#define USE_MATERIAL", "#define USE_TEXTURES");

            var tsProgram = new ShaderProgramCreator();
            tsProgram.CreateShaderFromString(Gl.GL_VERTEX_SHADER, sbVertex.ToString());
            tsProgram.CreateShaderFromString(Gl.GL_FRAGMENT_SHADER, sbFragment.ToString());
            tsProgram.Link();
            Programs.Add("MeshTangentSpace", tsProgram);

            sbVertex.Replace("#define TANGENT_SPACE", "#define MODEL_SPACE");
            sbFragment.Replace("#define TANGENT_SPACE", "#define MODEL_SPACE");
            var msProgram = new ShaderProgramCreator();
            msProgram.CreateShaderFromString(Gl.GL_VERTEX_SHADER, sbVertex.ToString());
            msProgram.CreateShaderFromString(Gl.GL_FRAGMENT_SHADER, sbFragment.ToString());
            msProgram.Link();
            Programs.Add("MeshModelSpace", msProgram);
        }

        private void OnInit(object sender, EventArgs e)
        {
            Gl = glControl.OpenGL;
            glControl.MouseWheel += OnMouseWheel;
            glControl.OpenGLDraw += DrawScene;

            Programs = new Dictionary<string, ShaderProgramCreator>();
            Models = new Dictionary<string, Model>();

            Gl.Enable(Gl.GL_DEPTH_TEST);
            Gl.DepthFunc(Gl.GL_LEQUAL);
            Gl.Enable(Gl.GL_LINE_SMOOTH);

            camera = new Camera(new vec3(0.57735f, 0.57735f, 0.57735f), new vec3(0.0f, 0.0f, 0.0f), (float)glControl.Width / glControl.Height);
            grid = new Grid(Camera.AspectRatio);
            CreateShaderProgramFromResource("Grid", Resources.grid_vs, Resources.grid_fs);
            CreateShaderProgramFromResource("Arrows", Resources.arrows_vs, Resources.arrows_fs);
            CreateShaderProgramFromResource("Labels", Resources.labels_vs, Resources.labels_fs);
            CreateShaderProgramFromResource("MeshMaterial", Resources.mesh_vs, Resources.mesh_fs);
            CreateMeshTexturedProgram();
            Disposed += OnDisposed;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            grid?.Dispose();
            camera?.Dispose();
            foreach (var model in Models.Values)
                model.Dispose();
            foreach (var program in Programs)
                program.Value.Dispose();
        }

        private void DrawScene(object sender, RenderEventArgs args)
        {
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            Gl.ClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.Clear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //Gl.Disable(Gl.GL_CULL_FACE);
            Gl.PolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);//Режимы отображения будут позже
            foreach (var model in Models)
            {
                
                model.Value.Draw(Programs);
            }
            grid?.Draw(Programs["Grid"]);
            camera?.DisplayAxises(Programs["Arrows"], Programs["Labels"]);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var aspectRatio = (float)glControl.Width / glControl.Height;
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            camera?.ChangePerspectiveProjection((float)Math.PI / 3, aspectRatio, 0.1f, 100f);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lastWorldPos = Camera.GetWorldPosition();
            lastMousePos = e.Location;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var dir = Camera.Target - lastWorldPos;
                Camera.Target = Camera.GetWorldPosition() + dir;
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
