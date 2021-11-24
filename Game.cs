using System;
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

            string guess = Console.ReadLine().ToLower().Trim(); // get guess from user
            bool validGuess = guess.Length == 1 && !GuessedLetters.Contains(guess); // check validity of input
            
            if (!validGuess)
            {
                do
                {
                    if (guess.Length != 1)
                    {
                        Display.PrintIncorrectGuessLength();
                        Display.PromptGuess();
                    }

                    if (GuessedLetters.Contains(guess))
                    {
                        Display.PrintAlreadyGuessedLetterError();
                        Display.PromptGuess();
                    }

                    guess = Console.ReadLine().ToLower().Trim();
                    validGuess = guess.Length == 1 && !GuessedLetters.Contains(guess);

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
                    if (c.ToString() == LastGuess)
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
                    if (word[i].ToString() == LastGuess && code[i] == '0')
                    {
                        WordList.Remove(word);
                    }
                    else if (word[i].ToString() == LastGuess && code[i] == '1')
                    {
                        continue;
                    }
                    else if (word[i].ToString() != LastGuess && code[i] == '0')
                    {
                        continue;
                    }
                    else if (word[i].ToString() != LastGuess && code[i] == '1')
                    {
                        WordList.Remove(word);
                    }
                }
            }
        }

        //public void PartitionWordList()
        //{
        //    List<string> wordsWithoutGuess = new List<string>();
        //    List<string> wordsWithOneGuess = new List<string>();
        //    List<string> wordsWithTwoGuesses = new List<string>();
        //    List<string> wordsWithThreeGuesses = new List<string>();
        //    List<string> wordsWithFourGuesses = new List<string>();
        //    List<string> wordsWithFivePlusGuesses = new List<string>();
        //    List<List<string>> wordFamilies = new List<List<string>>(); // holds all sorted word families

        //    // Add words families to list
        //    wordFamilies.Add(wordsWithoutGuess);
        //    wordFamilies.Add(wordsWithOneGuess);
        //    wordFamilies.Add(wordsWithTwoGuesses);
        //    wordFamilies.Add(wordsWithThreeGuesses);
        //    wordFamilies.Add(wordsWithFourGuesses);
        //    wordFamilies.Add(wordsWithFivePlusGuesses);

        //    foreach (string word in WordList)
        //    {
        //        int numberOfOccurences = 0; // counter for number of times guessed letter occurs in each word

        //        foreach (char c in word) // count occurrences of guessed letter
        //        {
        //            if (c.ToString() == LastGuess)
        //            {
        //                numberOfOccurences++;
        //            }
        //        }

        //        if (numberOfOccurences == 0)
        //        {
        //            wordsWithoutGuess.Add(word);
        //        }
        //        else if (numberOfOccurences == 1)
        //        {
        //            wordsWithOneGuess.Add(word);
        //        }
        //        else if (numberOfOccurences == 2)
        //        {
        //            wordsWithTwoGuesses.Add(word);
        //        }
        //        else if (numberOfOccurences == 3)
        //        {
        //            wordsWithThreeGuesses.Add(word);
        //        }
        //        else if (numberOfOccurences == 4)
        //        {
        //            wordsWithFourGuesses.Add(word);
        //        }
        //        else if (numberOfOccurences >= 5)
        //        {
        //            wordsWithFivePlusGuesses.Add(word);
        //        }
        //    }

        //    List<string> largestFamily = new List<string>(); // holds the largest family of words
        //    int wordCount = 0; // holds number of words in word family

        //    // Find largest family
        //    foreach (List<string> family in wordFamilies)
        //    {
        //        if (family.Count > wordCount)
        //        {
        //            wordCount = family.Count; // update current largest total
        //            largestFamily = family; // assign current largest family
        //        }
        //    }

        //    // Update wordlist to largest family
        //    WordList = largestFamily;
        //}

        //public void FindLargestNumberOfOccurrences()
        //{
        //    Dictionary<int, int> indexOccurrences = new Dictionary<int, int>(); // holds number of occurrences of last guessed letter for each index in every word in wordlist

        //    foreach (string word in WordList)
        //    {
        //        for (int i = 0; i < WordLength; i++)
        //        {
        //            if (word[i].ToString() == LastGuess)
        //            {
        //                if (indexOccurrences.ContainsKey(i)) // check if dictionary already contains a value for the letter index
        //                {
        //                    indexOccurrences[i]++;
        //                }
        //                else
        //                {
        //                    indexOccurrences.Add(i, 1);
        //                }
        //            }
        //        }
        //    }

        //    List<int> maxValueIndexes = new List<int>(); // holds indexes of letters with max number of occurrences - needs to hold multiple in case multiple letter indexes have the ame value
        //    int maxValue = 0; // counter for the maximum number of occurrences
        //    // find largest value in dictionary
        //    foreach (int i in indexOccurrences.Keys)
        //    {
        //        if (indexOccurrences[i] > maxValue)
        //        {
        //            maxValue = indexOccurrences[i];
        //        }
        //    }

        //    // add index(es) with the max value to list
        //    foreach (int i in indexOccurrences.Keys)
        //    {
        //        if (indexOccurrences[i] == maxValue)
        //        {
        //            maxValueIndexes.Add(i);
        //        }
        //    }

        //    //remove other words from word list
        //    foreach (string word in WordList.ToList())
        //    {
        //        foreach (int i in maxValueIndexes)
        //        {
        //            if (word[i].ToString() != LastGuess)
        //            {
        //                WordList.Remove(word);
        //            }
        //        }
        //    }
        //}

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
                if (GuessedLetters.Contains(WordList[0][i].ToString()))
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
                    if (c.ToString() == LastGuess)
                    {
                        count++;
                    }
                }
                Display.PrintCorrectGuess(LastGuess, count);
            }
        }
    }
}
