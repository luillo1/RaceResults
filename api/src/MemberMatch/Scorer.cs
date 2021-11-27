using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RaceResults.MemberMatch
{
    public class Scorer
    {
        static public double defaultPriorScore = -11.51291546;
        static public double defaultProbabilityAppearsInLineFromReference = 0.6;
        static private double defaultRare = 1e-5;

        public Scorer(Dictionary<string, double> nameToProbability)
        {
            _nameToProbability = nameToProbability;
        }


        public Scorer(string nameToProbabilityFileName)
        {
            _nameToProbability = new Dictionary<string, double>();
            int index = -1;
            foreach (string line in File.ReadLines(nameToProbabilityFileName))
            {
                index++;
                if (index==0)
                {
                    continue;
                }
                var fields = line.Split('\t');
                _nameToProbability[fields[0]] = double.Parse(fields[1]);
            }
        }

        private Dictionary<string, double> _nameToProbability;

        static public double Delta(
                                   double[] probabilityAppearsInLineByCoincidenceList,
                                   bool[] isContainedList,
                                   double[] probabilityAppearsInLineFromReferenceList = default)

        {
            var length = probabilityAppearsInLineByCoincidenceList.Length;
            Trace.Assert(length == isContainedList.Length, "Expect lists to all have same length");
            Trace.Assert(probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Length,
                "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = _ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = (from coincidence in probabilityAppearsInLineByCoincidenceList
                         from isContained in isContainedList
                         from fromRef in probabilityAppearsInLineFromReferenceList
                         let altScore = Scorer.Delta(probabilityAppearsInLineByCoincidence: coincidence,
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
            Trace.Assert(probabilityAppearsInLineFromReferenceList == default || length == probabilityAppearsInLineFromReferenceList.Length,
                "Expect lists to all have same length");
            if (length == 0)
            {
                return 0;
            }

            probabilityAppearsInLineFromReferenceList = _ReplaceAnyDefault(probabilityAppearsInLineFromReferenceList, length);

            var delta = (from name in nameList
                         from isContained in isContainedList
                         from fromRef in probabilityAppearsInLineFromReferenceList
                         let altScore = this.Delta(name: name,
                             isContained: isContained,
                             probabilityAppearsInLineFromReference: fromRef)
                         select altScore)
                        .Max();
            return delta;
        }


        private static double[] _ReplaceAnyDefault(double[] probabilityAppearsInLineFromReference, int length)
        {
            if (probabilityAppearsInLineFromReference == default)
            {
                if (length == 1)
                {
                    probabilityAppearsInLineFromReference = new[] { defaultProbabilityAppearsInLineFromReference };
                }
                else
                {
                    Trace.Assert(defaultProbabilityAppearsInLineFromReference == .6); //real assert
                    probabilityAppearsInLineFromReference = Enumerable.Repeat(.1 / (length - 1), length).ToArray();
                    probabilityAppearsInLineFromReference[0] = .5;
                }
            }

            return probabilityAppearsInLineFromReference;
        }

        public double Delta(string name,
                                   bool isContained,
                                   double probabilityAppearsInLineFromReference = double.NaN)
        {
            var probabilityAppearsInLineByCoincidence = this._nameToProbability.GetValueOrDefault(name, Scorer.defaultRare);
            return Delta(probabilityAppearsInLineByCoincidence, isContained, probabilityAppearsInLineFromReference);
        }


        static public double Delta(double probabilityAppearsInLineByCoincidence,
                                   bool isContained,
                                   double probabilityAppearsInLineFromReference = double.NaN)
        {
            if (probabilityAppearsInLineFromReference != probabilityAppearsInLineFromReference)
            {
                probabilityAppearsInLineFromReference = defaultProbabilityAppearsInLineFromReference;
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

        static public double ScoreToProbability(double score)
        {
            var odds = Math.Exp(score);
            var probability = odds / (1 + odds);
            return probability;
        }
    }
}
