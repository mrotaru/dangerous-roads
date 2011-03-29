using System;

namespace DangerousRoads
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (DangerousRoads game = new DangerousRoads())
            {
                game.Run();
            }
        }
    }
}

