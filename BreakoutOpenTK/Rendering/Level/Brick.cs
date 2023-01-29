using BreakoutOpenTK.Rendering.Sprites;
using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Mathematics;

namespace BreakoutOpenTK.Rendering.Level
{
    public class Brick
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
        public Texture Texture { get; set; }
        
        public Vector3 Color { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsIndestructible { get; set; }

        public Brick(Vector2 position, Vector2 size, float rotation, Vector2 velocity, Texture texture, Vector3 color, bool isDestroyed, bool isIndestructible)
        {
            Position = position;
            Size = size;
            Rotation = rotation;
            Velocity = velocity;
            Texture = texture;
            Color = color;
            IsDestroyed = isDestroyed;
            IsIndestructible = isIndestructible;
        }
        
        public void Render(Sprite sprite)
        {
            sprite.Render(Position, Size, Rotation, Texture, Color);
        }

    }
}