using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using CoreVisualizer.Properties;
using GlmSharp;
using OpenGL;

namespace CoreVisualizer
{
    public class ArrowLabels : IVaoSurface
    {
        private struct BitmapData
        {
            public Bitmap Bitmap { get; set; }
            public byte[] Data { get; set; }
            public BitmapData(Bitmap bitmap)
            {
                Bitmap = bitmap;
                Data = null;
            }
        }
        private uint[] Texture {  get; set; }

        public uint[] EBO { get; set; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }
        public uint[] MatrixBuffer { get; set; }
        public int[] Indices { get; set; }
        public mat4[] ModelMatrix { get; set; }
        public ArrowLabels() 
        {
            var data = CreateBitmapData();
            SetupTexture(data);

            var sX = mat4.Scale(0.03f, 0.03f, 0.03f);

            var tX = mat4.Translate(new vec3(0.11f, -0.01f, 0.0f));
            var tY = mat4.Translate(new vec3(-0.011f, 0.1f, 0.0f));
            var tZ = mat4.Translate(new vec3(-0.011f, -0.011f, 0.11f));

            var xAxis = tX * sX;
            var yAxis = tY * sX;
            var zAxis = tZ * sX;

            ModelMatrix = new mat4[] { xAxis, yAxis, zAxis };

            var indices = CreateIndices();
            Indices = new int[1];
            Indices[0] = indices.Count;

            var coords = CreateCoords();
            CreateVertexArray(indices.ToArray(), coords.ToArray(), null, null, null, null);
        }
        public void Dispose()
        {
            if (MatrixBuffer != null)
                Gl.DeleteBuffers(MatrixBuffer);
            Gl.DeleteTextures(Texture);
            Gl.DeleteBuffers(VertexBuffer);
            Gl.DeleteBuffers(EBO);
            Gl.DeleteVertexArrays(VAO);
        }

        public void Draw(ShaderProgramCreator program)
        {
            if (VAO == null || VAO[0] == 0)
                return;
            var vp = new int[4];
            Gl.Get(GetPName.Viewport, vp);
            var koef = (float)vp[2] / vp[3];
            var oldProj = Camera.Projection;
            var oldView = Camera.View;
            var currentView = oldView;
            currentView.Column3 = new vec4(new vec3(0.11f, 0.11f, 0), 1);

            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);

