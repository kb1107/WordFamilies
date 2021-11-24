using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WordFamilies
{
    class Program
    {
        static void Main(string[] args)
        {
            bool sessionOpen = true;

            while (sessionOpen)
            {
                Game g = new Game();
                g.Initialise();
                g.MainLoop();
                sessionOpen = g.PlayAgain();
            }
        }
    }
}
