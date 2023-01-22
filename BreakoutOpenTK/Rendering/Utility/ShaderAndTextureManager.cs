using System.Collections.Generic;
using BreakoutOpenTK.Rendering.Shaders;
using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Graphics.OpenGL4;

namespace BreakoutOpenTK.Rendering.Utility
{
    public class ShaderAndTextureManager
    {
        Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        Dictionary<string, Texture> textures = new Dictionary<string, Texture>();


        public ShaderAndTextureManager()
        {
        }
        
        public void DeleteAll()
        {
            foreach (Shader shader in shaders.Values)
            {
                GL.DeleteProgram(shader.ProgramId);
            }
            foreach (Texture texture in textures.Values)
            {
                GL.DeleteTexture(texture.TextureId);
            }
            shaders.Clear();
            textures.Clear();
        }
        
        public Shader GetShader(string name)
        {
            return shaders[name];
        }
        
        public Texture GetTexture(string name)
        {
            return textures[name];
        }
        
        public Shader LoadShader(string name, string vertexShaderPath, string fragmentShaderPath)
        {
            Shader shader = new(vertexShaderPath, fragmentShaderPath);
            shaders.Add(name, shader);
            return shader;
        }
        
        public Shader LoadShader (string name, string vertexShaderPath, string fragmentShaderPath, string geometryShaderPath)
        {
            Shader shader = new(vertexShaderPath, fragmentShaderPath, geometryShaderPath);
            shaders.Add(name, shader);
            return shader;
        }
        
        public Texture LoadTexture(string name, string texturePath)
        {
            Texture texture = new(texturePath);
            textures.Add(name, texture);
            return texture;
        }
    }
}