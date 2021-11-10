﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WordFamilies
{
    static class Display
    {
        public static void PrintWordState(string word)
        {
            Console.WriteLine("Word: " + word);
        }

        public static void PrintGuesses(List<string> letters, int guesses)
        {
            Console.WriteLine("You have {0} guesses left.", guesses);
            Console.Write("Used letters: ");
            foreach (string letter in letters)
            {
                Console.Write(letter + " ");
            }
        }

        public static void PromptGuess()
        {
            Console.WriteLine("Enter guess: ");
        }

        public static void PrintGameWon(string word)
        {
            Console.WriteLine("You won! Congratulations! The word was: " + word);
        }

        public static void PrintGameLost(string word)
        {
            Console.WriteLine("You lose! The word was: " + word);
        }

        public static void PrintCorrectGuess(string guess, int occurences)
        {
            Console.WriteLine("Yes, the word contains {0} {1} times.", guess, occurences);
        }

        public static void PrintWrongGuess(string guess)
        {
            Console.WriteLine("Sorry, the word does not contain any {0}'s.", guess);
        }

        public static void PrintInvalidInput()
        {
            Console.WriteLine("Invalid Input. Try again: ");
        }

        public static void PrintPlayAgain()
        {
            Console.WriteLine("Play again?");
            Console.Write("Enter y or n: ");

        }
    }
}