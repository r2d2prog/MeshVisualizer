using CoreVisualizer;
using CoreVisualizer.Interfaces;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshVisualizer
{
    public class LightingControl
    {
        private const float scaleFactor = 1 / 23f;
        private static mat3 circleInverse = new mat3(vec3.UnitX, -vec3.UnitY, new vec3(23f, 23f, 1f));
        private static mat3 circleSpace = new mat3(vec3.UnitX, -vec3.UnitY, new vec3(-23f, 23f, 1f));

        private static RectangleF ellipseBound = new RectangleF(0f, 0f, 46f, 46f);

        private static vec3 center = new vec3(23f, 23f, 0f);

        private static vec2 xArrow = new vec2(46f, 23f);
        private static PointF[] xTip = new PointF[] { new PointF(46f, 23f), new PointF(43f, 21f), new PointF(43f, 25f) };

        private static vec2 zArrow = new vec2(23f, 46f);
        private static PointF[] zTip = new PointF[] { new PointF(23f, 46f), new PointF(25f, 43f), new PointF(21f, 43f) };

        private static Color backColor = Color.FromArgb(255, 240, 240, 240);

        private static Brush redDye = new SolidBrush(Color.Red);
        private static Brush greenDye = new SolidBrush(Color.Green);
        private static Brush blueDye = new SolidBrush(Color.Blue);
        
        private static Pen redPen = new Pen(Color.Red);
        private static Pen greenPen = new Pen(Color.Green);
        private static Pen bluePen = new Pen(Color.Blue);
        
        private vec3 lightDir2D;
        public bool IsPositiveHemisphere {  get; set; }
        public vec3 LightDirection3D { get; private set; }
        public LightingControl() 
        {
            lightDir2D = new vec3(23f, 0, -1);
            IsPositiveHemisphere = true;
            LightDirection3D = -vec3.UnitZ;
        }

        public bool CalculateLightDirection(vec3 mousePos)
        {
            vec3 circlePos = circleSpace * mousePos;
            var length = circlePos.Length;
            if (Math.Abs(length) > 1e-4)
            {
                CalculateLightDirection3D(circlePos, length);
                lightDir2D = circleInverse * circlePos;
                var center = new vec3(23f, 23f, 1);
                var dir = lightDir2D - center;
                var dirLength = dir.Length;
                if (dirLength > 23f)
                {
                    var t = 23f / dirLength;
                    lightDir2D = center + t * dir;
                }
                return true;
            }
            return false;
        }

        public void AlignDirection(ViewPlane direction)
        {
            switch (direction)
            {
                case ViewPlane.XY:
                    LightDirection3D = vec3.UnitZ;
                    lightDir2D = new vec3(23f, 46f, 0f);
                    IsPositiveHemisphere = true;
                    break;
                case ViewPlane.YX:
                    LightDirection3D = -vec3.UnitZ;
                    lightDir2D = new vec3(23f, 0f, 0f);
                    IsPositiveHemisphere = true;
                    break;
                case ViewPlane.ZX:
                    LightDirection3D = vec3.UnitY;
                    lightDir2D = new vec3(23f, 23f, 0f);
                    IsPositiveHemisphere = true;
                    break;
                case ViewPlane.XZ:
                    LightDirection3D = -vec3.UnitY;
                    lightDir2D = new vec3(23f, 23f, 0f);
                    IsPositiveHemisphere = false;
                    break;
                case ViewPlane.YZ:
                    LightDirection3D = vec3.UnitX;
                    lightDir2D = new vec3(46f, 23f, 0f);
                    IsPositiveHemisphere = true;
                    break;
                default:
                    LightDirection3D = -vec3.UnitX;
                    lightDir2D = new vec3(0f, 23f, 0f);
                    IsPositiveHemisphere = true;
                    break;
            }
        }

        public void ChangeLightHemisphere()
        {
            IsPositiveHemisphere = !IsPositiveHemisphere;
            LightDirection3D = new vec3(LightDirection3D.x, -LightDirection3D.y, LightDirection3D.z);
        }

        public void DrawLightDirection(Graphics graphics)
        {
            graphics.Clear(backColor);

            graphics.DrawEllipse(redPen, ellipseBound);

            graphics.DrawLine(redPen, center.x, center.y, xArrow.x, xArrow.y);
            graphics.FillPolygon(redDye, xTip);

            graphics.DrawLine(bluePen, center.x, center.y, zArrow.x, zArrow.y);
            graphics.FillPolygon(blueDye, zTip);

            graphics.DrawLine(greenPen, center.x, center.y, lightDir2D.x, lightDir2D.y);
            var tip = CreateDirectionTip();
            graphics.FillPolygon(greenDye, tip);

            var rect = new RectangleF(lightDir2D.x - 2, lightDir2D.y - 2, 4, 4);
            if (IsPositiveHemisphere)
                graphics.FillEllipse(new SolidBrush(Color.Green), rect);
            else
                graphics.DrawEllipse(new Pen(new SolidBrush(Color.Green)), rect);
        }

        private void CalculateLightDirection3D(vec3 circleSpacePos, float vecLength)
        {
            if (vecLength > 23f)
            {
                var t = 23f / vecLength;
                circleSpacePos = t * circleSpacePos;
            }
            var r = 23f * 23f;
            var xS = circleSpacePos.x * circleSpacePos.x;
            var yS = circleSpacePos.y * circleSpacePos.y;
            var z = (float)Math.Sqrt(r - xS - yS);
            z = IsPositiveHemisphere ? z : -z;
            LightDirection3D = new vec3(circleSpacePos.x, z, -circleSpacePos.y) * scaleFactor;
        }

        private PointF[] CreateDirectionTip()
        {
            var dir = lightDir2D - center;
            var length = 23f / dir.Length;
            var normal = new vec3(-dir.y, dir.x, dir.z);
            var p1 = normal * 0.12f * length;
            var p2 = -normal * 0.12f * length;
            p1 = center + dir * 0.12f * length;
            p2 = center + dir * 0.12f * length;
            p1 = p1 + normal * 0.12f * length;
            p2 = p2 - normal * 0.12f * length;
            var tip = new PointF[] { new PointF(center.x, center.y), new PointF(p1.x, p1.y), new PointF(p2.x, p2.y) };
            return tip;
        }
    }
}
