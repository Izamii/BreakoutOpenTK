using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Mathematics;

namespace BreakoutOpenTK.Rendering.Levels
{
    public enum PowerUpType
    {
        SpeedUp,
        SpeedDown,
        Sticky,
        PassThrough,
        PadSizeIncrease,
        PadSizeDecrease,
        InstantKill,
    }
    public class PowerUp : Brick
    {
        public PowerUpType Type { get; }
        public float Duration { get; set;  } = -1f;

        public PowerUp(Vector2 position, Vector2 size, float rotation, Vector2 velocity, Texture texture,
                Vector3 color, bool isDestroyed, bool isIndestructible, PowerUpType type) 
                : base(position, size, rotation, velocity, texture, color, isDestroyed, isIndestructible)
        {
            Type = type;
        }
        
        public void UpdatePosition(float dt)
        {
            Position += Velocity * dt;
        }
    }
}