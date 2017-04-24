using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpamFilter
{
    /// <summary>
    /// Klasė skirta saugoti duomenims apie žodį
    /// </summary>
    [Serializable]
    class Word :IComparable<Word>
    {
        private double spam;
        private double notSpam;
        private double probability;


        public Word ()
        {
            this.spam = 0;
            this.notSpam = 0;
            this.probability = 0.4;
        }

        public double GetProbability()
        {
            return probability;
        }

        public void AddToSpam ()
        {
            spam++;
        }

        public void AddToNotSpam ()
        {
            notSpam++;
        }

        public void CalculateProbability(int spamCount, int notSpamCount)
        {
            if (spam == 0 && notSpam == 0) return;
            double spamProp = spam / spamCount;
            probability = (spamProp) / (spamProp + (notSpam / notSpamCount));
        }

        public override string ToString()
        {
            return string.Format("Spam count:  {0, 10}, Non-spam count:   {1, 10}, Probability:  {2, 5}", spam, notSpam, probability);
        }

        public int CompareTo(Word other)
        {
            if (probability > other.probability)
                return 1;
            else if (probability < other.probability)
                return -1;
            else
                return 0;
        }
    }
}