            Camera.View = currentView;
            Camera.Projection = mat4.Ortho(0.0f, koef, 0.0f, 1f);
            program.BindTexture("labels", TextureTarget.ProxyTexture2dArray, Texture[0], 0);
            program.SetUniform("projection", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            Gl.DrawElementsInstanced(PrimitiveType.Triangles, Indices[0], DrawElementsType.UnsignedInt, IntPtr.Zero, ModelMatrix.Length);

            Gl.Disable(EnableCap.Blend);
            Camera.View = oldView;
            Camera.Projection = oldProj;
            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public void CreateVertexArray(int[] indices, float[] coords, float[] colors, float[] uvs, float[] normals, float[] tangents, int index = 0)
        {
            VAO = new uint[1];
            EBO = new uint[1];
            VertexBuffer = new uint[1];

            Gl.GenVertexArrays(VAO);
            Gl.GenBuffers(EBO);
            Gl.GenBuffers(VertexBuffer);

            Gl.BindVertexArray(VAO[0]);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, EBO[0]);

            IntPtr intPtr = Marshal.AllocHGlobal(indices.Length * sizeof(int));
            Marshal.Copy(indices, 0, intPtr, indices.Length);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(indices.Length * sizeof(int)), intPtr, BufferUsage.StaticDraw);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer[0]);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(coords.Length * sizeof(float)), coords, BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, 0, IntPtr.Zero);

            MatrixBuffer = new uint[1];
            Gl.GenBuffers(MatrixBuffer);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, MatrixBuffer[0]);
            var matrixSize = Marshal.SizeOf(typeof(mat4));
            var vec4Size = Marshal.SizeOf(typeof(vec4));
            var size = matrixSize * ModelMatrix.Length;

            var modelMatrices = ModelMatrix.SelectMany(v => v.ToArray()).ToArray();
            IntPtr modelPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(modelMatrices, 0, modelPtr, modelMatrices.Length);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)size, modelPtr, BufferUsage.StaticDraw);
            Marshal.FreeHGlobal(modelPtr);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 4, VertexAttribType.Float, false, matrixSize, IntPtr.Zero);
            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 4, VertexAttribType.Float, false, matrixSize, (IntPtr)(vec4Size * 1));
            Gl.EnableVertexAttribArray(3);
            Gl.VertexAttribPointer(3, 4, VertexAttribType.Float, false, matrixSize, (IntPtr)(vec4Size * 2));
            Gl.EnableVertexAttribArray(4);
            Gl.VertexAttribPointer(4, 4, VertexAttribType.Float, false, matrixSize, (IntPtr)(vec4Size * 3));
            Gl.VertexAttribDivisor(1, 1);
            Gl.VertexAttribDivisor(2, 1);
            Gl.VertexAttribDivisor(3, 1);
            Gl.VertexAttribDivisor(4, 1);

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private List<float> CreateCoords()
        {
            var coords = new List<float>() { 0.0f, 0.0f,  
                                             1.0f, 0.0f, 
                                             1.0f, 1.0f,
                                             0.0f, 1.0f
                                           };
            return coords;
        }

        private List<int> CreateIndices()
        {
            var indices = new List<int>() { 0, 1, 2, 0, 2, 3 };
            return indices;
        }

        private BitmapData[] CreateBitmapData()
        {
            var data = new BitmapData[] { new BitmapData(Resources.X_Axis),new BitmapData(Resources.Y_Axis), new BitmapData(Resources.Z_Axis) };
            for (var i = 0; i < data.Length; ++i)
            {
                var bitmap = data[i].Bitmap;
                var bitmapData = bitmap.LockBits(new Rectangle(0,0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                var length = bitmapData.Stride * bitmapData.Height;

                data[i].Data = new byte[length];
                Marshal.Copy(bitmapData.Scan0, data[i].Data, 0, length);
                bitmap.UnlockBits(bitmapData);
            }
            return data;
        }

        private void SetupTexture(BitmapData[] data)
        {
            Texture = new uint[1];
            Gl.GenTextures(Texture);
            
            Gl.BindTexture(TextureTarget.Texture2dArray, Texture[0]);
            Gl.TexImage3D(TextureTarget.Texture2dArray, 0, InternalFormat.Rgba, data[0].Bitmap.Width, data[0].Bitmap.Height, 
                          data.Length, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            for (var i = 0; i < data.Length; ++i)
            {
                IntPtr ptr = Marshal.AllocHGlobal(data[i].Data.Length * sizeof(byte));
                Marshal.Copy(data[i].Data, 0, ptr, data[i].Data.Length);
                Gl.TexSubImage3D(TextureTarget.Texture2dArray, 0, 0, 0, i, data[i].Bitmap.Width, data[i].Bitmap.Height, 1, 
                                 OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ptr);
                Marshal.FreeHGlobal(ptr);
            }
            Gl.TexParameterIi(TextureTarget.Texture2dArray, TextureParameterName.TextureWrapS, Gl.CLAMP);
            Gl.TexParameterIi(TextureTarget.Texture2dArray, TextureParameterName.TextureWrapT, Gl.CLAMP);
            Gl.TexParameterIi(TextureTarget.Texture2dArray, TextureParameterName.TextureMinFilter, Gl.NEAREST);
            Gl.TexParameterIi(TextureTarget.Texture2dArray, TextureParameterName.TextureMagFilter, Gl.NEAREST);
        }
    }
}
