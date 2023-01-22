using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BreakoutOpenTK.Rendering.Camera;
using BreakoutOpenTK.Rendering.Levels;
using BreakoutOpenTK.Rendering.Shaders;
using BreakoutOpenTK.Rendering.Sprites;
using BreakoutOpenTK.Rendering.Textures;
using BreakoutOpenTK.Rendering.Utility;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BreakoutOpenTK.Gameloop
{
    enum GameStaus
    {
        Menu,
        Game,
        GameOver,
        Win
    }
    public class Game : GameWindow 
    {
        readonly ShaderAndTextureManager _shaderAndTextureManager;
        Dictionary<string, Level> _levels;
        private Level _currentLevel;
        readonly Camera2D _camera;
        Sprite _sprite;
        private Brick _player;
        private Vector2 _playerVelocity;
        private Vector2 _initialPlayerPosition;
        private Vector2 _playerSize;
        private Ball _ball;
        private float _initialDrag = 70;
        private float _currentDrag = 70;
        private int _lives = 3;
        GameStaus _gameStatus;
        Random _randomizer = new Random();
        

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (width, height), Title = title, WindowBorder = WindowBorder.Fixed, StartFocused = true,
            StartVisible = true
        })
        {
            _camera = new Camera2D(new Vector2((float)width/2, (float)height/2), new Vector2(width, height));
            _shaderAndTextureManager = new ShaderAndTextureManager();
            _gameStatus = GameStaus.Game;
        }

        protected override void OnLoad()
        {
            VSync = VSyncMode.Off;
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.Blend);
            //this is required for transparency
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            //load shaders and textures
            _shaderAndTextureManager.LoadShader("sprite", "Rendering/Shaders/shader.vert", "Rendering/Shaders/shader.frag").Use();
            _shaderAndTextureManager.GetShader("sprite").SetBool("tex", false);
            _shaderAndTextureManager.GetShader("sprite").SetMat4("projection",
                _camera.GetProjectionMatrix());
            _shaderAndTextureManager.LoadTexture("pickaxe", "Resources/Textures/pickaxe.png");
            _shaderAndTextureManager.LoadTexture("pickaxeD", "Resources/Textures/diamond_pickaxe.png");
            _shaderAndTextureManager.LoadTexture("bedrock", "Resources/Textures/bedrock.jpg");
            _shaderAndTextureManager.LoadTexture("iron", "Resources/Textures/iron.jpg");
            _shaderAndTextureManager.LoadTexture("gold", "Resources/Textures/gold.jpg");
            _shaderAndTextureManager.LoadTexture("cobble", "Resources/Textures/cobble.jpg");
            _shaderAndTextureManager.LoadTexture("dirt", "Resources/Textures/dirt.jpg");
            _shaderAndTextureManager.LoadTexture("background", "Resources/Textures/background.jpg");
            _shaderAndTextureManager.LoadTexture("player", "Resources/Textures/wood.jpg");
            _shaderAndTextureManager.LoadTexture("sizeUp", "Resources/Textures/size_inc.png");
            _shaderAndTextureManager.LoadTexture("sizeDown", "Resources/Textures/size_dec.png");
            _shaderAndTextureManager.LoadTexture("speedUp", "Resources/Textures/speedup.png");
            _shaderAndTextureManager.LoadTexture("speedDown", "Resources/Textures/speeddown.png");
            _shaderAndTextureManager.LoadTexture("passThrough", "Resources/Textures/passthrough.png");
            _shaderAndTextureManager.LoadTexture("sticky", "Resources/Textures/sticky.png");
            _shaderAndTextureManager.LoadTexture("death", "Resources/Textures/death.png");
            
            //load sprite
            _sprite = new Sprite(_shaderAndTextureManager.GetShader("sprite"));
            
            //load player
            _playerSize = new Vector2(80, 20);
            _playerVelocity = new Vector2(500, 0);
            _initialPlayerPosition = new Vector2(_camera.WindowSize.X / 2 - _playerSize.X,
                _camera.WindowSize.Y - _playerSize.Y - 20);
            _player = new Brick(_initialPlayerPosition, _playerSize, 0, _playerVelocity,
                _shaderAndTextureManager.GetTexture("player"), new Vector3(1, 1, 1),
                false, true);
            
            //load ball
            var ballSize = new Vector2(20, 20);
            
            _ball = new Ball(
                new Vector2(_camera.WindowSize.X/2 - _playerSize.X/2 - ballSize.X/2, _camera.WindowSize.Y - _playerSize.Y - 20 - ballSize.Y),
                ballSize, new Vector2(_currentDrag, -400), _shaderAndTextureManager.GetTexture("pickaxe"),
                _camera.WindowSize.X);
            
            //load levels
            Level item = new ("Rendering/Levels/TestLevel.json" ,_shaderAndTextureManager);
            _levels = new Dictionary<string, Level> {{"TestLevel", item}};
            _currentLevel = item;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            //clear the screen
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (_gameStatus == GameStaus.Game)
            {
                //render the background
                _sprite.Render(Vector2.Zero, new Vector2(Size.X, Size.Y), 0.0f,
                    _shaderAndTextureManager.GetTexture("background"), Vector3.One);
                //render the level
                _currentLevel.RenderLevel(_sprite);
                _player.Render(_sprite);
                _ball.Render(_sprite);
            }
            SwapBuffers();
        }

        private void ResetBall()
        {
            _ball.Position = new Vector2(_player.Position.X + _player.Size.X/2 - _ball.Size.X/2, _player.Position.Y - _ball.Size.Y);
            _ball.Velocity = new Vector2(_initialDrag, -400);
            _ball.Tethered = true;
        }
        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            //Update the time of the game
            GameTime.DeltaTime = (float)args.Time;
            GameTime.TotalTime += (float)args.Time;
            
            base.OnUpdateFrame(args);
            
            //update the ball
            _ball.UpdatePosition(GameTime.DeltaTime);
            ProcessCollisions();
            UpdatePowerUps();
            
            //check if the player lost a life
            if (_ball.Position.Y > _camera.WindowSize.Y)
            {
                _lives--;
                ResetBall();
            }
            
            //check if the player lost all lives
            if (_lives <= 0)
            {
                _currentLevel.Reset();
                _lives = 3;
                _player.Size = _playerSize;
                _player.Position = _initialPlayerPosition;
                _player.Color = Vector3.One;
                _ball.Texture = _shaderAndTextureManager.GetTexture("pickaxe");
                _ball.Ghost = false;
                _currentDrag = _initialDrag;
                ResetBall();
                //_gameStatus = GameStaus.GameOver;
            }
            
            //check for keyboard input
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                _gameStatus = _gameStatus switch
                {
                    GameStaus.Game => GameStaus.Menu,
                    GameStaus.Menu => GameStaus.Game,
                    _ => _gameStatus
                };
            }

            if (_gameStatus == GameStaus.Game)
            {
                if (KeyboardState.IsKeyDown(Keys.A))
                {
                    if (_player.Position.X >= 0)
                    {
                        _player.Position -= _player.Velocity * GameTime.DeltaTime;
                        if (_ball.Tethered)
                        {
                            _ball.Position -= _player.Velocity * GameTime.DeltaTime;
                        }
                    }
                }
                if (KeyboardState.IsKeyDown(Keys.D))
                {
                    if (_player.Position.X + _player.Size.X <= _camera.WindowSize.X)
                    {
                        _player.Position += _player.Velocity * GameTime.DeltaTime;
                        if (_ball.Tethered)
                        {
                            _ball.Position += _player.Velocity * GameTime.DeltaTime;
                        }
                    }
                }
                if (KeyboardState.IsKeyDown(Keys.Space))
                {
                    _ball.Tethered = false;
                }
            }
        }

        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            _shaderAndTextureManager.DeleteAll();
            
            base.OnUnload();
        }

        private void UpdatePowerUps()
        {
            int stickyCount = _currentLevel.PowerUps.Count(up => up.Type == PowerUpType.Sticky && up.Duration > 0);
            int passCount = _currentLevel.PowerUps.Count(up => up.Type == PowerUpType.PassThrough && up.Duration > 0);
            foreach (var powerUp in _currentLevel.PowerUps)
            {
                //update power up position
                powerUp.UpdatePosition(GameTime.DeltaTime);
                //check if the power up is past the player
                if (powerUp.Position.Y > _camera.WindowSize.Y)
                {
                    powerUp.IsDestroyed = true;
                }
                //check timed power ups
                if (powerUp.Duration > 0)
                {
                    powerUp.Duration -= GameTime.DeltaTime;
                    switch (powerUp.Type)
                    {
                        case PowerUpType.Sticky when powerUp.Duration <= 0:
                            if (stickyCount == 1)
                            {
                                _ball.Sticky = false;
                                _player.Color = Vector3.One;
                            }
                            break;
                        case PowerUpType.PassThrough when powerUp.Duration <= 0:
                            if (passCount == 1)
                            {
                                _ball.Ghost = false;
                                _ball.Texture = _shaderAndTextureManager.GetTexture("pickaxe");
                            }
                            break;
                    }
                }
            }
        }
        
        private static bool IsColliding(Brick brick, Vector2 position, Vector2 size)
        {
            // check for collision on the x axis
            bool collisionX = position.X + size.X >= brick.Position.X &&
                              position.X <= brick.Position.X + brick.Size.X;
            // check for collision on the y axis
            bool collisionY = position.Y + size.Y >= brick.Position.Y &&
                              position.Y <= brick.Position.Y + brick.Size.Y;
            // return true if there is a collision
            return collisionX && collisionY;
        }

        private void ProcessCollisions()
        {
            //check for collisions with the player
            if (IsColliding(_player, _ball.Position, _ball.Size) && !_ball.Tethered)
            {
                //used for sticky power up
                _ball.Tethered = _ball.Sticky;
                
                //calculate how far the ball is from the center of the paddle in percent
                float percent = Math.Abs((_ball.Position.X + _ball.Size.X / 2) - (_player.Position.X + _player.Size.X / 2)) / (_player.Size.X / 2);
                float amplifier = 2.0f;
                Vector2 oldVelocity = _ball.Velocity;
                //calculate the new velocity
                var newXVelocity = Math.Abs(_currentDrag) * percent * amplifier;
                // check from which direction the ball is coming from and set the new velocity accordingly
                if (oldVelocity.X < 0)
                {
                    newXVelocity = -newXVelocity;
                }
                
                var newYVelocity = -1 * Math.Abs(oldVelocity.Y);
                _ball.Velocity = new Vector2(newXVelocity, newYVelocity);
            }
            foreach (var brick in _currentLevel.Bricks.Where(brick => !brick.IsDestroyed).Where(brick => IsColliding(brick, _ball.Position, _ball.Size)))
            {
                // destroy the brick
                if (!brick.IsIndestructible)
                {
                    brick.IsDestroyed = true;
                    if (_randomizer.Next(0, 100) < 25)
                    { 
                        SpawnPowerUp(brick); 
                    }
                }
                
                if (_ball.Ghost && !brick.IsIndestructible)
                {
                    continue;
                }
                // check if the ball is on the left or right side of the brick
                var isLeft = _ball.Position.X + _ball.Size.X / 2 < brick.Position.X + brick.Size.X / 2;
                var isRight = _ball.Position.X + _ball.Size.X / 2 > brick.Position.X + brick.Size.X / 2;
                        
                // check if the ball is on the top or bottom side of the brick
                var isTop = _ball.Position.Y + _ball.Size.Y / 2 < brick.Position.Y + brick.Size.Y / 2;
                var isBottom = _ball.Position.Y + _ball.Size.Y / 2 > brick.Position.Y + brick.Size.Y / 2;
                        
                // check if the ball is on the top or bottom side of the brick
                if (isLeft)
                {
                    var ballVelocity = _ball.Velocity;
                    ballVelocity.X = -Math.Abs(ballVelocity.X);
                    _ball.Velocity = ballVelocity;
                }
                else if (isRight)
                {
                    var ballVelocity = _ball.Velocity;
                    ballVelocity.X = Math.Abs(ballVelocity.X);
                    _ball.Velocity = ballVelocity;
                }
                if (isTop)
                {
                    var ballVelocity = _ball.Velocity;
                    ballVelocity.Y = -Math.Abs(ballVelocity.Y);
                    _ball.Velocity = ballVelocity;
                }
                else if (isBottom)
                {
                    var ballVelocity = _ball.Velocity;
                    ballVelocity.Y = Math.Abs(ballVelocity.Y);
                    _ball.Velocity = ballVelocity;
                }
            }

            foreach (var powerUp in _currentLevel.PowerUps.Where(powerUp => !powerUp.IsDestroyed).Where(powerUp => IsColliding(powerUp, _player.Position, _player.Size)))
            {
                powerUp.IsDestroyed = true;
                switch (powerUp.Type)
                {
                    case PowerUpType.PadSizeDecrease:
                        _player.Size = new Vector2(_player.Size.X * 0.8f, _player.Size.Y);
                        break;
                    case PowerUpType.PadSizeIncrease:
                        _player.Size = new Vector2(_player.Size.X * 1.2f, _player.Size.Y);
                        break;
                    case PowerUpType.SpeedDown:
                        _ball.Velocity = new Vector2(_ball.Velocity.X * 0.6f, _ball.Velocity.Y * 0.6f);
                        _currentDrag *= 0.6f;
                        break;
                    case PowerUpType.SpeedUp:
                        _ball.Velocity = new Vector2(_ball.Velocity.X * 1.4f, _ball.Velocity.Y * 1.4f);
                        _currentDrag *= 1.4f;
                        break;
                    case PowerUpType.Sticky:
                        _ball.Sticky = true;
                        _player.Color = new Vector3(1, 0.5f, 1);
                        powerUp.Duration = 15f;
                        break;
                    case PowerUpType.InstantKill:
                        _lives = 0;
                        break;
                    case PowerUpType.PassThrough:
                        _ball.Ghost = true;
                        _ball.Texture = _shaderAndTextureManager.GetTexture("pickaxeD");
                        powerUp.Duration = 10f;
                        break;
                }
            }
        }
        
        private void SpawnPowerUp(Brick brick)
        {
            //check if the brick is a power up brick
            var randomValue = _randomizer.Next(0, 60);
            //spawn a random power up
            switch (randomValue)
            {
                case < 3:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 150),
                        _shaderAndTextureManager.GetTexture("death"), Vector3.One, false,
                        false, PowerUpType.InstantKill);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 3 and < 15:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 300),
                        _shaderAndTextureManager.GetTexture("sizeDown"), Vector3.One, false,
                        false, PowerUpType.PadSizeDecrease);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 15 and < 27:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 300),
                        _shaderAndTextureManager.GetTexture("speedDown"), Vector3.One, false,
                        false, PowerUpType.SpeedDown);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 27 and < 37:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 350),
                        _shaderAndTextureManager.GetTexture("sizeUp"), Vector3.One, false,
                        false, PowerUpType.PadSizeIncrease);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 37 and < 47:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 350),
                        _shaderAndTextureManager.GetTexture("speedUp"), Vector3.One, false,
                        false, PowerUpType.SpeedUp);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 47 and < 57:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 400),
                        _shaderAndTextureManager.GetTexture("sticky"), Vector3.One, false,
                        false, PowerUpType.Sticky);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
                case >= 57 and < 60:
                {
                    var powerUp = new PowerUp(brick.Position, brick.Size, 0, new Vector2(0, 450),
                        _shaderAndTextureManager.GetTexture("passThrough"), Vector3.One, false,
                        false, PowerUpType.PassThrough);
                    _currentLevel.PowerUps.Add(powerUp);
                    break;
                }
            }
        }
    }
}