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
using System.Drawing.Imaging;
using System.Reflection;
using SharpGL.SceneGraph;
using System.Windows.Forms;

namespace CoreVisualizer
{
    public class Model : IVaoSurface
    {
        private const int textureSlots = 7;
        private static OpenGL Gl = RenderControl.Gl;
        public uint[] EBO { get ; set ; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }
        public uint[] UvBuffer { get; set; }
        public uint[] NormalBuffer { get; set; }
        public uint[] TangentBuffer { get; set; }
        public int[] Indices { get; set; }
        public int[] Points { get; set; }
        public MeshTexture[][] Textures { get; set; }
        public MeshMaterial[] Materials { get; set; }
        public RasterizationMode[] RasterizationModes { get; set; }
        public mat4[] ModelMatrix { get; set; }
        public bool IsModelMatrixUniform { get; set; } = true;
        public BoundingBox[] BoundingBoxes { get; private set; }
        public string[] Names { get; private set; }
        public static bool AutoSize { get; set; } = true;
        public static Color4D LineColor { get; set; } = new Color4D(0.0f, 1.0f, 0.0f, 1.0f);
        public static Color4D PointColor { get; set; } = new Color4D(1.0f, 1.0f, 0.0f, 1.0f);
        public Model(string path)
        {
            var context = new AssimpContext();
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.CalculateTangentSpace);
            var filename = Path.GetFileNameWithoutExtension(path);

            VAO = new uint[scene.MeshCount];
            EBO = new uint[scene.MeshCount];
            VertexBuffer = new uint[scene.MeshCount];
            ColorBuffer = new uint[scene.MeshCount];
            UvBuffer = new uint[scene.MeshCount];
            NormalBuffer = new uint[scene.MeshCount];
            TangentBuffer = new uint[scene.MeshCount];
            Indices = new int[scene.MeshCount];
            Textures = new MeshTexture[scene.MeshCount][];
            Materials = new MeshMaterial[scene.MeshCount];
            RasterizationModes = new RasterizationMode[scene.MeshCount];
            ModelMatrix = new mat4[scene.MeshCount];
            Names = new string[scene.MeshCount];
            Points = new int[scene.MeshCount];
            BoundingBoxes = new BoundingBox[scene.MeshCount];
            

            Gl.GenVertexArrays(scene.MeshCount, VAO);
            Gl.GenBuffers(scene.MeshCount, EBO);
            Gl.GenBuffers(scene.MeshCount, VertexBuffer);
            Gl.GenBuffers(scene.MeshCount, ColorBuffer);
            Gl.GenBuffers(scene.MeshCount, UvBuffer);
            Gl.GenBuffers(scene.MeshCount, NormalBuffer);
            Gl.GenBuffers(scene.MeshCount, TangentBuffer);

            for (var i = 0; i < scene.MeshCount; ++i)
            {
                var mesh = scene.Meshes[i];
                ModelMatrix[i] = mat4.Identity;
                Names[i] = $"{filename}_{scene.Meshes[i].Name}_{i}";
                var indices = scene.Meshes[i].GetIndices();
                Indices[i] = indices.Length;
                Points[i] = scene.Meshes[i].Vertices.Count;
                var coords = ConvertCoords(scene.Meshes[i].Vertices, i);
                if (scene.MeshCount == 1 && AutoSize)
                {
                    var scaleFactor = Grid.WorldUnit / BoundingBoxes[0].MaxSize();
                    ModelMatrix[0] = ModelMatrix[0] * mat4.Scale(scaleFactor * 4);
                }
                var mI = mesh.MaterialIndex;
                var material = scene.Materials[mI];
                Materials[i] = new MeshMaterial(material);
                float[] uvs = null;
                float[] normals = null;
                float[] tangents = null;
                if(mesh.HasTextureCoords(0))
                    uvs = mesh.TextureCoordinateChannels[0].SelectMany(v => new float[] { v.X, v.Y }).ToArray();
                if(mesh.HasNormals)
                    normals = mesh.Normals.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();
                if(mesh.HasTangentBasis)
                    tangents = mesh.Tangents.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();
                CreateTextures(material, i, path);
                RasterizationModes[i] = RasterizationMode.Shaded;
                float[] colors = null;
                CreateVertexArray(indices, coords, colors, uvs, normals, tangents, i);
            }
        }

        public void CreateVertexArray(int[] indices, float[] coords, float[] colors, float[] uvs, 
                                      float[] normals, float[] tangents, int index = 0)
        {
            Gl.BindVertexArray(VAO[index]);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, EBO[index]);

