using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace WordFamilies
{
    class WordFamilies
    {
        private List<string> wordList; // holds the complete list of possible words
        private Dictionary<string, int> wordFamilies; // holds the various word families
        private List<string> usedLetters; // holds the letters guessed by the user
        private int wordLength; // holds the length of the current word
        private int guessesLeft; // holds the number of guesses the user has left
        private string currentWord; // holds the state of the current word e.g. '---e-'

        public void PlayGame()
        {
            bool appOpen = true;
            bool gameOver = false;

            while (appOpen)
            {
                Initialise();

                while (!gameOver)
                {
                    PrintGameStatus();

                    string guess = GetUserGuess();
                    usedLetters.Add(guess);
                    guessesLeft--;

                    // Update the word family populations
                    wordFamilies = GetWordFamilies(guess);
                    // Update current word list
                    wordList = GetRemainingWords(guess);
                    //update word status
                    currentWord = GetLargestFamily();
                    //if '-' not in currentWord
                    //  WIN!
                    //  print answer
                    //  Play again?

                    //if guessesLeft=0 & gameOver=False
                    //  gameOver = true
                    //  LOSE!
                    //  print answer
                    //  play again?
                    if (!currentWord.Contains('-'))
                    {
                        gameOver = true;
                        Console.WriteLine("You won! Congratulations!");
                        Console.WriteLine("The word was {0}", currentWord);
                    }
                    else if (guessesLeft == 0 && !gameOver)
                    {
                        gameOver = true;
                        Console.WriteLine("You lose!");
                        Console.WriteLine("The word was {0}", currentWord);
                    }

                    //TODO add play again feature.
                }
            }
        }

        public void Initialise()
        {
            List<string> words = File.ReadAllLines("dictionary.txt").ToList(); // Holds all possible words
            wordList = new List<string>();
            wordLength = new Random().Next(4, 13); // Random word length between 4 and 12 characters
            guessesLeft = 10; // User starts with 10 guesses
            usedLetters = new List<string>();
            wordFamilies = new Dictionary<string, int>();
            foreach (string word in words)
            {
                if (word.Length == wordLength)
                {
                    wordList.Add(word);
                }
            }

            // Initialise status of current word to correct number of blanks
            for (int i = 0; i < wordLength; i++)
            {
                currentWord += "-";
            }
        }

        public void PrintGameStatus()
        {
            Console.WriteLine("You have {0} guesses left", guessesLeft);
            Console.WriteLine("Used letters: ");
            foreach (string letter in usedLetters)
            {
                Console.Write(letter + " ");
            }

            Console.WriteLine("\nWord: {0}", currentWord);
        }

        public string GetUserGuess()
        {
            Console.WriteLine("Enter guess: ");

            string guess = Console.ReadLine().ToLower().Trim();

            bool validGuess = guess.Length == 1 && !usedLetters.Contains(guess);

            if (!validGuess)
            {
                do
                {
                    if (guess.Length != 1)
                    {
                        Console.WriteLine("Invalid input. Enter a single letter.");
                    }

                    if (usedLetters.Contains(guess))
                    {
                        Console.WriteLine("You have already guessed that letter. Try again.");
                    }

                    guess = Console.ReadLine().ToLower().Trim();
                    validGuess = guess.Length == 1 && !usedLetters.Contains(guess);

                } while (!validGuess);

                return guess;
            }

            return guess;
        }

        private Dictionary<string, int> GetWordFamilies(string guess)
        {
            // generate dictionary
            foreach (string word in wordList)
            {
                string wordState = "";

                foreach (char letter in word)
                {
                    if (letter.ToString() == guess)
                    {
                        wordState += guess;
                    }
                    else
                    {
                        wordState += "-";
                    }
                }

                if (!wordFamilies.ContainsKey(wordState))
                {
                    wordFamilies[wordState] = 1;
                }
                else
                {
                    wordFamilies[wordState] = wordFamilies[wordState] + 1;
                }
            }

            return wordFamilies;
        }

        public List<string> GetRemainingWords(string guess)
        {
            List<string> words = new List<string>();
            string family = ""; // holds the word family to return
            bool skipGuess = guessesLeft == 0 && wordFamilies.ContainsKey(currentWord);

            if (skipGuess)
            {
                family = currentWord;
            }
            else
            {
                family = GetLargestFamily();
            }

            foreach (string word in wordList)
            {
                string currentWordFamily = "";

                foreach (char letter in word)
                {
                    if (letter.ToString() == guess)
                    {
                        currentWordFamily += guess;
                    }
                    else
                    {
                        currentWordFamily += "-";
                    }
                }

                if (currentWordFamily == family)
                {
                    words.Add(word);
                }
            }

            return words;
        }

        private string GetLargestFamily()
        {
            string family = "";
            int max = 0;

            foreach (string wordFamily in wordFamilies.Keys)
            {
                if (wordFamilies[wordFamily] > max)
                {
                    max = wordFamilies[wordFamily];
                    family = wordFamily;
                }
            }

            return family;
        }
    }
}
