using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BreakoutOpenTK.Rendering.Sprites;
using BreakoutOpenTK.Rendering.Utility;
using Newtonsoft.Json;
using OpenTK.Mathematics;


namespace BreakoutOpenTK.Rendering.Levels
{
    public class Level
    {
        public List<Brick> Bricks { get; } = new List<Brick>();
        public List<PowerUp> PowerUps { get; } = new List<PowerUp>();

        LevelData levelData;
        ShaderAndTextureManager shaderAndTextureManager;

        public Level(string levelPath, ShaderAndTextureManager shaderAndTextureManager)
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(File.ReadAllText(levelPath)) ?? throw new Exception("Level data is null");
            this.shaderAndTextureManager = shaderAndTextureManager;
            LoadLevel();
            
        }

        private void LoadLevel()
        {
            //Loop through the level data and create the bricks
            for (var x = 0; x < levelData.Tiles.Count; x++)
            {
                for (var y = 0; y < levelData.Tiles[x].Count; y++)
                {
                    switch (levelData.Tiles[x][y])
                    {
                        case "bedrock":
                            Bricks.Add(new Brick(new Vector2(y * levelData.TileWidth, x *levelData.TileHeight),
                                new Vector2(levelData.TileWidth,levelData.TileHeight), 0, Vector2.Zero,
                                shaderAndTextureManager.GetTexture("bedrock"), Vector3.One, false,
                                true));
                            break;
                        case "cobble":
                            Bricks.Add(new Brick(new Vector2(y * levelData.TileWidth, x *levelData.TileHeight),
                                new Vector2(levelData.TileWidth,levelData.TileHeight), 0, Vector2.Zero,
                                shaderAndTextureManager.GetTexture("cobble"), Vector3.One, false,
                                false));
                            break;
                        case "gold":
                            Bricks.Add(new Brick(new Vector2(y * levelData.TileWidth, x *levelData.TileHeight),
                                new Vector2(levelData.TileWidth,levelData.TileHeight), 0, Vector2.Zero,
                                shaderAndTextureManager.GetTexture("gold"), Vector3.One, false,
                                false));
                            break;
                        case "iron":
                            Bricks.Add(new Brick(new Vector2(y * levelData.TileWidth, x *levelData.TileHeight),
                                new Vector2(levelData.TileWidth,levelData.TileHeight), 0, Vector2.Zero,
                                shaderAndTextureManager.GetTexture("iron"), Vector3.One, false,
                                false));
                            break;
                        case "dirt":
                            Bricks.Add(new Brick(new Vector2(y * levelData.TileWidth, x *levelData.TileHeight),
                                new Vector2(levelData.TileWidth,levelData.TileHeight), 0, Vector2.Zero,
                                shaderAndTextureManager.GetTexture("dirt"), Vector3.One, false,
                                false));
                            break;
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (var brick in Bricks)
            {
                brick.IsDestroyed = false;
            }
            PowerUps.Clear();
        }
        
        public void RenderLevel(Sprite sprite)
        {
            foreach (var brick in Bricks.Where(brick => !brick.IsDestroyed))
            {
                brick.Render(sprite);
            }

            foreach (var powerUp in PowerUps.Where(powerUp => !powerUp.IsDestroyed))
            {
                powerUp.Render(sprite);
            }
        }
        
        public bool IsLevelComplete()
        {
            return Bricks.All(brick => brick.IsIndestructible || brick.IsDestroyed);
        }

        public string GetLevelName()
        {
            return levelData.LevelName;
        }
    }
}