using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpamFilter
{

    /// <summary>
    /// Klasė skirta darbui su vienu tekstiniu failu.
    /// </summary>
    class DataFile
    {
        //private double probability;
        const string SEPERATORS = " [\n\t\r.,;:!?(){]><={}";

        public  int WordCount = 0; //Words in file 

        public  void ReadFile(string filePath, Dictionary<string, Word> dictionary, bool isSpam)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string tempLine;
                while ((tempLine = sr.ReadLine()) != null)
                {
                    string [] words = tempLine.Split(SEPERATORS.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        WordCount++;
                        if(dictionary.ContainsKey(word))
                        {
                            if (isSpam)
                            {
                                dictionary[word].AddToSpam();
                            }
                            else
                            {
                                dictionary[word].AddToNotSpam();
                            }
                        }
                        else
                        {
                            dictionary.Add(word, new Word());
                        }
                    }
                }
            }
        }

        public  void ReadFile(string filePath, Dictionary<string, Word> dictionary, out Dictionary<string, Word> tempDictionary)
        {
            tempDictionary = new Dictionary<string, Word>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string tempLine;
                while ((tempLine = sr.ReadLine()) != null)
                {
                    string[] words = tempLine.Split(SEPERATORS.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        WordCount++;
                        if (dictionary.ContainsKey(word))
                        {
                            if(!(tempDictionary.ContainsKey(word)))
                                tempDictionary.Add(word, dictionary[word]);
                        }
                        else
                        {
                            dictionary.Add(word, new Word());
                            tempDictionary.Add(word, new Word());
                        }
                    }
                }
            }
        }
    }
}
