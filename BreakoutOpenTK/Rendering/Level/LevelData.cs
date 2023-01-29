namespace BreakoutOpenTK.Rendering.Level
{
    public class LevelData
    {
        public LevelData(List<List<string>> tiles, int tileWidth, int tileHeight, string levelName)
        {
            Tiles = tiles;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            LevelName = levelName;
        }

        public List<List<string>> Tiles { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public string LevelName { get; set; }
        

    }
}