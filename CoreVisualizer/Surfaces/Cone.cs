using GlmSharp;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public class Cone : RotationSurface
    {
        public Cone(int slices, int stacks, float radius, float height, vec3 center, Color color) : base(slices, stacks, radius, height, center, color)
        {
        }

        public override void Create(CreateFlags flags = CreateFlags.GenerateVertexArray)
        {
            if (Slices < 3 || Stacks < 1)
                throw new ArgumentException("Аргумент slices меньше 3 или аргумент stacks меньше 1");

            var coords = new List<float>();
            var colors = new List<float>();
            var indices = new List<int>();

            CreateLateralSurface(coords);
            CreatePolusPoints(coords);

            CreateLateralIndices(indices, Stacks - 1, Slices - 1);
            CreatePolusIndices(indices);
            Indices = new int[1];
            Indices[0] = indices.Count;

            if((flags & CreateFlags.NoColor) != CreateFlags.NoColor)
                CreateColors(colors, Color, coords.Count / 3);

            SendEvents(indices, coords, colors, flags);
            if ((flags & CreateFlags.GenerateVertexArray) == CreateFlags.GenerateVertexArray)
                CreateVertexArray(indices.ToArray(), coords.ToArray(), colors.ToArray());
        }

        protected override void CreateLateralSurface(List<float> coords)
        {
            var stackStep = Height / Stacks;
            var step = LocalCenter.y - Height * 0.5f;
            var min = step;

            var cosCache = CreateTrigonometricCache(Math.Cos);
            var sinCache = CreateTrigonometricCache(Math.Sin);
            var oldRadius = Radius;
            var koef = Height / Radius;
            for (var i = 0; i < Stacks; ++i, step += stackStep)
            {
                Radius = (Height - (step - min)) / koef;
                for (var j = 0; j < Slices; ++j)
                {
                    coords.Add(LocalCenter.x + Radius * cosCache[j]);
                    coords.Add(step);
                    coords.Add(LocalCenter.z + Radius * sinCache[j]);
                }
            }
        }

        protected override void CreatePolusIndices(List<int> indices)
        {
            CreatePolusIndices(indices, Slices * Stacks, FanTriangles.Up);
            CreatePolusIndices(indices, Slices * Stacks + 1, FanTriangles.Down);
        }
    }
}
