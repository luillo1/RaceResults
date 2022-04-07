using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RaceResults.MemberMatch
{
    public class Scorer
    {
        public static double DefaultPriorScore = -11.51291546;
        public static double DefaultProbabilityAppearsInLineFromReference = 0.6;
        private static double defaultRare = 1e-5;

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

        private Dictionary<string, double> nameToProbability;

        public static double Delta(
                                   IList<double> probabilityAppearsInLineByCoincidenceList,
                                   IList<bool> isContainedList,
                                   IList<double> probabilityAppearsInLineFromReferenceList = default)
        {
            var length = probabilityAppearsInLineByCoincidenceList.Count;
            Trace.Assert(length == isContainedList.Count, "Expect lists to all have same length");
            Trace.Assert(
                probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Count,
                "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = probabilityAppearsInLineByCoincidenceList
                .Zip(isContainedList, probabilityAppearsInLineFromReferenceList)
                .Select(tuple => Scorer.Delta(tuple.Item1, tuple.Item2, tuple.Item3))
                .Max();
            return delta;
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

        public double Delta(
                                   IList<string> nameList,
                                   IList<bool> isContainedList,
                                   IList<double> probabilityAppearsInLineFromReferenceList = default)
        {
            var length = nameList.Count;
            Trace.Assert(length == isContainedList.Count, "Expect lists to all have same length");
            Trace.Assert(
                probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Count,
                "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = nameList.Zip(isContainedList, probabilityAppearsInLineFromReferenceList)
                    .Select(tuple => Delta(tuple.First, tuple.Second, tuple.Third))
                    .Max();

            return delta;
        }

        public double Delta(
            string name,
            bool isContained,
            double probabilityAppearsInLineFromReference = double.NaN)
        {
            var probabilityAppearsInLineByCoincidence = this.nameToProbability.GetValueOrDefault(name, Scorer.defaultRare);
            return Delta(probabilityAppearsInLineByCoincidence, isContained, probabilityAppearsInLineFromReference);
        }

        private static IList<double> ReplaceAnyDefault(IList<double> probabilityAppearsInLineFromReference, int length)
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