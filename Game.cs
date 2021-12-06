using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace WordFamilies
{
    class Game
    {
        List<string> WordList { get; set; } // holds all possible words
        List<char> GuessedLetters { get; set; } // holds users guessed letters
        string DifficultyLevel { get; set; } // holds either 'easy' or 'hard' - the level of difficulty chosen by the user.
        int WordLength { get; set; } // holds the number of letters of the word being guessed
        int GuessesLeft { get; set; } // holds the number of guesses the user has left
        string CurrentWord { get; set; } // holds the state of the current word
        char LastGuess { get; set; } // holds the last guess entered by the user
        bool GameOver { get; set; }
        bool DebugMenu { get; set; } // holds value whether or not to display the debug menu

        public void Initialise()
        {
            GameOver = false;
            WordList = File.ReadAllLines("dictionary.txt").ToList();
            GuessedLetters = new List<char>();
            WordLength = new Random().Next(4, 13); // Randomise word length 4-12 characters
            GuessesLeft = 2 * WordLength; // user starts with 2x guesses to length of word
            DifficultyLevel = PromptDifficultyLevel(); // Get difficulty (easy or hard) from user
            Display.PrintDifficultyLevel(DifficultyLevel);
            
            for (int i = 0; i < WordLength; i++)
            {
                CurrentWord += "-"; // initialise un-guessed letters to dashes
            }

            List<string> correctLengthWords = new List<string>();

            foreach (string word in WordList) // Remove all words of the wrong length
            {
                if (word.Length == WordLength)
                {
                    correctLengthWords.Add(word);
                }
            }
            // Update wordlist with only words of the correct length
            WordList = correctLengthWords;
            DebugMenu = PromptDebugMenu();
        }

        private string PromptDifficultyLevel()
        {
            bool validInput = false;
            string input = "";

            while (!validInput)
            {
                Display.PromptDifficultyLevel();
                input = Console.ReadLine().Trim();

                if (input == "1" || input == "2")
                {
                    validInput = true;
                }
                else
                {
                    Display.PrintInvalidInput();
                }
            }

            if (input == "1")
            {
                return "Easy";
            }
            else
            {
                return "Hard";
            }
        }

        public bool PromptDebugMenu()
        {
            // Display prompt message to user
            Display.PromptDebugMenu();

            return Console.ReadLine()?.ToLower().Trim() == "y"; // y=true, any other input=false
        }

        public void PromptGuess()
        {
            bool validGuess = false; //guess > 96 && guess < 123;
            char guess = ' '; //Console.ReadLine().ToLower()[0]; // get guess from user

            while (!validGuess)
            {
                Display.PromptGuess();
                guess = Console.ReadLine().ToLower()[0]; // get guess from user

                if (GuessedLetters.Contains(guess) || guess < 97 || guess > 122) //check validity of input
                {
                    Display.PrintInvalidInput();
                }
                else
                {
                    validGuess = true;
                }
            }

            LastGuess = guess;
            GuessedLetters.Add(guess);
        }

        /// <summary>
        /// Maps each word family to a code of 1's and 0's relating to the number of occurrences of the last guess.
        /// Instantiates a dictionary of the word families and the number of elements in each.
        /// Calls GetLargestFamily, passing the dictionary as an argument.
        /// </summary>
        public Dictionary<string, int> SortFamilies()
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

            return families;
        }

        /// <summary>
        /// Sorts a dictionary of word families, selecting the family with the largest number of elements.
        /// Removes all words from WordList that are not in the selected family.
        /// </summary>
        /// <param name="familyDictionary"></param>
        public void SortLargestFamily(Dictionary<string, int> familyDictionary)
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

                if (DebugMenu)
                {
                    Display.PrintWordFamilyCodesAndValues(key, familyDictionary[key]);
                }
            }

            if (DebugMenu)
            {
                Display.PrintWordFamilyChosen(code);
            }

            // remove words not in largest family
            RemoveWordsFromWordList(code);
        }

        /// <summary>
        /// This is the method used if the user chooses 'hard' mode.
        /// Uses a minimax algorithm that looks ahead and gives a weighting to word families that do not reveal letters.
        /// </summary>
        /// <param name="familyDictionary"></param>
        public void SortMinMaxFamily(Dictionary<string, int> familyDictionary)
        {
            string code = "";
            int weight = 0;

            foreach (string key in familyDictionary.Keys)
            {
                int wordWeight = 0;

                foreach (char c in key)
                {
                    if (c == '0')
                    {
                        wordWeight += 10;
                    }
                    else if (c == '1')
                    {
                        wordWeight += 1;
                    }
                }

                if (wordWeight > weight)
                {
                    code = key;
                    weight = wordWeight;
                }
            }

            RemoveWordsFromWordList(code);
        }

        public void RemoveWordsFromWordList(string code)
        {
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
                if (DebugMenu)
                {
                    Display.PrintWordlistCount(WordList.Count);
                }
                Display.PrintGuesses(GuessedLetters, GuessesLeft);
                Display.PrintWordState(CurrentWord);
                PromptGuess();
                Dictionary<string, int> sortedFamilies = SortFamilies(); // word family codes and counts
                if (DifficultyLevel == "Easy")
                    SortLargestFamily(sortedFamilies); // if easy sort families by size
                else            
                    SortMinMaxFamily(sortedFamilies); // if hard sort families using a minimax algorithm
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
