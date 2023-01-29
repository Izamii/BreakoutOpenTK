using BreakoutOpenTK.Rendering.Sprites;
using BreakoutOpenTK.Rendering.Textures;
using OpenTK.Mathematics;

namespace BreakoutOpenTK.Rendering.Level
{
    public class Ball {
        private Vector2 _position;
        private Vector2 _size;
        private Vector2 _velocity;
        private float _rotation = 0.0f;
        private Texture _texture;
        private Vector3 _color = new(1.0f, 1.0f, 1.0f);
        private bool _tethered = true;
        private bool _sticky = false;
        private bool _ghost = false;
        private float _gameWidth;


        public Ball(Vector2 position, Vector2 size, Vector2 velocity, Texture texture, float gameWidth)
        {
            _position = position;
            _size = size;
            _velocity = velocity;
            _texture = texture;
            _gameWidth = gameWidth;
        }
        
        public Vector2 UpdatePosition(float dt)
        {
            if (_tethered) return _position;
            _position += _velocity * dt;
            if (_position.X <= 0.0f)
            {
                _velocity.X = -_velocity.X;
                _position.X = 0.0f;
            }
            else if (_position.X + _size.X >= _gameWidth)
            {
                _velocity.X = -_velocity.X;
                _position.X = _gameWidth - _size.X;
            }
            if (_position.Y <= 0.0f)
            {
                _velocity.Y = -_velocity.Y;
                _position.Y = 0.0f;
            }
            return _position;
        }
        
        public void Reset(Vector2 position, Vector2 velocity)
        {
            _position = position;
            _velocity = velocity;
            _tethered = true;
        }
        
        public void Render(Sprite sprite)
        {
            sprite.Render(_position, _size, _rotation, _texture, _color);
        }

        public float GameWidth
        {
            get => _gameWidth;
            set => _gameWidth = value;
        }

        public bool Tethered
        {
            get => _tethered;
            set => _tethered = value;
        }

        public Vector3 Color
        {
            get => _color;
            set => _color = value;
        }

        public Texture Texture
        {
            get => _texture;
            set => _texture = value ?? throw new ArgumentNullException(nameof(value));
        }

        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
        
        public bool Ghost
        {
            get => _ghost;
            set => _ghost = value;
        }
        
        public bool Sticky
        {
            get => _sticky;
            set => _sticky = value;
        }

        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public Vector2 Size
        {
            get => _size;
            set => _size = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
    }
}