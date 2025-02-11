﻿using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using OpenGL;

namespace CoreVisualizer
{
    public class Arrows : IVaoSurface
    {
        public uint[] EBO { get; set; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }
        public uint[] MatrixBuffer { get; set; }
        public int[] Indices { get; set; }
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
            Indices = new int[1];
            Indices[0] = indices.Count;

            coords.AddRange(conCoords);
            CreateVertexArray(indices.ToArray(), coords.ToArray(), null, null, null, null);
        }

        public void Dispose()
        {
            if(ColorBuffer != null)
                Gl.DeleteBuffers(ColorBuffer);
            if (MatrixBuffer != null)
                Gl.DeleteBuffers(MatrixBuffer);
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
            currentView.Column3 = new vec4 (new vec3(0.11f,0.11f,0), 1); 

            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);

            Camera.View = currentView;
            Camera.Projection = mat4.Ortho(0.0f, koef, 0.0f, 1f);
            program.SetUniform("projection", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            Gl.DrawElementsInstanced(PrimitiveType.Triangles, Indices[0], DrawElementsType.UnsignedInt, IntPtr.Zero, ModelMatrix.Length);

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

            var indArray = indices.ToArray();
            IntPtr intPtr = Marshal.AllocHGlobal(indArray.Length * sizeof(int));
            Marshal.Copy(indArray, 0, intPtr, indArray.Length);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)indArray.Length * sizeof(int), intPtr, BufferUsage.StaticDraw);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer[0]);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)coords.Length * sizeof(float), coords.ToArray(), BufferUsage.StaticDraw);
            
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);

            if (colors != null)
            {
                ColorBuffer = new uint[1];
                Gl.GenBuffers(ColorBuffer);
                
                Gl.BindBuffer(BufferTarget.ArrayBuffer, ColorBuffer[0]);
                Gl.BufferData(BufferTarget.ArrayBuffer, (uint)colors.Length * sizeof(float), colors.ToArray(), BufferUsage.StaticDraw);

                Gl.EnableVertexAttribArray(1);
                Gl.VertexAttribPointer(1, 4, VertexAttribType.Float, false, 0, IntPtr.Zero);
            }

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
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
