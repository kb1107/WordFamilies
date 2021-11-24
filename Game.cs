﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace WordFamilies
{
    class Game
    {
        List<string> WordList { get; set; } // holds all possible words
        List<char> GuessedLetters { get; set; } // holds users guessed letters
        // difficulty level always holds easy for the moment. Will implement hard later.
        private string DifficultyLevel { get; set; } // holds either 'easy' or 'hard' - the level of difficulty chosen by the user.
        int WordLength { get; set; } // holds the number of letters of the word being guessed
        private int GuessesLeft { get; set; } // holds the number of guesses the user has left
        string CurrentWord { get; set; } // holds the state of the current word
        char LastGuess { get; set; } // holds the last guess entered by the user
        private bool GameOver { get; set; }

        public void Initialise()
        {
            GameOver = false;
            WordList = File.ReadAllLines("dictionary.txt").ToList();
            GuessedLetters = new List<char>();
            WordLength = new Random().Next(4, 13); // Randomise word length 4-12 characters
            GuessesLeft = 2 * WordLength; // user starts with 2x guesses to length of word
            DifficultyLevel = "easy"; // always easy for now. Will update later.

            for (int i = 0; i < WordLength; i++)
            {
                CurrentWord += "-"; // initialise un-guessed letters to dashes
            }

            List<string> correctLengthWords = new List<string>();
            // Remove all words of the wrong length
            foreach (string word in WordList)
            {
                if (word.Length == WordLength)
                {
                    correctLengthWords.Add(word);
                }
            }
            // Update wordlist with only words of the correct length
            WordList = correctLengthWords;
        }

        public void PromptGuess()
        {
            Display.PromptGuess();

            char guess = Console.ReadLine().ToLower()[0]; // get guess from user
            bool validGuess = guess > 96 && guess < 123;
            
            if (!validGuess)
            {
                do
                {
                    if (GuessedLetters.Contains(guess))
                    {
                        Display.PrintAlreadyGuessedLetterError();
                        Display.PromptGuess();
                    }
                    else
                    {
                        Display.PrintInvalidInput();
                        Display.PromptGuess();
                    }

                    guess = Console.ReadLine().ToLower()[0];
                    validGuess = guess > 96 && guess < 123;
                } while (!validGuess);
            }

            LastGuess = guess; // update valid guess
            GuessedLetters.Add(guess); // Add to list
        }

        public void SortFamilies()
        {
            Dictionary<string, int> families = new Dictionary<string, int>();

            foreach (string word in WordList)
            {
                StringBuilder familyCode = new StringBuilder(WordLength); // holds a string containing 1 and 0 in relation to whether the corresponding index matches users last guess

                foreach (char c in word)
                {
                    if (c == LastGuess)
                    {
                        familyCode.Append('1'); // 1 = match
                    }
                    else
                    {
                        familyCode.Append('0'); // 0 = no match
                    }
                }

                if (families.ContainsKey(familyCode.ToString()))
                {
                    families[familyCode.ToString()]++;
                }
                else
                {
                    families.Add(familyCode.ToString(), 1); // add first instance of word family to dict.
                }
            }

            GetLargestFamily(families);
        }

        private void GetLargestFamily(Dictionary<string, int> familyDictionary)
        {
            string code = ""; // holds the code which corresponds to the number/ index of last guess occurrences
            int familyCount = 0; // holds the largest number of elements in dict.

            // Get largest family
            foreach (string key in familyDictionary.Keys)
            {
                if (familyDictionary[key] > familyCount)
                {
                    code = key;
                    familyCount = familyDictionary[key];
                }

                //---TESTING---
                Console.WriteLine("Family Code: " + key + " Count: " + familyDictionary[key]);
            }

            //---TESTING---
            Console.WriteLine("Family Chosen: " + code);

            // remove words not in largest family
            foreach (string word in WordList.ToList())
            {
                for (int i = 0; i < WordLength; i++)
                {
                    if (word[i] == LastGuess && code[i] == '0')
                    {
                        WordList.Remove(word);
                    }
                    else if (word[i] == LastGuess && code[i] == '1')
                    {
                        continue;
                    }
                    else if (word[i] != LastGuess && code[i] == '0')
                    {
                        continue;
                    }
                    else if (word[i] != LastGuess && code[i] == '1')
                    {
                        WordList.Remove(word);
                    }
                }
            }
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
                    if (CurrentWord.Contains('-'))
                    {
                        Display.PrintGameLost(WordList[0]);
                        GameOver = true;
                    }
                    else
                    {
                        Display.PrintGameWon(WordList[0]);
                    }
                }
            }
            else
            {
                if (!CurrentWord.Contains('-'))
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
                //for testing
                Console.WriteLine("wordlist count = " + WordList.Count);

                Display.PrintGuesses(GuessedLetters, GuessesLeft);
                Display.PrintWordState(CurrentWord);
                PromptGuess();
                //PartitionWordList();
                //FindLargestNumberOfOccurrences();

                SortFamilies(); //new

                UpdateWord();
                UpdateGuesses();
                CheckGameStatus();                
            }
        }

        public bool PlayAgain()
        {
            Display.PrintPlayAgain();
            string input = Console.ReadLine().ToLower().Trim(); // get input from user

            return input == "y";
        }

        public void UpdateWord()
        {
            StringBuilder newWordState = new StringBuilder(CurrentWord, WordLength);
            
            // Reveal correctly guessed letters, keep hidden letters as dashes
            for (int i = 0; i < WordLength; i++)
            {
                if (GuessedLetters.Contains(WordList[0][i]))
                {
                    newWordState[i] = WordList[0][i];
                }
                else
                {
                    newWordState[i] = '-';
                }
            }

            CurrentWord = newWordState.ToString();
        }

        public void UpdateGuesses()
        {
            // If guess was unsuccessful, deduct 1
            if (!CurrentWord.Contains(LastGuess))
            {
                GuessesLeft--;
                Display.PrintWrongGuess(LastGuess);
            }
            else // correct guess
            {
                int count = 0; // used to count occurrences of correct guess
                foreach (char c in CurrentWord)
                {
                    if (c == LastGuess)
                    {
                        count++;
                    }
                }
                Display.PrintCorrectGuess(LastGuess, count);
            }
        }
    }
}
