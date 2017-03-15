using System;
using vikebot;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game("zTxyW4q6T_vr"))
            {
                // Run our GameProcedure method till the game has finished
                try
                {
                    while (true)
                        GameProcedure(game.Player);
                }
                catch (GameEndedException)
                {
                    Console.WriteLine("Finished game! You can review your results in the web app!");
                }
            }
        }

        /// <summary>
        /// A method that we call as log as the game is running
        /// </summary>
        /// <param name="player">Our player instance</param>
        static void GameProcedure(Player player)
        {
            // Get the amount of players in our chunk
            int radar = player.Radar();

            // Get a 5x5 matrix of block types 
            BlockType[,] aroundMe = player.GetSurrounding();
            // If there is a player at our right we attack him { x = 3, y = 2 }
            if (aroundMe[3, 2] == BlockType.Opponent)
            {
                player.Rotate(Angle.Right);
                player.Attack();
            }
                

            player.Attack();

            // Count the players in the map
            int horizontal = player.Scout(Alignment.Horizontal);
            int vertical = player.Scout(Alignment.Vertical);

            // Move on block upwards
            player.Move(Direction.Forward);

            // Change the defend mode of our player
            player.Defend();
            player.Undefend();
        }
    }
}