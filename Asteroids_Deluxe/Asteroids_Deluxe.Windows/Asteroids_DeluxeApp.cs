using Xenko.Engine;

namespace Asteroids_Deluxe
{
    class Asteroids_DeluxeApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
