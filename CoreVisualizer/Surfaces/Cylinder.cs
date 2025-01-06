using GlmSharp;
using SharpGL;
using SharpGL.VertexBuffers;
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
    public class Cylinder : RotationSurface
    {
        public Cylinder(int slices, int stacks, float radius, float height, vec3 center, Color color) : base(slices, stacks, radius, height, center, color)
        {
        }
    }
}
