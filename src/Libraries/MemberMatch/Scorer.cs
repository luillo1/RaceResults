using System;
using System.Diagnostics;
using System.Linq;

namespace MemberMatch
{
    static public class Scorer
    {
        static public double defaultPriorScore = -11.51291546;
        static public double defaultProbabilityAppearsInLineFromReference = 0.6;

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
