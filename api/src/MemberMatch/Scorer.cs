using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RaceResults.MemberMatch
{
    public class Scorer
    {
        public const double DefaultPriorScore = -11.51291546;
        public const double DefaultProbabilityAppearsInLineFromReference = 0.6;
        public const double DefaultRare = 1e-5;

        private Dictionary<string, double> nameToProbability;

        public Scorer(Dictionary<string, double> nameToProbability)
        {
            this.nameToProbability = nameToProbability;
        }

        public Scorer(string nameToProbabilityFileName)
        {
            nameToProbability = new Dictionary<string, double>();
            int index = -1;
            foreach (string line in File.ReadLines(nameToProbabilityFileName))
            {
                index++;
                if (index == 0)
                {
                    continue;
                }

                var fields = line.Split('\t');
                nameToProbability[fields[0]] = double.Parse(fields[1]);
            }
        }

        public static double Delta(
                double probabilityAppearsInLineByCoincidence,
                bool isContained,
                double probabilityAppearsInLineFromReference = double.NaN)
        {
            if (double.IsNaN(probabilityAppearsInLineFromReference))
            {
                probabilityAppearsInLineFromReference = DefaultProbabilityAppearsInLineFromReference;
            }

            double logBayesFactor;
            if (isContained)
            {
                logBayesFactor = Math.Log(probabilityAppearsInLineFromReference / probabilityAppearsInLineByCoincidence);
            }
            else
            {
                logBayesFactor = Math.Log((1.0 - probabilityAppearsInLineFromReference) / (1.0 - probabilityAppearsInLineByCoincidence));
            }

            return logBayesFactor;
        }

        public static double ScoreToProbability(double score)
        {
            var odds = Math.Exp(score);
            var probability = odds / (1 + odds);
            return probability;
        }

        public static double Delta(
                                   double[] probabilityAppearsInLineByCoincidenceList,
                                   bool[] isContainedList,
                                   double[] probabilityAppearsInLineFromReferenceList = default)
        {
            var length = probabilityAppearsInLineByCoincidenceList.Length;
            Trace.Assert(length == isContainedList.Length, "Expect lists to all have same length");
            Trace.Assert(
                    probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Length,
                    "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = (from coincidence in probabilityAppearsInLineByCoincidenceList
                         from isContained in isContainedList
                         from fromRef in probabilityAppearsInLineFromReferenceList
                         let altScore = Scorer.Delta(
                             probabilityAppearsInLineByCoincidence: coincidence,
                             isContained: isContained,
                             probabilityAppearsInLineFromReference: fromRef)
                         select altScore)
                        .Max();
            return delta;
        }

        public double Delta(
                                   string[] nameList,
                                   bool[] isContainedList,
                                   double[] probabilityAppearsInLineFromReferenceList = default)
        {
            var length = nameList.Length;
            Trace.Assert(length == isContainedList.Length, "Expect lists to all have same length");
            Trace.Assert(
                    probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Length,
                    "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = (from name in nameList
                         from isContained in isContainedList
                         from fromRef in probabilityAppearsInLineFromReferenceList
                         let altScore = this.Delta(
                             name: name,
                             isContained: isContained,
                             probabilityAppearsInLineFromReference: fromRef)
                         select altScore)
                        .Max();
            return delta;
        }

        public double Delta(
                string name,
                bool isContained,
                double probabilityAppearsInLineFromReference = double.NaN)
        {
            var probabilityAppearsInLineByCoincidence = this.nameToProbability.GetValueOrDefault(name, DefaultRare);
            return Delta(probabilityAppearsInLineByCoincidence, isContained, probabilityAppearsInLineFromReference);
        }

        private static double[] ReplaceAnyDefault(double[] probabilityAppearsInLineFromReference, int length)
        {
            if (probabilityAppearsInLineFromReference == default)
            {
                if (length == 1)
                {
                    probabilityAppearsInLineFromReference = new[] { DefaultProbabilityAppearsInLineFromReference };
                }
                else
                {
                    Trace.Assert(DefaultProbabilityAppearsInLineFromReference == .6); // real assert
                    probabilityAppearsInLineFromReference = Enumerable.Repeat(.1 / (length - 1), length).ToArray();
                    probabilityAppearsInLineFromReference[0] = .5;
                }
            }

            return probabilityAppearsInLineFromReference;
        }
    }
}
