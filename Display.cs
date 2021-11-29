using System;
using System.Collections.Generic;
using System.Text;

namespace WordFamilies
{
    static class Display
    {
        public static void PrintWordState(string word)
        {
            Console.WriteLine("\nWord: " + word);
        }

        public static void PrintGuesses(List<char> letters, int guesses)
        {
            Console.WriteLine("You have {0} guesses left.", guesses);
            Console.Write("Used letters: ");
            foreach (char letter in letters)
            {
                Console.Write(letter + " ");
            }
        }

        public static void PromptDifficultyLevel()
        {
            Console.WriteLine("Choose difficulty level...");
            Console.WriteLine("1 = EASY");
            Console.WriteLine("2 = HARD");
            Console.Write("Enter 1 or 2: ");

        }

        public static void PromptDebugMenu()
        {
            Console.WriteLine("Display Debug Menu?");
            Console.WriteLine("Enter 'y' for yes, any other input for no...");
        }

        public static void PromptGuess()
        {
            Console.Write("Enter guess: ");
        }

        public static void PrintAlreadyGuessedLetterError()
        {
            Console.WriteLine("You have already guessed that letter. Try again.");
        }

        public static void PrintGameWon(string word)
        {
            Console.WriteLine("You won! Congratulations! The word was: " + word);
        }

        public static void PrintGameLost(string word)
        {
            Console.WriteLine("You lose! The word was: " + word);
        }

        public static void PrintCorrectGuess(char guess, int occurences)
        {
            Console.WriteLine("Yes, the word contains {0} {1} time(s).\n", guess, occurences);
        }

        public static void PrintWrongGuess(char guess)
        {
            Console.WriteLine("Sorry, the word does not contain any {0}'s.\n", guess);
        }

        public static void PrintInvalidInput()
        {
            Console.WriteLine("Invalid Input. Try again: ");
        }

        public static void PrintPlayAgain()
        {
            Console.WriteLine("Enter 'y' to play again or any other input to quit.");
            Console.WriteLine("Play again?: ");
        }

        public static void PrintWordlistCount(int count)
        {
            Console.WriteLine("---DEBUG--- Wordlist Count = " + count);

        }

        public static void PrintWordFamilyCodesAndValues(string family, int count)
        {
            Console.WriteLine("Family Code: " + family + " Count: " + count);
        }

        public static void PrintWordFamilyChosen(string family)
        {
            Console.WriteLine("Family Chosen: " + family);
        }
    }
}
