using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace BreakoutOpenTK.Rendering.Textures
{
    public class Texture
    {
        public int TextureId { get; }
        
        public Texture(string fileName)
        {
            // Load the image from the file
            //StbImage.stbi_set_flip_vertically_on_load(1);
            var image = ImageResult.FromStream(File.OpenRead(fileName), ColorComponents.RedGreenBlueAlpha);
            
            // Generate the texture
            TextureId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            //unbind texture as we are done initializing it
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        
        public void Bind(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
}