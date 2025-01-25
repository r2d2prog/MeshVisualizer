using CoreVisualizer.Interfaces;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using CoreVisualizer.Properties;

namespace CoreVisualizer
{
    public partial class RenderControl : UserControl , IRenderControl
    {
        private Dictionary<string, Model> Models { get; set; }

        private vec3 lastWorldPos;
        private Point lastMousePos;
        private Camera camera;
        private Grid grid;
        private DirectionalLight[] lights;

        public Model ActiveModel {  get; set; }
        public RenderHandler RenderHandler { get; private set; }

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
            ActiveModel = model;
        }

        public IEnumerable<string> GetModelNames() => Models.Keys;

        public Model GetModel(string name)
        {
            if(Models.ContainsKey(name)) 
                return Models[name];
            return null;
        }

        public void SetAntialiasing(bool isEnable)
        {
            if(isEnable)
                Gl.Enable(EnableCap.Multisample);
            else
                Gl.Disable(EnableCap.Multisample);
        }

        private void CreateShaderProgramFromResource(string key, string vertexSource, string fragmentSource)
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromString(ShaderType.VertexShader, vertexSource);
            program.CreateShaderFromString(ShaderType.FragmentShader, fragmentSource);
            program.Link();
            RenderHandler.Programs.Add(key, program);
        }

        private void CreateShaderProgramFromArrays(string key, string[] vSource, string[] fSource)
        {
            var program = new ShaderProgramCreator();
            program.CreateShaderFromStringArray(ShaderType.VertexShader, vSource);
            program.CreateShaderFromStringArray(ShaderType.FragmentShader, fSource);
            program.Link();
            RenderHandler.Programs.Add(key, program);
        }

        private void CreateMeshMaterialProgram()
        {
            CreateShaderProgramFromResource("MeshMaterial", Resources.mesh_vs, Resources.mesh_fs);
            CreateShaderProgramFromResource("PrimitiveRasterization", Resources.primitive_vs, Resources.primitive_fs);
        }

        private void CreateMeshTexturedProgram()
        {
            var sbVertex = new StringBuilder(Resources.mesh_vs);
            var sbFragment = new StringBuilder(Resources.mesh_fs);
            sbVertex.Replace("#define USE_MATERIAL", "#define USE_TEXTURES");
            sbFragment.Replace("#define USE_MATERIAL", "#define USE_TEXTURES");

            var tsProgram = new ShaderProgramCreator();
            tsProgram.CreateShaderFromString(ShaderType.VertexShader, sbVertex.ToString());
            tsProgram.CreateShaderFromString(ShaderType.FragmentShader, sbFragment.ToString());
            tsProgram.Link();
            RenderHandler.Programs.Add("MeshTangentSpace", tsProgram);

            sbVertex.Replace("#define TANGENT_SPACE", "#define MODEL_SPACE");
            sbFragment.Replace("#define TANGENT_SPACE", "#define MODEL_SPACE");
            var msProgram = new ShaderProgramCreator();
            msProgram.CreateShaderFromString(ShaderType.VertexShader, sbVertex.ToString());
            msProgram.CreateShaderFromString(ShaderType.FragmentShader, sbFragment.ToString());
            msProgram.Link();
            RenderHandler.Programs.Add("MeshModelSpace", msProgram);
        }

        private void OnInit(object sender, EventArgs e)
        {
            glControl.MouseWheel += OnMouseWheel;
            glControl.Render += DrawScene;

            RenderHandler = new RenderHandler();
            Models = new Dictionary<string, Model>();

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);
            SetAntialiasing(true);

            camera = new Camera(new vec3(0.57735f, 0.57735f, 0.57735f), new vec3(0.0f, 0.0f, 0.0f), (float)glControl.Width / glControl.Height);
            grid = new Grid(Camera.AspectRatio);

            CreateShaderProgramFromResource("Grid", Resources.grid_vs, Resources.grid_fs);
            CreateShaderProgramFromResource("Arrows", Resources.arrows_vs, Resources.arrows_fs);
            CreateShaderProgramFromResource("Labels", Resources.labels_vs, Resources.labels_fs);
            CreateMeshMaterialProgram();
            CreateMeshTexturedProgram();

            Disposed += OnDisposed;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            grid?.Dispose();
            camera?.Dispose();
            foreach (var model in Models.Values)
                model.Dispose();
            foreach (var program in RenderHandler.Programs)
                program.Value.Dispose();
        }

        private void DrawScene(object sender, GlControlEventArgs args)
        {
            Gl.Viewport(0, 0, glControl.Width, glControl.Height);
            Gl.ClearColor(BackColor.R / 255f, BackColor.G / 255f, BackColor.B / 255f, BackColor.A / 255f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var model in Models)
            {
                model.Value.Draw(RenderHandler);
            }
            grid?.Draw(RenderHandler.Programs["Grid"]);
            camera?.DisplayAxises(RenderHandler.Programs["Arrows"], RenderHandler.Programs["Labels"]);
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
