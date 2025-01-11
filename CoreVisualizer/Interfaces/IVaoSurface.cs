using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreVisualizer
{
    public interface IVaoSurface : IDisposable
    {
        uint[] EBO { get; set; }
        uint[] VAO { get; set; }
        uint[] VertexBuffer { get; set; }
        uint[] ColorBuffer { get; set; }
        int[] Indices { get; set; }
        mat4[] ModelMatrix { get; set; }
        void CreateVertexArray(int[] indices, float[] coords, float[] colors, float[] uvs, float[] normals, float[] tangents, int index = 0);
        //void Draw(ShaderProgramCreator program);
    }

    [Flags]
    public enum CreateFlags
    { 
        None,
        GenerateVertexArray,
        NoColor,
    }

    public enum FanTriangles
    {
        Up,
        Down,
    }
}
