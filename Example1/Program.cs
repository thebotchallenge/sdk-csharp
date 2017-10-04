using System;
using vikebot;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game("APuXKetgdgehcNvsEs"))
            {
                try
                {
                    game.Player.Name = "john";

                    while (true)
                    {
                        int radar = game.Player.Radar();
                        BlockType[,] enemies = game.Player.Watch();
                        BlockType[,] environment = game.Player.WatchEnvironment();
                        int scout = game.Player.Scout();

                        game.Player.Move(Direction.Forward);

                        game.Player.Rotate(Angle.Right);
                        game.Player.Attack();

                        game.Player.InDefend = true;
                        game.Player.InDefend = false;
                    }
                }
                catch (GameEndedException)
                {
                    Console.WriteLine("Finished game! You can review your results in the web app!");
                }
            }
        }
    }
}