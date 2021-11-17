using MemberMatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace TestMemberMatch
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod0()
        {
            // If we look at nothing, what's the score and probability
            // that a result line refers to a particular member?

            var score1 = Scorer.defaultPriorScore;
            Assert.AreEqual(score1, -11.51291, delta: .001);
            var probability1 = Scorer.ScoreToProbability(score1);
            Assert.AreEqual(probability1, 1e-5, delta: 0.0001);

        }

        [TestMethod]
        public void TestMethod1()
        {
            // We have a member named John and we see
            // "John" in a line of some results. 1% of Americans are
            // named "John". What is the score and
            // probability that this line refers to our member?

            var score1 = Scorer.defaultPriorScore + Scorer.Delta(probabilityAppearsInLineByCoincidence: .01, isContained: true);
            Assert.AreEqual(score1, -7.4185, delta: .001);
            var probability1 = Scorer.ScoreToProbability(score1);
            Assert.AreEqual(probability1, 0.000599, delta: 0.0001);

            // We have a member named Alice and we see
            // "Alice" in a line of some results. 32.75 of 100,000
            // Americans have that name. What is the score and
            // probability that this line refers to our member?

            var score2 = Scorer.defaultPriorScore + Scorer.Delta(probabilityAppearsInLineByCoincidence: 32.75 / 100_000, isContained: true);
            Assert.AreEqual(score2, -3.9997, delta: .001);
            var probability2 = Scorer.ScoreToProbability(score2);
            Assert.AreEqual(probability2, 0.01799, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // We have a member named Alice Smith and we see
            // "Alice" and "Smith" in a line of some results.
            // 32.75 of 100,000 Americans have the name Alice
            // 823.92 of 100,000 Americans have the name Smith
            // What is the score and probability that this line refers to our member?

            var score1 = Scorer.defaultPriorScore + Scorer.Delta(probabilityAppearsInLineByCoincidence: 32.75 / 100_000, isContained: true);
            var score2 = score1 + Scorer.Delta(probabilityAppearsInLineByCoincidence: 823.92 / 100_000, isContained: true);
            Assert.AreEqual(score2, 0.2883, delta: .001);
            var probability2 = Scorer.ScoreToProbability(score2);
            Assert.AreEqual(probability2, .572, delta: 0.01);
        }

        [TestMethod]
        public void TestMethod3()
        {
            // We have a member named Alice Smith and we see
            // "Alice" but not "Smith" in a line of some results.
            // 32.75 of 100,000 Americans have the name Alice
            // 823.92 of 100,000 Americans have the name Smith
            // What is the score and probability that this line refers to our member?

            var score1 = Scorer.defaultPriorScore + Scorer.Delta(probabilityAppearsInLineByCoincidence: 32.75 / 100_000, isContained: true);
            var score2 = score1 + Scorer.Delta(probabilityAppearsInLineByCoincidence: 823.92 / 100_000, isContained: false);
            Assert.AreEqual(score2, -4.9077, delta: .001);
            var probability2 = Scorer.ScoreToProbability(score2);
            Assert.AreEqual(probability2, 0.00738, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod4()
        {
            // We have a member named Alice and we see
            // "Alison" in a line of some results.
            // "Alison" and "Allison" are nicknames of "Alice".
            // 32.75 of 100,000 Americans have the name Alice
            // 154.27 of 100,000 Americans have the name Alison
            // 63.54  of 100,000 Americans have the name Allison
            // What is the score and probability that this line refers to our member?

            var score0 = Scorer.Delta(probabilityAppearsInLineByCoincidence: 32.75 / 100_000, isContained: false, probabilityAppearsInLineFromReference: .5);
            var score1 = Scorer.Delta(probabilityAppearsInLineByCoincidence: 154.27 / 100_000, isContained: true, probabilityAppearsInLineFromReference: .05);
            var score2 = Scorer.Delta(probabilityAppearsInLineByCoincidence: 63.54 / 100_000, isContained: false, probabilityAppearsInLineFromReference: .05);
            var score012 = Scorer.defaultPriorScore + new[] { score0, score1, score2 }.Max();
            Assert.AreEqual(score012, -8.03442, delta: .001);
            var probability012 = Scorer.ScoreToProbability(score012);
            Assert.AreEqual(probability012, 0.00032, delta: 0.0001);

            // We also see "Smith"
            // 823.92 of 100,000 Americans have the name Smith
            // What is the score and probability that this line refers to our member?
            var score4 = score012 + Scorer.Delta(probabilityAppearsInLineByCoincidence: 823.92 / 100_000, isContained: true);
            Assert.AreEqual(score4, -3.74640, delta: .001);
            var probability4 = Scorer.ScoreToProbability(score4);
            Assert.AreEqual(probability4, 0.023058, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod5()
        {
            // We have a member named Alice from Bellevue.
            // "Alice" in a line of some results. 32.75 of 100,000
            // Americans have that name. What is the score and
            // probability that this line refers to our member?

            var score = Scorer.defaultPriorScore;
            score += Scorer.Delta(probabilityAppearsInLineByCoincidence: 32.75 / 100_000, isContained: true);

            // "Bellevue" appears in 30 of the 3000 result lines.
            var score1 = score + Scorer.Delta(probabilityAppearsInLineByCoincidence: (30.0 + 1.0) / (3000.0 + 2.0), isContained: true);
            Assert.AreEqual(score1, 0.0625, delta: .001);
            var probability1 = Scorer.ScoreToProbability(score1);
            Assert.AreEqual(probability1, 0.515620, delta: 0.0001);

            // What if "Bellevue" appears in 150 of the 300 result lines?
            var score2 = score + Scorer.Delta(probabilityAppearsInLineByCoincidence: (150.0 + 1.0) / (300.0 + 2.0), isContained: true);
            Assert.AreEqual(score2, -3.8173, delta: .001);
            var probability2 = Scorer.ScoreToProbability(score2);
            Assert.AreEqual(probability2, 0.021512, delta: 0.0001);

            // What if "Bellevue" appears in 30 of the 3000 result lines, but we don't see it here??
            var score3 = score + Scorer.Delta(probabilityAppearsInLineByCoincidence: (300.0 + 1.0) / (3000.0 + 2.0), isContained: false);
            Assert.AreEqual(score3, -4.810352, delta: .001);
            var probability3 = Scorer.ScoreToProbability(score3);
            Assert.AreEqual(probability3, 0.00807, delta: 0.0001);
        }


        [TestMethod]
        public void TestMethod6()
        {
            // Let's put it all together.
            // We found a line containing "Alison" and we wonder if it refers to our member
            // Alice Smith, for whom "Alison" and "Allison" are nicknames.
            // The line does not contain "Smith", but it does contain "Bellevue" her town.

            double[] firstNameCoincidenceList = new[] { 32.75 / 100_000, 154.27 / 100_000, 63.54 / 100_000 };
            bool[] firstNameIsContainedList = new[] { false, true, false };
            double[] lastNameCoincidenceList = new[] { 823.9 / 100_000 };
            bool[] lastNameIsContainedList = new[] { false };
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };
            bool[] cityIsContainedList = new[] { true };

            // Prior
            var score = Scorer.defaultPriorScore;

            // Evidence from First Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: firstNameCoincidenceList,
                                 isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: lastNameCoincidenceList,
                                 isContainedList: lastNameIsContainedList);

            // Evidence from City
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
                                 isContainedList: cityIsContainedList);

            // Results
            Assert.AreEqual(score, -3.300, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability, 0.0355, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod7()
        {
            // We found a line containing "Alice" and we wonder if it refers to our member
            // Alice Smith, for whom "Alison" and "Allison" are nicknames.
            // The line does contain "Smith", but it does not contain "Bellevue" her town
            // even though there are city names in the results

            double[] firstNameCoincidenceList = new[] { 32.75 / 100_000, 154.27 / 100_000, 63.54 / 100_000 };
            bool[] firstNameIsContainedList = new[] { true, false, false };
            double[] lastNameCoincidenceList = new[] { 823.9 / 100_000 };
            bool[] lastNameIsContainedList = new[] { true };
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };
            bool[] cityIsContainedList = new[] { false };

            // Prior
            var score = Scorer.defaultPriorScore;

            // Evidence from First Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: firstNameCoincidenceList,
                                 isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: lastNameCoincidenceList,
                                 isContainedList: lastNameIsContainedList);

            // Evidence from City
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
                                 isContainedList: cityIsContainedList);

            // Results
            Assert.AreEqual(score, -0.704623, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability, 0.33078, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod8()
        {
            // We found a line containing "Alice" and we wonder if it refers to our member
            // Alice Smith, for whom "Alison" and "Allison" are nicknames.
            // The line does contain "Smith". None of the results contain city info, so we skip
            // that.

            double[] firstNameCoincidenceList = new[] { 32.75 / 100_000, 154.27 / 100_000, 63.54 / 100_000 };
            bool[] firstNameIsContainedList = new[] { true, false, false };
            double[] lastNameCoincidenceList = new[] { 823.9 / 100_000 };
            bool[] lastNameIsContainedList = new[] { true };

            // Prior
            var score = Scorer.defaultPriorScore;

            // Evidence from First Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: firstNameCoincidenceList,
                                 isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: lastNameCoincidenceList,
                                 isContainedList: lastNameIsContainedList);


            // Results
            Assert.AreEqual(score, 0.10601, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability, 0.526477, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod9()
        {
            // Suppose we find a race results line containing the text "Alice"
            // and we wonder if it refers to our member Alice Smith,
            // for whom "Alison" and "Allison" are nicknames.
            // The line also contains "Smith" and "Bellevue", her town.
            // From outside sources we know:
            // 32.75 of 100,000 Americans have the name Alice
            // 154.27 of 100,000 Americans have the name Alison
            // 63.54  of 100,000 Americans have the name Allison
            // 823.92 of 100,000 Americans have the name Smith
            // We can also see that 300 of the 3000 result lines mention "Bellevue".

            // For each first name and related nicknames, we list the probability
            // that we might see this name on an output line even if this line
            // doesn't refer to our Alice Smith.
            double[] firstNameCoincidenceList = new[] { 32.75 / 100_000, 154.27 / 100_000, 63.54 / 100_000 };
            // We also tell which of her first names (if any) we saw.
            bool[] firstNameIsContainedList = new[] { true, false, false };

            // We provide the same info for her last name(s)
            double[] lastNameCoincidenceList = new[] { 823.9 / 100_000 };
            // We found "Smith" on the line, so this is 'true'
            bool[] lastNameIsContainedList = new[] { true };

            // Finally, if the race results include city information, we include
            // the probability of seeing her city on this result line in the
            // case where this line is not about her.
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };
            // We found "Bellevue" on the line, so this is 'true'
            bool[] cityIsContainedList = new[] { true };

            // Prior score of -11 -- without evidence we assume it is very unlikely that
            // this line refers to our Alice Smith.
            var score = Scorer.defaultPriorScore;
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Alice" on the line moves the score up 7 to -4.
            // We now think there is a 1.5% chance this line refers to our Alice.
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: firstNameCoincidenceList,
                                 isContainedList: firstNameIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Smith" moves the score up 4 to 0.
            // We now think there is a 50% chance this line refers to our Alice.
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: lastNameCoincidenceList,
                                 isContainedList: lastNameIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Bellevue" move the score from 0 to 2.
            // We now think there is an 87% chance this line refers to our Alice.
            score += Scorer.Delta(probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
                                 isContainedList: cityIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Check Results
            Assert.AreEqual(score, 1.89510, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability,  0.8693, delta: 0.0001);
        }

    }
}
