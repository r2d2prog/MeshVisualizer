using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Assimp;
using OpenGL;

namespace CoreVisualizer
{
    public struct MeshTexture : IDisposable
    {
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
                Gl.DeleteTextures(TextureId);
                TextureId[0] = 0;
            }
        }

        private void CreateTexture(TextureSlot texture, string texturePath)
        {
            TextureId = new uint[1];
            Gl.GenTextures(TextureId);
            Gl.BindTexture(TextureTarget.Texture2d, TextureId[0]);

            var bitmap = new Bitmap(texturePath);
            if(FlipY)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            
            var formatData = DetectTextureFormat(bitmap.PixelFormat);
            var wrapU = DetectTextureWrapping(texture.WrapModeU);
            var wrapV = DetectTextureWrapping(texture.WrapModeV);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, formatData.Item1, bitmap.Width, bitmap.Height, 0, formatData.Item2, PixelType.UnsignedByte, bitmapData.Scan0);

            Gl.TexParameterIi(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, wrapU);
            Gl.TexParameterIi(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, wrapV);
            Gl.TexParameterIi(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameterIi(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            bitmap.UnlockBits(bitmapData);
        }

        private Tuple<InternalFormat, OpenGL.PixelFormat> DetectTextureFormat(System.Drawing.Imaging.PixelFormat format)
        {
            var result = Tuple.Create(OpenGL.InternalFormat.Rgba, OpenGL.PixelFormat.Rgb);
            switch (format)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    result = Tuple.Create(InternalFormat.Rgb, OpenGL.PixelFormat.Bgr);
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    result = Tuple.Create(InternalFormat.Red, OpenGL.PixelFormat.Red);
                    break;
                default:
                    result = Tuple.Create(InternalFormat.Rgba, OpenGL.PixelFormat.Bgra);
                    break;
            }
            return result;
        }

        private int DetectTextureWrapping(Assimp.TextureWrapMode mode)
        {
            var result = Gl.REPEAT;
            switch (mode)
            {
                case Assimp.TextureWrapMode.Clamp:
                    result = Gl.CLAMP_TO_EDGE;
                    break;
                case Assimp.TextureWrapMode.Mirror:
                    result = Gl.MIRRORED_REPEAT;
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
