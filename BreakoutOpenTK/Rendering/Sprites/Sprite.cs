using BreakoutOpenTK.Rendering.Shaders;
using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BreakoutOpenTK.Rendering.Sprites
{
    public class Sprite
    {
        private Shader _shader;
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        
        public Sprite(Shader shader)
        {
            _shader = shader;
            InitializeRenderData();
        }

        private void InitializeRenderData()
        {
            float[] vertices = { 
                // pos      // tex
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f, 
    
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f
            };
            
            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();
            
            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            var posIndex = _shader.GetAttribLocation("aPosition");
            var texIndex = _shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(posIndex, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posIndex);
            GL.VertexAttribPointer(texIndex, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(texIndex);
        }
        
        public void Render(Vector2 position, Vector2 size, float rotate, Texture texture, Vector3 color)
        {
            _shader.Use();
            _shader.SetVec3("color", color);
            
            // Prepare transformations
            Matrix4 trans = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
            Matrix4 scale = Matrix4.CreateScale(size.X, size.Y, 1.0f);
            Matrix4 rot = Matrix4.CreateRotationZ(rotate);
            Matrix4 rotationAdjustment = Matrix4.CreateTranslation(0.5f*size.X, 0.5f*size.Y, 0.0f);
            Matrix4 rotationReturn = Matrix4.CreateTranslation(-0.5f*size.X, -0.5f*size.Y, 0.0f);
            
            // multiplication order is important -> right to left starting with trans
            _shader.SetMat4("model",scale * rotationReturn * rot * rotationAdjustment * trans);
            texture.Bind(TextureUnit.Texture0);
            
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
        
        public void Clear()
        {
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteBuffer(_vertexBufferObject);
        }
    }
}