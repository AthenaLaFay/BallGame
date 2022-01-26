using Game2.Content.Controls;
using Game2.Content.States;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Game2
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            using (var game = new Polyball())
                game.Run();
                
        }


    }
#endif
}
