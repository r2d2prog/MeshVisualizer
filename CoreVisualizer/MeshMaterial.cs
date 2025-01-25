using System.Runtime.InteropServices;
using Assimp;

namespace CoreVisualizer
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MeshMaterial
    {
        [FieldOffset(0)]
        public float[] Ambient;
        [FieldOffset(16)]
        public float[] Diffuse;
        [FieldOffset(32)]
        public float[] Specular;
        [FieldOffset(48)]
        public float[] Emissive;
        [FieldOffset(64)]
        public float[] Reflective;
        [FieldOffset(80)]
        public float Shininess;
        public MeshMaterial(Material material)
        {
            Ambient = new float[4];
            Diffuse = new float[4];
            Specular = new float[4];
            Emissive = new float[4];
            Reflective = new float[4];
            Shininess = material.Shininess;
            SetColor(Ambient, material.ColorAmbient);
            SetColor(Diffuse, material.ColorDiffuse);
            SetColor(Specular, material.ColorSpecular);
            SetColor(Emissive, material.ColorEmissive);
            SetColor(Reflective, material.ColorReflective);
        }
        
        public MeshMaterial(float shininess = 0)
        {
            Ambient = new float[4];
            Diffuse = new float[4];
            Specular = new float[4];
            Emissive = new float[4];
            Reflective = new float[4];
            Shininess = shininess;
        }

        public void SetColor(float[] data, Color4D color)
        {
            for(var i = 0; i < 4; ++i)
                data[i] = color[i];
        }

        public void SetColor(float[] dst, float[] src)
        {
            for (var i = 0; i < 4; ++i)
                dst[i] = src[i];
        }
    }
}
