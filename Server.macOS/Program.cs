using System;

using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            while (true)
            {
                game.update();
                Thread.Sleep(1);
            }
        }
    }
}
