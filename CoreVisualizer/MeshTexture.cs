using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public struct MeshTexture : IDisposable
    {
        private static OpenGL Gl = RenderControl.Gl;
        private static bool FlipY = true;
        public uint Slot { get; private set; }
        public uint[] TextureId { get; set; }
        public string UniformName { get; private set; }
        public static bool TryFindTexture = true;
        public MeshTexture(TextureSlot texture, string modelPath)
        {
            Slot = 0;
            TextureId = null;
            UniformName = string.Empty;
            
            var dir = Path.GetDirectoryName(modelPath);
            var fullTexturePath = dir + @"\" + texture.FilePath;
            if (File.Exists(fullTexturePath))
            {
                CreateTexture(texture, fullTexturePath);
                DetectTextureType(texture.TextureType);
            }
        }

        public void Dispose()
        {
            if (TextureId != null)
            {
                Gl.DeleteTextures(TextureId.Length, TextureId);
                TextureId[0] = 0;
            }
        }

        private void CreateTexture(TextureSlot texture, string texturePath)
        {
            TextureId = new uint[1];
            Gl.GenTextures(1, TextureId);
            Gl.BindTexture(Gl.GL_TEXTURE_2D, TextureId[0]);

            var bitmap = new Bitmap(texturePath);
            if(FlipY)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            //var length = bitmapData.Stride * bitmapData.Height;

            //var data = new byte[length];
            //Marshal.Copy(bitmapData.Scan0, data, 0, length);
            

            var formatData = DetectTextureFormat(bitmap.PixelFormat);
            var wrapU = DetectTextureWrapping(texture.WrapModeU);
            var wrapV = DetectTextureWrapping(texture.WrapModeV);

            Gl.TexImage2D(Gl.GL_TEXTURE_2D, 0, formatData.Item1, bitmap.Width, bitmap.Height, 0, formatData.Item2, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            Gl.TexParameter(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, wrapU);
            Gl.TexParameter(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, wrapV);
            Gl.TexParameter(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.TexParameter(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.BindTexture(Gl.GL_TEXTURE_2D, 0);
            bitmap.UnlockBits(bitmapData);
        }

        private Tuple<uint,uint> DetectTextureFormat(PixelFormat format)
        {
            Tuple<uint,uint> result = new Tuple<uint, uint>(Gl.GL_RGBA, Gl.GL_RGB);
            switch (format)
            {
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format24bppRgb:
                    result = new Tuple<uint, uint>(Gl.GL_RGB, Gl.GL_BGR);
                    break;
                case PixelFormat.Format8bppIndexed:
                    result = new Tuple<uint, uint>(Gl.GL_RED, Gl.GL_RED);
                    break;
                default:
                    break;
            }
            return result;
        }

        private uint DetectTextureWrapping(TextureWrapMode mode)
        {
            var result = Gl.GL_REPEAT;
            switch (mode)
            {
                case TextureWrapMode.Clamp:
                    result = Gl.GL_CLAMP_TO_EDGE;
                    break;
                case TextureWrapMode.Mirror:
                    result = Gl.GL_MIRRORED_REPEAT;
                    break;
                default:
                    break;
            }
            return result;
        }

        private void DetectTextureType(TextureType type)
        {
            switch (type)
            {
                case TextureType.Normals:
                    Slot = 1;
                    UniformName = "normal";
                    break;
                case TextureType.Specular:
                    Slot = 2;
                    UniformName = "specular";
                    break;
                case TextureType.Reflection:
                    Slot = 3;
                    UniformName = "reflection";
                    break;
                case TextureType.Height:
                    Slot = 4;
                    UniformName = "height";
                    break;
                case TextureType.Emissive: 
                    Slot = 5;
                    UniformName = "emmisive";
                    break;
                case TextureType.Shininess:
                    Slot = 6;
                    UniformName = "shininess";
                    break;
                default:
                    UniformName = "diffuse";
                    break;
            }
        }
    }
}
