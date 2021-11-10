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
            //List<string> l = new List<string>();
            //l.Add("a");
            //l.Add("b");
            //l.Add("c");

            //Display.PrintGuesses(l, 10);
            //Display.PrintWrongGuess("a");

            //Game g = new Game();
            //g.Initialise();
            //g.PromptGuess();

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
