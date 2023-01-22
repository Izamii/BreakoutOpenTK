using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace BreakoutOpenTK.Rendering.Shaders
{
    public class Shader
    {
        string vertexShaderSource;
        string fragmentShaderSource;
        string geometryShaderSource;

        public int ProgramId { get; private set; }
        
        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            this.vertexShaderSource = vertexShaderSource;
            this.fragmentShaderSource = fragmentShaderSource;
            geometryShaderSource = "";
            Compile();
        }

        public Shader(string vertexShaderSource, string fragmentShaderSource, string geometryShaderSource)
        {
            this.vertexShaderSource = vertexShaderSource;
            this.fragmentShaderSource = fragmentShaderSource;
            this.geometryShaderSource = geometryShaderSource;
            Compile();
        }

        private void Compile()
        {
            int vertexShader, fragmentShader, geometryShader; 

            //create vertex shader
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText(vertexShaderSource));
            CompileShader(vertexShader);
            
            //create fragment shader
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(fragmentShaderSource));
            CompileShader(fragmentShader);
            
            if (geometryShaderSource != "")
            {
                //create geometry shader
                geometryShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShader, File.ReadAllText(geometryShaderSource));
                CompileShader(geometryShader);
                
                //create shader program and link shaders
                ProgramId = GL.CreateProgram();
                GL.AttachShader(ProgramId, vertexShader);
                GL.AttachShader(ProgramId, fragmentShader);
                GL.AttachShader(ProgramId, geometryShader);
                LinkProgram(ProgramId);
                
                //delete shaders after linking
                GL.DetachShader(ProgramId, vertexShader);
                GL.DetachShader(ProgramId, fragmentShader);
                GL.DetachShader(ProgramId, geometryShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
                GL.DeleteShader(geometryShader);
            }
            else
            {
                //create shader program and link shaders
                ProgramId = GL.CreateProgram();
                GL.AttachShader(ProgramId, vertexShader);
                GL.AttachShader(ProgramId, fragmentShader);
                LinkProgram(ProgramId);
                
                //delete shaders after linking
                GL.DetachShader(ProgramId, vertexShader);
                GL.DetachShader(ProgramId, fragmentShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
            }
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);
            if (status == 1) return;
            GL.GetShaderInfoLog(shader, out string info);
            throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{info}");
        }
        
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var success);
            if (success == 1) return;
            GL.GetProgramInfoLog(program, out string info);
            throw new Exception($"Error occurred whilst linking Program({program}).\n\n{info}");
        }
        
        public void Use()
        {
            GL.UseProgram(ProgramId);
        }
        
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(ProgramId, attribName);
        }
        
        public void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(ProgramId, name), value ? 1 : 0);
        }
        
        public void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(ProgramId, name), value);
        }
        
        public void SetFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(ProgramId, name), value);
        }
        
        public void SetVec2(string name, float x, float y)
        {
            GL.Uniform2(GL.GetUniformLocation(ProgramId, name), x, y);
        }
        
        public void SetVec2(string name, Vector2i value)
        {
            GL.Uniform2(GL.GetUniformLocation(ProgramId, name), value.X, value.Y);
        }
        
        public void SetVec2(string name, Vector2 value)
        {
            GL.Uniform2(GL.GetUniformLocation(ProgramId, name), value.X, value.Y);
        }
        
        public void SetVec3(string name, float x, float y, float z)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), x, y, z);
        }
        
        public void SetVec3(string name, Vector3i value)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), value.X, value.Y, value.Z);
        }
        
        public void SetVec3(string name, Vector3 value)
        {
            GL.Uniform3(GL.GetUniformLocation(ProgramId, name), value.X, value.Y, value.Z);
        }
        
        public void SetVec4(string name, float x, float y, float z, float w)
        {
            GL.Uniform4(GL.GetUniformLocation(ProgramId, name), x, y, z, w);
        }
        
        public void SetVec4(string name, Vector4i value)
        {
            GL.Uniform4(GL.GetUniformLocation(ProgramId, name), value.X, value.Y, value.Z, value.W);
        }
        
        public void SetVec4(string name, Vector4 value)
        {
            GL.Uniform4(GL.GetUniformLocation(ProgramId, name), value.X, value.Y, value.Z, value.W);
        }
        
        public void SetMat4(string name, Matrix4 value)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ProgramId, name), false, ref value);
        }
    }
}