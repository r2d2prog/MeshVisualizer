using GlmSharp;
using SharpGL.SceneGraph.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreVisualizer
{
    public class Camera : IDisposable
    {
        private Arrows arrows;
        private ArrowLabels arrowsLabels;
        public static float MouseWheelSens { get; set; } = 1f;
        public static float MouseSensX { get; set; } = 2f;
        public static float MouseSensY { get; set; } = 2f;
        public static float Near { get; private set; } = 0.1f;
        public static float Far { get; private set; } = 100f;
        public static float FovY { get; private set; } = (float)Math.PI / 3;
        public static float AspectRatio { get; set; } = 1.0f;
        public static mat4 View { get; set; }
        public static mat4 Projection { get; set; }
        public static float Length { get; private set; }
        public static vec3 Target { get; set; }

        public bool ShowAxises { get; set; } = true;
        public Camera(vec3 position, vec3 target, float aspectRatio)
        {
            ChangePosition(position, target);
            ChangePerspectiveProjection((float)Math.PI / 3, aspectRatio, 0.1f, 100f);
            arrows = new Arrows();
            arrowsLabels = new ArrowLabels();
        }

        public void Dispose()
        {
            arrows?.Dispose();
            arrowsLabels?.Dispose();
        }

        public void DisplayAxises(ShaderProgramCreator arrowsProg, ShaderProgramCreator labelsProg)
        {
            if(ShowAxises)
            {
                arrows?.Draw(arrowsProg);
                arrowsLabels?.Draw(labelsProg);
            }
        }

        public void ChangePosition(vec3 position)
        {
            var direction = Target - position;
            Length = direction.Length;
            vec3 up = vec3.UnitY;
            direction = direction.Normalized;
            var dot = vec3.Dot(up, direction);
            if (1 - Math.Abs(dot) < 1e-4)
                up = Math.Sign(direction.y) == -1 ? vec3.UnitX : -vec3.UnitX;
            View = mat4.LookAt(position, Target, up);
        }

        public void ChangePosition(vec3 position, vec3 target)
        {
            Target = target;
            ChangePosition(position);
        }

        public static vec3 GetWorldPosition()
        {
            var vec = -View.Column3;
            var mat = View.Transposed;
            var result = mat * vec;
            return result.xyz;
        }

        public void ChangePerspectiveProjection(float fovy, float aspectRatio, float near, float far)
        {
            Projection = mat4.Perspective(fovy, aspectRatio, near, far);
            FovY = fovy;
            AspectRatio = aspectRatio;
            Near = near;
            Far = far;
        }

        public void Translate(float deltaX, float deltaY, float deltaZ)
        {
            if (Length + deltaZ < 0.1f || Length + deltaZ > 100f)
                return;
            Length += deltaZ;
            var translate = mat4.Translate(deltaX, deltaY, -deltaZ);
            View = translate * View;
        }

        public void Rotate(float deltaX, float deltaY, float deltaZ)
        {
            var mx = mat4.Rotate(deltaX, View.Row0.xyz);
            var my = mat4.Rotate(deltaY, vec3.UnitY);
            var negTarget = mat4.Translate(-Target);
            var target = mat4.Translate(Target);
            View = View * target * mx * my * negTarget;
        }

        public void SetViewPlane(ViewPlane plane)
        {
            vec3 newDir;
            switch (plane)
            {
                case ViewPlane.XY:
                    newDir = vec3.UnitZ;
                    break;
                case ViewPlane.ZX:
                    newDir = vec3.UnitY;
                    break;
                case ViewPlane.XZ:
                    newDir = -vec3.UnitY;
                    break;
                case ViewPlane.YZ:
                    newDir = vec3.UnitX;
                    break;
                case ViewPlane.ZY:
                    newDir = -vec3.UnitX;
                    break;
                default:
                    newDir = -vec3.UnitZ;
                    break;
            }
            var newPosition = Target + newDir * Length;
            ChangePosition(newPosition);
        }
    }
    public enum ViewPlane
    {
        XY,
        YX,
        ZX,
        XZ,
        YZ,
        ZY
    }
}
