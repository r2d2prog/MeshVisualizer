using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace CoreVisualizer
{
    public struct DirectionalLight
    {
        public bool IsEnable;
        public vec3 Direction;
        public vec4 Color;

        public DirectionalLight(vec3 direction, vec4 color)
        {
            Direction = direction;
            Color = color;
            IsEnable = true;
        }
    }
}
