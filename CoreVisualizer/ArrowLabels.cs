﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoreVisualizer.Properties;
using GlmSharp;
using SharpGL;
using SharpGL.SceneGraph.Lighting;
using Gl = SharpGL.OpenGL;


namespace CoreVisualizer
{
    public class ArrowLabels : IVaoSurface
    {
        private static OpenGL Gl = RenderControl.Gl;

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
            /*
            Library = new Library();
            var fontsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var path = fontsFolderPath + @"\sserife.fon";
            Face = Library.NewFace(path, 0);
            */
            var data = CreateBitmapData();
            SetupTexture(data);

            //Face.Dispose();
            //Library.Dispose();

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
            CreateVertexArray(indices.ToArray(), coords.ToArray(), null);
        }
        public void Dispose()
        {
            /*if(!Face.IsDisposed)
                Face.Dispose();
            if(!Library.IsDisposed)
                Library.Dispose();*/
            if (MatrixBuffer != null)
                Gl.DeleteBuffers(1, MatrixBuffer);
            Gl.DeleteTextures(Texture.Length, Texture);
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
            currentView.Column3 = new vec4(new vec3(0.11f, 0.11f, 0), 1);

            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);

            Camera.View = currentView;
            Camera.Projection = mat4.Ortho(0.0f, koef, 0.0f, 1f);
            program.BindTexture("labels", Gl.GL_TEXTURE_2D_ARRAY, Texture[0], 0);
            program.SetUniform("projection", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());

            Gl.Enable(Gl.GL_BLEND);
            Gl.BlendFunc((int)Gl.GL_SRC_ALPHA, (int)Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.DrawElementsInstanced(Gl.GL_TRIANGLES, Indices[0], Gl.GL_UNSIGNED_INT, IntPtr.Zero, ModelMatrix.Length);

            Gl.Disable(Gl.GL_BLEND);
            Camera.View = oldView;
            Camera.Projection = oldProj;
            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public void CreateVertexArray(int[] indices, float[] coords, float[] colors, int index = 0)
        {
            VAO = new uint[1];
            EBO = new uint[1];
            VertexBuffer = new uint[1];

            Gl.GenVertexArrays(1, VAO);
            Gl.GenBuffers(1, EBO);
            Gl.GenBuffers(1, VertexBuffer);

            Gl.BindVertexArray(VAO[0]);
            Gl.BindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, EBO[0]);

            IntPtr intPtr = Marshal.AllocHGlobal(indices.Length * sizeof(int));
            Marshal.Copy(indices, 0, intPtr, indices.Length);
            Gl.BufferData(Gl.GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(int), intPtr, Gl.GL_STATIC_DRAW);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, VertexBuffer[0]);
            Gl.BufferData(Gl.GL_ARRAY_BUFFER, coords.ToArray(), Gl.GL_STATIC_DRAW);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 2, Gl.GL_FLOAT, false, 0, IntPtr.Zero);

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



                /*
                Face.LoadChar(data[i].Label, LoadFlags.Default, LoadTarget.Normal);

                var bmp = Face.Glyph.Bitmap;
                data[i].Width = bmp.Width;
                data[i].Height = bmp.Rows;
                var length = bmp.Width * bmp.Rows;
                data[i].Data = new byte[length];

                var src = bmp.BufferData;
                for (int j = 0; j < bmp.Rows; ++j)
                {
                    var stride = j * bmp.Width;
                    if (stride >= length)
                        break;
                    for (var k = 0; k < bmp.Width; k += 8)
                    {
                        var curByteIndex = Math.Max(j * 8, j * k) >> 3;
                        var pixels = (int)src[curByteIndex];
                        var mask = 0XFF;
                        for (var l = 0; pixels != 0; ++l)
                        {
                            var curBit = 7 - l;
                            data[i].Data[stride + k + l] = (byte)(((pixels >> curBit) & 0x1) * 255);
                            mask >>= 1;
                            pixels &= mask;
                        }
                    }
                }*/
            }
            return data;
        }

        private void SetupTexture(BitmapData[] data)
        {
            Texture = new uint[1];
            Gl.GenTextures(1, Texture);
            
            Gl.BindTexture(Gl.GL_TEXTURE_2D_ARRAY, Texture[0]);
            Gl.TexImage3D(Gl.GL_TEXTURE_2D_ARRAY, 0, (int)Gl.GL_RGBA, data[0].Bitmap.Width, data[0].Bitmap.Height, data.Length, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, IntPtr.Zero);
            for (var i = 0; i < data.Length; ++i)
            {
                IntPtr ptr = Marshal.AllocHGlobal(data[i].Data.Length * sizeof(byte));
                Marshal.Copy(data[i].Data, 0, ptr, data[i].Data.Length);
                Gl.TexSubImage3D(Gl.GL_TEXTURE_2D_ARRAY, 0, 0, 0, i, data[i].Bitmap.Width, data[i].Bitmap.Height, 1, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, ptr);
                Marshal.FreeHGlobal(ptr);
            }
            Gl.TexParameter(Gl.GL_TEXTURE_2D_ARRAY, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.TexParameter(Gl.GL_TEXTURE_2D_ARRAY, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
            Gl.TexParameter(Gl.GL_TEXTURE_2D_ARRAY, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
            Gl.TexParameter(Gl.GL_TEXTURE_2D_ARRAY, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
        }
    }
}
