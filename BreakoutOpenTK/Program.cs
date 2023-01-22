using System;
using BreakoutOpenTK.Gameloop;
using BreakoutOpenTK.Rendering.Levels;
using BreakoutOpenTK.Rendering.Utility;
using OpenTK.Windowing.Common;

namespace BreakoutOpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new(800, 600, "Mine-Out"))
            {
                game.Run();
            }

        }
    }
}