            IntPtr intPtr = Marshal.AllocHGlobal(indices.Length * sizeof(int));
            Marshal.Copy(indices, 0, intPtr, indices.Length);
            Gl.BufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(int), intPtr, Gl.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, VertexBuffer[index]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, coords, Gl.GL_STATIC_DRAW);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

            if (colors != null)
            {
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, ColorBuffer[index]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, colors, Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(1);
                Gl.VertexAttribPointer(1, 4, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            if (uvs != null)
            {
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, UvBuffer[index]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, uvs, Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(2);
                Gl.VertexAttribPointer(2, 2, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            if (normals != null)
            {
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, NormalBuffer[index]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, normals, Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(3);
                Gl.VertexAttribPointer(3, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            if (tangents != null)
            {
                Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, TangentBuffer[index]);
                Gl.BufferData(Gl.GL_ARRAY_BUFFER, tangents, Gl.GL_STATIC_DRAW);

                Gl.EnableVertexAttribArray(4);
                Gl.VertexAttribPointer(4, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            }

            Gl.BindVertexArray(0);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, 0);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, 0);
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(ColorBuffer.Length, ColorBuffer);
            Gl.DeleteBuffers(VertexBuffer.Length, VertexBuffer);
            Gl.DeleteBuffers(UvBuffer.Length, UvBuffer);
            Gl.DeleteBuffers(NormalBuffer.Length, NormalBuffer);
            Gl.DeleteBuffers(EBO.Length, EBO);
            Gl.DeleteVertexArrays(VAO.Length, VAO);
            for (var i = 0; i < VAO.Length; ++i)
            {
                if (Textures[i] != null)
                    for (var j = 0; j < Textures[i].Length; ++j)
                        Textures[i][j].Dispose();
            }
        }

        public void Draw(Dictionary<string, ShaderProgramCreator> programs)
        {
            if (VAO != null)
            {
                for (var i = 0; i < VAO.Length; i++)
                {
                    if (VAO[i] != 0)
                        Draw(programs, i);
                }
            }
        }

        public void SetRasterizationMode(RasterizationMode mode, bool checkState)
        {
            if (checkState)
                for (var i = 0; i < RasterizationModes.Length; ++i)
                {
                    RasterizationModes[i] |= mode;
                    var b = 0;
                }
            else
                for (var i = 0; i < RasterizationModes.Length; ++i)
                {
                    RasterizationModes[i] &= ~mode;
                    var a = 0;
                }
        }

        private void Draw(Dictionary<string, ShaderProgramCreator> programs, int index)
        {
            var oldMaterial = Materials[index];
            Gl.BindVertexArray(VAO[index]);
            if ((RasterizationModes[index] & RasterizationMode.Shaded) != RasterizationMode.None)
            {
                ShaderProgramCreator program;
                if (Textures[index] == null)
                    program = programs["MeshMaterial"];
                else
                {
                    var nt = Textures[index].Where(v => v.UniformName == "normal").ToArray();
                    program = nt.Length > 0 ? programs["MeshTangentSpace"] : programs["MeshModelSpace"];
                }

                Gl.UseProgram(program.Program);
                PassMatrices(program, index);
                PassMaterial(program, index);

                program.SetUniform("lightDir", new float[] { 0, 0, -1 });
                program.SetUniform("lightColor", new float[] { 0.5f, 0.5f, 0.5f, 1f });
                program.SetUniform("viewPos", Camera.GetWorldPosition().ToArray());
                var normalMatrix = IsModelMatrixUniform ? new mat3(ModelMatrix[index]) : new mat3(ModelMatrix[index]).Inverse.Transposed;
                program.SetUniform("normalMatrix", normalMatrix.ToArray());
                if (Textures[index] != null)
                {
                    for (var j = 0; j < Textures[index].Length; ++j)
                    {
                        var texture = Textures[index][j];
                        if (texture.TextureId != null)
                            program.BindTexture(texture.UniformName, Gl.GL_TEXTURE_2D, texture.TextureId[0], texture.Slot);
                    }
                }
                Gl.PolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                Gl.DrawElements(Gl.GL_TRIANGLES, Indices[index], Gl.GL_UNSIGNED_INT, IntPtr.Zero);
            }
            if ((RasterizationModes[index] & RasterizationMode.Wireframe) != RasterizationMode.None)
                DrawInWireframeOrPointMode(programs["PrimitiveRasterization"], Gl.GL_LINE, index, LineColor);

            if ((RasterizationModes[index] & RasterizationMode.Points) != RasterizationMode.None)
                DrawInWireframeOrPointMode(programs["PrimitiveRasterization"], Gl.GL_POINT, index, PointColor);
            Materials[index] = oldMaterial;
        }

        private void SetDiffuseMaterial(int index, Color4D color)
        {
            var oldMaterial = Materials[index];
            Materials[index] = new MeshMaterial(oldMaterial.Shininess);
            Materials[index].SetColor(Materials[index].Diffuse, color);
        }

        private void DrawInWireframeOrPointMode(ShaderProgramCreator program, uint rasterMode, int index, Color4D color)
        {
            Gl.UseProgram(program.Program);
            Gl.PointSize(2.5f);
            PassMatrices(program, index);
            SetDiffuseMaterial(index, color);
            program.SetUniform("color", Materials[index].Diffuse);
            Gl.PolygonMode(Gl.GL_FRONT_AND_BACK, rasterMode);
            Gl.DrawElements(Gl.GL_TRIANGLES, Indices[index], Gl.GL_UNSIGNED_INT, IntPtr.Zero);
            Gl.PointSize(1.0f);
        }

        private void PassMatrices(ShaderProgramCreator program, int index)
        {
            program.SetUniform("projection", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());
            program.SetUniform("model", ModelMatrix[index].ToArray());
        }

        private void PassMaterial(ShaderProgramCreator program, int index)
        {
            program.SetUniform("ambientColor", /*Materials[index].Ambient*/new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
            program.SetUniform("diffuseColor", Materials[index].Diffuse);
            program.SetUniform("specularColor", Materials[index].Specular);
            program.SetUniform("emmisiveColor", Materials[index].Emissive);
            program.SetUniform("shinStrength", new float[] { Materials[index].Shininess });
        }

        private void CreateTextures(Material material, int meshIndex, string modelPath)
        {
            var textures = material.GetAllMaterialTextures();
            if (textures != null && textures.Length == 0)
            {
                if (MeshTexture.TryFindTexture)
                {
                    var path = FindTextureWithSameModelName(modelPath);
                    if (!string.IsNullOrEmpty(path))
                    {
                        var slot = CreateDiffuseSlot(path);
                        Textures[meshIndex] = new MeshTexture[1];
                        Textures[meshIndex][0] = new MeshTexture(slot, modelPath);
                    }
                }
            }
            else if (textures != null && textures.Length > 0)
            {
                Textures[meshIndex] = new MeshTexture[textureSlots];
                for (var i = 0; i < textures.Length; ++i)
                {
                    Textures[meshIndex][i] = new MeshTexture(textures[i], modelPath);
                }
            }
        }

        private string FindTextureWithSameModelName(string modelPath)
        {
            var filename = Path.GetFileNameWithoutExtension(modelPath);
            var modelExt = Path.GetExtension(modelPath);
            var folder = new DirectoryInfo(Path.GetDirectoryName(modelPath));
            var raster = new List<string>() { ".png", ".jpg", ".tiff", ".bmp", ".jpeg", ".tga" };
            var files = folder.GetFiles(filename + "*.*").Where(v => v.Extension != modelExt && raster.Contains(v.Extension)).ToArray();
            var status = files.Length > 0;
            return files.Length > 0 ? files[0].Name : string.Empty;
        }

        private TextureSlot CreateDiffuseSlot(string path)
        {
            var slot = new TextureSlot(path, TextureType.Diffuse, 0, TextureMapping.FromUV, 0, 0,
                                       TextureOperation.Multiply, TextureWrapMode.Wrap, TextureWrapMode.Wrap, 0);
            return slot;
        }

        private float[] ConvertCoords(List<Vector3D> vertices, int index)
        {
            float xMin = float.MaxValue;
            float yMin = float.MaxValue;
            float zMin = float.MaxValue;

            float xMax = float.MinValue;
            float yMax = float.MinValue;
            float zMax = float.MinValue;

            var coords = new float[vertices.Count * 3];
            for(var i = 0; i < vertices.Count; ++i)
            {
                var stride = i * 3;
                coords[stride] = vertices[i].X;
                coords[stride + 1] = vertices[i].Y;
                coords[stride + 2] = vertices[i].Z;

                xMin = Math.Min(xMin, coords[stride]);
                xMax = Math.Max(xMax, coords[stride]);

                yMin = Math.Min(yMin, coords[stride + 1]);
                yMax = Math.Max (yMax, coords[stride + 1]);

                zMin = Math.Min(zMin, coords[stride + 2]);
                zMax = Math.Max(zMax, coords[stride + 2]);
            }
            BoundingBoxes[index] = new BoundingBox(new vec3(xMin, yMax, zMax), new vec3(xMax, yMin, zMin));
            return coords;
        }

        private float[] CreateColors(Color color, int count)
        {
            var stride = count * 4;
            float[] colors = new float[stride];
            for (var i = 0; i < stride; i+=4)
            {
                colors[i] = color.R / 255f;
                colors[i + 1] = color.G / 255f;
                colors[i + 2] = color.B / 255f;
                colors[i + 3] = color.A / 255f;
            }
            return colors;
        }
    }


    public class NoDiffuse
    {
        public static string vertexNoDiffuse =
            "#version 330 core\n" +
            "layout (location = 0) in vec3 position;\n" +
            "layout (location = 1) in vec3 color;\n" +
            "layout (location = 2) in vec3 uvs;\n" +
            "layout (location = 3) in vec3 normal;\n" +
            "layout (location = 4) in vec3 tangent;\n" +
            "uniform mat4 view;\n" +
            "uniform mat4 model;\n" +
            "uniform mat4 projection;\n" +
            "void main()\n" +
            "{\n" +
            "   gl_Position = projection * view * model * vec4(position, 1.0);\n" +
            "}";

        public static string fragmentNoDiffuse =
            "#version 330 core\n" +
            "uniform vec4 color;\n" +
            "void main()\n" +
            "{\n" +
            "   gl_FragColor = color;\n" +
            "}";
        
    } 
}
