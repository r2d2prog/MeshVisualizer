using GlmSharp;
using System.Drawing;

namespace CoreVisualizer
{
    public class Cylinder : RotationSurface
    {
        public Cylinder(int slices, int stacks, float radius, float height, vec3 center, Color color) : base(slices, stacks, radius, height, center, color)
        {
        }
    }
}
