using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreVisualizer
{
    public class BoundingBox
    {
        public vec3 LeftUpNear {  get; private set; }
        public vec3 RightDownFar { get; private set; }
        public vec3 Center { get; private set; }
        public float SizeX { get; private set; }
        public float SizeY { get; private set; }
        public float SizeZ { get; private set; }
        public BoundingBox(vec3 leftUpNear, vec3 rightDownFar) 
        {
            LeftUpNear = leftUpNear;
            RightDownFar = rightDownFar;
            var dir = rightDownFar - leftUpNear;
            Center = dir * 0.5f;

            SizeX = rightDownFar.x - leftUpNear.x;
            SizeY = leftUpNear.y - rightDownFar.y;
            SizeZ = leftUpNear.z - rightDownFar.z;
        }

        public float MaxSize()
        {
            var maxSize = Math.Max(SizeX, SizeY);
            return Math.Max(maxSize, SizeZ);
        }
    }
}
