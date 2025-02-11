﻿using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenGL;

namespace CoreVisualizer
{
    public abstract class RotationSurface : IVaoSurface
    {
        protected int Slices { get; private set; }
        protected int Stacks { get; private set; }
        protected vec3 LocalCenter { get; set; }
        public float Radius { get; protected set; }
        public float Height { get; protected set; }
        public Color Color { get; protected set; }
        public mat4[] ModelMatrix { get; set; }

        public uint[] EBO { get; set; }
        public uint[] VAO { get; set; }
        public uint[] VertexBuffer { get; set; }
        public uint[] ColorBuffer { get; set; }

        public int[] Indices { get; set; }

        public event Action<List<int>> getIndicesArray;
        public event Action<List<float>> getCoordsArray;
        public event Action<List<float>> getColorsArray;
        public RotationSurface(int slices, int stacks, float radius, float height, vec3 center, Color color)
        {
            Slices = slices;
            Stacks = stacks;
            Radius = radius;
            Height = height;
            LocalCenter = center;
            Color = color;
            ModelMatrix = new mat4[] { mat4.Identity };
        }

        public void Dispose()
        {
            Gl.DeleteBuffers(ColorBuffer);
            Gl.DeleteBuffers(VertexBuffer);
            Gl.DeleteBuffers(EBO);
            Gl.DeleteVertexArrays(VAO);
        }

        public virtual void Create(CreateFlags flags = CreateFlags.GenerateVertexArray)
        {
            if (Slices < 3 || Stacks < 1)
                throw new ArgumentException("Аргумент slices меньше 3 или аргумент stacks меньше 1");

            var coords = new List<float>();
            var colors = new List<float>();
            var indices = new List<int>();

            CreateLateralSurface(coords);
            CreatePolusPoints(coords);

            CreateLateralIndices(indices,Stacks, Slices - 1);
            CreatePolusIndices(indices);
            Indices = new int[1];
            Indices[0] = indices.Count;
            if ((flags & CreateFlags.NoColor) != CreateFlags.NoColor)
                CreateColors(colors, Color, coords.Count / 3);
            SendEvents(indices, coords, colors, flags);
            if ((flags & CreateFlags.GenerateVertexArray) == CreateFlags.GenerateVertexArray)
                CreateVertexArray(indices.ToArray(), coords.ToArray(), colors.ToArray(), null, null, null);
        }

        public virtual void Draw(ShaderProgramCreator program)
        {
            if (VAO == null || VAO[0] == 0)
                return;
            Gl.UseProgram(program.Program);
            Gl.BindVertexArray(VAO[0]);

            program.SetUniform("perspective", Camera.Projection.ToArray());
            program.SetUniform("view", Camera.View.ToArray());
            program.SetUniform("model", ModelMatrix[0].ToArray());

            Gl.DrawElements(PrimitiveType.Triangles, Indices[0], DrawElementsType.UnsignedInt, IntPtr.Zero);

            Gl.BindVertexArray(0);
            Gl.UseProgram(0);
        }

        public void CreateVertexArray(int[] indices, float[] coords, float[] colors, float[] uvs, float[] normals, float[] tangents, int index = 0)
        {
            VAO = new uint[1];
            EBO = new uint[1];
            VertexBuffer = new uint[1];
            ColorBuffer = new uint[1];

            Gl.GenVertexArrays(VAO);
            Gl.GenBuffers(EBO);
            Gl.GenBuffers(VertexBuffer);
            Gl.GenBuffers(ColorBuffer);

            Gl.BindVertexArray(VAO[0]);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, EBO[0]);

