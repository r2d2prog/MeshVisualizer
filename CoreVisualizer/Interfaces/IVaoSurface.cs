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
        int Indices { get; set; }
        mat4[] ModelMatrix { get; set; }
        void CreateVertexArray(List<int> indices, List<float> coords, List<float> colors);
        void Draw(ShaderProgramCreator program);
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
