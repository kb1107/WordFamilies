using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace WordFamilies
{
    class Game
    {
        List<string> WordList { get; set; } // holds all possible words
        List<string> GuessedLetters { get; set; } // holds users guessed letters
        // difficulty level always holds easy for the moment. Will implement hard later.
        private string DifficultyLevel { get; set; } // holds either 'easy' or 'hard' - the level of difficulty chosen by the user.
        int WordLength { get; set; } // holds the number of letters of the word being guessed
        private int GuessesLeft { get; set; } // holds the number of guesses the user has left
        string CurrentWord { get; set; } // holds the state of the current word
        string LastGuess { get; set; } // holds the last guess entered by the user
        private bool GameOver { get; set; }

        public void Initialise()
        {
            GameOver = false;
            WordList = File.ReadAllLines("dictionary.txt").ToList();
            GuessedLetters = new List<string>();
            WordLength = new Random().Next(4, 13); // Randomise word length 4-12 characters
            GuessesLeft = 2 * WordLength; // user starts with 2x guesses to length of word
            DifficultyLevel = "easy"; // always easy for now. Will update later.

            for (int i = 0; i < WordLength; i++)
            {
                CurrentWord += "*"; // initialise un-guessed letters to asterisks
            }
        }

        public void PromptGuess()
        {
            Display.PromptGuess();

            string guess = Console.ReadLine().ToLower().Trim(); // get guess from user
            bool validGuess = guess.Length == 1 && !GuessedLetters.Contains(guess); // check validity of input
            
            if (!validGuess)
            {
                do
                {
                    if (guess.Length != 1)
                    {
                        //Display.PrintInvalidInput();
                        //Display.PromptGuess();
                        Console.WriteLine("Invalid input. Enter a single letter.");
                    }

                    if (GuessedLetters.Contains(guess))
                    {
                        Console.WriteLine("You have already guessed that letter. Try again.");
                    }

                    guess = Console.ReadLine().ToLower().Trim();
                    validGuess = guess.Length == 1 && !GuessedLetters.Contains(guess);

                } while (!validGuess);
            }

            LastGuess = guess; // update valid guess
        }

        public void PartitionWordList()
        {
            // Find largest family
            // Remove old words from word list
            // Update current word
            // if current word contains last guess
            //  display correct guess
            //  display word
            // else
            //  guesses left--
            //  display wrong guess
            //  display word

        }

        public void CheckGameStatus()
        {
            if (GuessesLeft < 1)
            {
                if (WordList.Count > 1)
                {
                    Display.PrintGameLost(WordList[0]);
                    GameOver = true;
                }
                else
                {
                    Display.PrintGameWon(WordList[0]);
                    GameOver = true;
                }
            }
            else
            {
                if (WordList.Count == 1)
                {
                    Display.PrintGameWon(WordList[0]);
                    GameOver = true;
                }
            }
        }

        public void MainLoop()
        {

            while (!GameOver)
            {
                Display.PrintGuesses(GuessedLetters, GuessesLeft);
                Display.PrintWordState(CurrentWord);
                PromptGuess();
                PartitionWordList();
                CheckGameStatus();
            }
        }

        public bool PlayAgain()
        {
            string guess = Console.ReadLine().ToLower().Trim(); // get guess from user
            bool validGuess = guess == "y" || guess == "n"; // check validity of input

            if (!validGuess)
            {
                do
                {
                    Display.PrintInvalidInput();
                    guess = Console.ReadLine().ToLower().Trim();

                } while (!validGuess);

            }

            return guess == "y";
        }
    }
}