            IntPtr intPtr = Marshal.AllocHGlobal(indices.Length * sizeof(int));
            Marshal.Copy(indices, 0, intPtr, indices.Length);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)indices.Length * sizeof(int), intPtr, BufferUsage.StaticDraw);
            Marshal.FreeHGlobal(intPtr);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer[0]);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)coords.Length * sizeof(float), coords.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, 0, IntPtr.Zero);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, ColorBuffer[0]);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)colors.Length * sizeof(float), colors.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 4, VertexAttribType.Float, false, 0, IntPtr.Zero);

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected void CreatePolusPoints(List<float> coords)
        {
            var upPolus = new float[] { LocalCenter.x, LocalCenter.y + Height * 0.5f, LocalCenter.z };
            var downPolus = new float[] { LocalCenter.x, LocalCenter.y - Height * 0.5f, LocalCenter.z };
            coords.AddRange(upPolus);
            coords.AddRange(downPolus);
        }

        protected float[] CreateTrigonometricCache(Func<double, double> trigFun)
        {
            var trigCache = new float[Slices];
            var angleStep = 2 * Math.PI / Slices;
            var angle = 0d;
            for (var i = 0; i < Slices; ++i, angle += angleStep)
                trigCache[i] = (float)trigFun(angle);
            return trigCache;
        }

        protected void CreateLateralIndices(List<int> indices, int stacks, int slices)
        {
            var lastRow = 0;
            for (var i = 0; i < stacks; ++i)
            {
                var nextRow = (i + 1) * Slices;
                for (var j = 0; j < slices; ++j)
                {
                    indices.Add(lastRow + j);
                    indices.Add(lastRow + j + 1);
                    indices.Add(nextRow + j);

                    indices.Add(lastRow + j + 1);
                    indices.Add(nextRow + j + 1);
                    indices.Add(nextRow + j);
                }
                indices.Add(nextRow - 1);
                indices.Add(lastRow);
                indices.Add(nextRow + slices);

                indices.Add(lastRow);
                indices.Add(nextRow);
                indices.Add(nextRow + slices);
                lastRow = nextRow;
            }
        }

        protected void CreatePolusIndices(List<int> indices, int indexPolus, FanTriangles fan)
        {
            var slicesDec = Slices - 1;
            if (fan == FanTriangles.Up)
            {
                var first = indexPolus - Slices;
                for (var i = 0; i < slicesDec; ++i)
                {
                    indices.Add(first + i);
                    indices.Add(first + i + 1);
                    indices.Add(indexPolus);
                }
                indices.Add(first + slicesDec);
                indices.Add(first);
                indices.Add(indexPolus);
            }
            else
            {
                //indexPolus += 1;
                var first = 0;
                for (var i = 0; i < slicesDec; ++i)
                {
                    indices.Add(first + i);
                    indices.Add(indexPolus);
                    indices.Add(first + i + 1);
                }
                indices.Add(first + slicesDec);
                indices.Add(indexPolus);
                indices.Add(first);
            }
        }

        protected void CreateColors(List<float> colors, Color color, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                colors.Add(color.R / 255f);
                colors.Add(color.G / 255f);
                colors.Add(color.B / 255f);
                colors.Add(color.A / 255f);
            }
        }

        protected void SendEvents(List<int> indices, List<float> coords, List<float> colors, CreateFlags flags)
        {
            getIndicesArray?.Invoke(indices);
            getCoordsArray?.Invoke(coords);
            if ((flags & CreateFlags.NoColor) != CreateFlags.NoColor)
                getColorsArray?.Invoke(colors);
        }

        protected virtual void CreateLateralSurface(List<float> coords)
        {
            var stackPoints = Stacks + 1;
            var stackStep = Height / Stacks;
            var step = LocalCenter.y - Height * 0.5f;

            var cosCache = CreateTrigonometricCache(Math.Cos);
            var sinCache = CreateTrigonometricCache(Math.Sin);
            for (var i = 0U; i < stackPoints; ++i, step += stackStep)
            {
                for (var j = 0; j < Slices; ++j)
                {
                    coords.Add(LocalCenter.x + Radius * cosCache[j]);
                    coords.Add(step);
                    coords.Add(LocalCenter.z + Radius * sinCache[j]);
                }
            }
        }

        protected virtual void CreatePolusIndices(List<int> indices)
        {
            CreatePolusIndices(indices, Slices * (Stacks + 1), FanTriangles.Up);
            CreatePolusIndices(indices, Slices * (Stacks + 1) + 1, FanTriangles.Down);
        }
    }
}
