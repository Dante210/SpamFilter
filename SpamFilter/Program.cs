using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SpamFilter
{
    class Program
    {
        static string SOURCE_DIRECTORY = Directory.GetCurrentDirectory();
        static string SPAM_SOURCE_DIRECTORY = SOURCE_DIRECTORY + "\\..\\..\\Spamas";
        static string NOT_SPAM_SOURCE_DIRECTORY = SOURCE_DIRECTORY + "\\..\\..\\NeSpamas";
        static string DICTIONARY = SOURCE_DIRECTORY + "\\..\\..\\dictionary.bin";

        static void Main(string[] args)
        {
            Dictionary<string, Word> dictionary = new Dictionary<string, Word>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            if (File.Exists(DICTIONARY))
            {
                Load(ref dictionary, binaryFormatter, DICTIONARY);
            }
            else
            {
                //Learn
                int spamWordCount;
                int notSpamWordCount;
                ReadFiles(SPAM_SOURCE_DIRECTORY, dictionary, true, out spamWordCount);
                ReadFiles(NOT_SPAM_SOURCE_DIRECTORY, dictionary, false, out notSpamWordCount);
                Learn(dictionary, spamWordCount, notSpamWordCount);
                Save(dictionary, binaryFormatter, DICTIONARY);
            }
            PrintDictionary("Apmokymas.txt", dictionary);
            CheckFile("Failas.txt", dictionary, 50);
        }

        static void Learn (Dictionary<string, Word> dictionary, int spamWordCount, int notSpamWordCount)
        {
            foreach (KeyValuePair<string, Word> word in dictionary)
            {
                word.Value.CalculateProbability(spamWordCount, notSpamWordCount);
            }
        }
        static void ReadFiles (string sourceDirectory, Dictionary<string, Word> dictionary, bool isSpam, out int wordCountInFiles)
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");
            wordCountInFiles = 0;
            foreach (string currentFile in txtFiles)
            {
                DataFile datafile = new DataFile();
                datafile.ReadFile(currentFile, dictionary, isSpam);
                wordCountInFiles += datafile.WordCount;
            }
        }

        static void Load(ref Dictionary<string, Word> dictionary, BinaryFormatter binaryFormatter, string destination)
        {
            using (var fs = File.Open(destination, FileMode.Open))
            {
                dictionary = (Dictionary<string, Word>)binaryFormatter.Deserialize(fs);
            }
        }

        static void Save(Dictionary<string, Word> dictionary, BinaryFormatter binaryFormatter, string destination)
        {
            using (var fs = File.Create(destination))
            {
                binaryFormatter.Serialize(fs, dictionary);
            }
        }

        static void PrintDictionary (string fileName, Dictionary<string, Word> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (KeyValuePair<string, Word> pairs in dictionary)
                {
                    string line = string.Format("Word: {0, -50},   {1}", pairs.Key, pairs.Value.ToString());
                    writer.WriteLine(line);
                }
            }
        }

        static void CheckFile (string filePath, Dictionary<string, Word> dictionary, int size)
        {
            Dictionary<string, Word> tempDictionary;
            DataFile dataFile = new DataFile();
            dataFile.ReadFile(filePath, dictionary, out tempDictionary);
            var sortedDict = tempDictionary.OrderByDescending(entry => entry.Value)
                     .Take(size)
                     .ToDictionary(pair => pair.Key, pair => pair.Value);
            double multiply = 1;
            double inverseMultiply = 1;

            int counter = 0;
            foreach (var item in sortedDict)
            {
                Console.WriteLine(counter + " " + item.Value.GetProbability());
                multiply *= item.Value.GetProbability();
                inverseMultiply *= (1 - item.Value.GetProbability());
                counter++;
            }
            double answer = multiply / (multiply + inverseMultiply);
            Console.WriteLine("Answer " + answer);
        }

    }
}
