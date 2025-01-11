using CoreVisualizer.Properties;
using SharpGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gl = SharpGL.OpenGL;

namespace CoreVisualizer
{
    public class ShaderProgramCreator
    {
        private static Gl Gl = RenderControl.Gl;

        public uint Vertex { get; set; }

        public uint Fragment { get; set; }

        public uint Geometry { get; set; }

        public uint Program { get; private set; }

        public ShaderProgramCreator() { }

        public void Bind() => Gl.UseProgram(Program);

        public void Unbind() => Gl.UseProgram(0);

        public void BindTexture(string name, uint target, uint texId, uint texUnit)
        {
            var err = Gl.GetError();
            Gl.ActiveTexture(Gl.GL_TEXTURE0 + texUnit);
            Gl.BindTexture(target, texId);
            var id = Gl.GetUniformLocation(Program, name);
            Gl.Uniform1(id, (int)texUnit);
            err = Gl.GetError();
        }

        public void SetCustomAttributes(uint buffer, string variable)
        {
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, buffer);
            var location = (uint)Gl.GetAttribLocation(Program, variable);
            Gl.BindAttribLocation(Program, location, variable);
            Gl.EnableVertexAttribArray(location);
            Gl.VertexAttribPointer(location, 3, Gl.GL_FLOAT, false, 0, IntPtr.Zero);
            Gl.BindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }

        public void UnsetCustomAttributes(string variable)
        {
            var location = (uint)Gl.GetAttribLocation(Program, variable);
            Gl.DisableVertexAttribArray(location);
        }

        public void SetUniform(string name, float[] values)
        {
            var id = Gl.GetUniformLocation(Program, name);
            var count = values.Length;
            if (id == -1)
                return;
            if (count == 1)
                Gl.Uniform1(id, values[0]);
            else if (count == 2)
                Gl.Uniform2(id, values[0], values[1]);
            else if (count == 3)
                Gl.Uniform3(id, values[0], values[1], values[2]);
            else if (count == 4)
                Gl.Uniform4(id, values[0], values[1], values[2], values[3]);
            else if (count == 9)
                Gl.UniformMatrix3(id, 1, false, values);
            else
                Gl.UniformMatrix4(id, 1, false, values);//Передача матрицы в шейдер
        }

        public void CreateShaderFromFile(uint type, string path)
        {
            var data = new List<string>();
            using (var reader = new StreamReader(path))
            {
                data.Add(reader.ReadToEnd());
            }
            CreateShaderFromStringArray(type, data.ToArray());
        }

        public void CreateShaderFromStringArray(uint type, string[] data)
        {
            var source = string.Join("", data);
            CreateShaderFromString(type, source);
        }



        public void CreateShaderFromString(uint type, string source)
        {
            var shader = Gl.CreateShader(type);
            Gl.ShaderSource(shader, source);
            try
            {
                Gl.CompileShader(shader);
                var status = new int[1];
                Gl.GetShader(shader, Gl.GL_COMPILE_STATUS, status);
                if (status[0] == Gl.GL_FALSE)
                    throw new Exception();
                if (type == Gl.GL_VERTEX_SHADER)
                    Vertex = shader;
                else if (type == Gl.GL_FRAGMENT_SHADER)
                    Fragment = shader;
                else if (type == Gl.GL_GEOMETRY_SHADER)
                    Geometry = shader;
            }
            catch
            {
                var sb = new StringBuilder(1024);
                Gl.GetShaderInfoLog(shader, 1024, IntPtr.Zero, sb);
                if (MessageBox.Show(sb.ToString(), "Compile shader exception",
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    Environment.Exit(1);
            }
        }

        public void Link()
        {
            Program = Gl.CreateProgram();
            if (Vertex != 0)
                Gl.AttachShader(Program, Vertex);
            if (Fragment != 0)
                Gl.AttachShader(Program, Fragment);
            if (Geometry != 0)
                Gl.AttachShader(Program, Geometry);
            try
            {
                Gl.LinkProgram(Program);
                var status = new int[1];
                Gl.GetProgram(Program, Gl.GL_LINK_STATUS, status);
                if (status[0] == Gl.GL_FALSE)
                    throw new Exception();
            }
            catch
            {
                var sb = new StringBuilder(1024);
                Gl.GetProgramInfoLog(Program, 1024, IntPtr.Zero, sb);
                if (MessageBox.Show(sb.ToString(), "Link program exception",
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                    Environment.Exit(1);
            }
        }

        public void Dispose()
        {
            Gl.DetachShader(Program, Vertex);
            Gl.DeleteShader(Vertex);
            Gl.DetachShader(Program, Fragment);
            Gl.DeleteShader(Fragment);
            Gl.DetachShader(Program, Geometry);
            Gl.DeleteShader(Geometry);
            Gl.DeleteProgram(Program);
        }
    }
}
