using System.Collections.Generic;
using BreakoutOpenTK.Rendering.Shaders;
using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Graphics.OpenGL4;

// the following header file was used in order to create the ShaderAndTextureManager class:
// https://learnopengl.com/code_viewer_gh.php?code=src/7.in_practice/3.2d_game/0.full_source/resource_manager.h
namespace BreakoutOpenTK.Rendering.Utility
{
    public class ShaderAndTextureManager
    {
        Dictionary<string, Shader> shaders = new();
        Dictionary<string, Texture> textures = new();


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