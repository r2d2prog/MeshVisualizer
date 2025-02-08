using GlmSharp;
using System;

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
            Center = new vec3((LeftUpNear.x + rightDownFar.x) / 2, (LeftUpNear.y + rightDownFar.y) / 2, (LeftUpNear.z + rightDownFar.z) / 2);

            SizeX = rightDownFar.x - leftUpNear.x;
            SizeY = leftUpNear.y - rightDownFar.y;
            SizeZ = leftUpNear.z - rightDownFar.z;
        }

        public float MaxSize()
        {
            var maxSize = Math.Max(SizeX, SizeY);
            return Math.Max(maxSize, SizeZ);
        }

        public static BoundingBox Combine(Model model)
        {
            var min_X = float.MaxValue;
            var min_Y = float.MaxValue;
            var min_Z = float.MaxValue;
            var max_X = float.MinValue;
            var max_Y = float.MinValue;
            var max_Z = float.MinValue;
            var bbs = model.BoundingBoxes;
            var length = bbs.Length;
            for (var i = 0; i < length; ++i)
            {
                var lu = model.ModelMatrix[i] * new vec4(bbs[i].LeftUpNear,1);
                var rd = model.ModelMatrix[i] * new vec4(bbs[i].RightDownFar, 1);
                min_X = Math.Min(lu.x, min_X);
                min_Y = Math.Min(rd.y, min_Y);
                min_Z = Math.Min(rd.z, min_Z);

                max_X = Math.Max(rd.x, max_X);
                max_Y = Math.Max(lu.y, max_Y);
                max_Z = Math.Max(lu.z, max_Z);
            }
            var luRes = new vec3(min_X, max_Y, max_Z);
            var rdRes = new vec3(max_X, min_Y, min_Z);
            return new BoundingBox(luRes, rdRes);
        }

        /*
        public static vec4[] ExtractPoints(vec4 leftUpNear, vec4 rightDownFar)
        {
            var points = new vec4[8];

            points[0] = new vec4(leftUpNear.x, leftUpNear.y, leftUpNear.z, leftUpNear.w);
            points[1] = new vec4(rightDownFar.x, leftUpNear.y, leftUpNear.z, leftUpNear.w);
            points[2] = new vec4(rightDownFar.x, leftUpNear.y, rightDownFar.z, rightDownFar.w);
            points[3] = new vec4(leftUpNear.x, leftUpNear.y, rightDownFar.z, rightDownFar.w);

            points[4] = new vec4(leftUpNear.x, rightDownFar.y, leftUpNear.z, leftUpNear.w);
            points[5] = new vec4(rightDownFar.x, rightDownFar.y, leftUpNear.z, leftUpNear.w);
            points[6] = new vec4(rightDownFar.x, rightDownFar.y, rightDownFar.z, rightDownFar.w);
            points[7] = new vec4(leftUpNear.x, rightDownFar.y, rightDownFar.z, rightDownFar.w);
            return points;
        }*/
    }
}
