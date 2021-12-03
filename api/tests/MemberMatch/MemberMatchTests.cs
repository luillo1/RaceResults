using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.MemberMatch;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TestMemberMatch
{
    [TestClass]
    public class MemberMatchTests
    {
        [TestMethod]
        public void TestMethod0()
        {
            // If we look at nothing, what's the score and probability
            // that a result line refers to a particular member?
            var score1 = Scorer.DefaultPriorScore;
            Assert.AreEqual(score1, -11.51291, delta: .001);
            var probability1 = Scorer.ScoreToProbability(score1);
            Assert.AreEqual(probability1, 1e-5, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var scorer = this.TestScorer();

            // We have a member named John and we see
            // "John" in a line of some results. 1% of Americans are
            // named "John". What is the score and
            // probability that this line refers to our member?
            var score1 = Scorer.DefaultPriorScore + scorer.Delta(name: "JOHN", isContained: true);
            Assert.AreEqual(score1, -7.4185, delta: .001);
            var probability1 = Scorer.ScoreToProbability(score1);
            Assert.AreEqual(probability1, 0.000599, delta: 0.0001);

            // We have a member named Alice and we see
            // "Alice" in a line of some results. 32.75 of 100,000
            // Americans have that name. What is the score and
            // probability that this line refers to our member?
            var score2 = Scorer.DefaultPriorScore + scorer.Delta(name: "ALICE", isContained: true);
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

            var scorer = this.TestScorer();
            var score1 = Scorer.DefaultPriorScore + scorer.Delta(name: "ALICE", isContained: true);
            var score2 = score1 + scorer.Delta(name: "SMITH", isContained: true);
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
            var scorer = TestScorer();
            var score1 = Scorer.DefaultPriorScore + scorer.Delta(name: "ALICE", isContained: true);
            var score2 = score1 + scorer.Delta(name: "SMITH", isContained: false);
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

            var scorer = this.TestScorer();
            var score0 = scorer.Delta(name: "ALICE", isContained: false, probabilityAppearsInLineFromReference: .5);
            var score1 = scorer.Delta(name: "ALISON", isContained: true, probabilityAppearsInLineFromReference: .05);
            var score2 = scorer.Delta(name: "ALLISON", isContained: false, probabilityAppearsInLineFromReference: .05);
            var score012 = Scorer.DefaultPriorScore + new[] { score0, score1, score2 }.Max();
            Assert.AreEqual(score012, -8.03442, delta: .001);
            var probability012 = Scorer.ScoreToProbability(score012);
            Assert.AreEqual(probability012, 0.00032, delta: 0.0001);

            // We also see "Smith"
            // 823.92 of 100,000 Americans have the name Smith
            // What is the score and probability that this line refers to our member?
            var score4 = score012 + scorer.Delta(name: "SMITH", isContained: true);
            Assert.AreEqual(score4, -3.74640, delta: .001);
            var probability4 = Scorer.ScoreToProbability(score4);
            Assert.AreEqual(probability4, 0.023058, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod5()
        {
            // We have a member named Alice from Bellevue.
            // "Alice" in a line of some results.
            // What is the score and
            // probability that this line refers to our member?

            var scorer = this.TestScorer();
            var score = Scorer.DefaultPriorScore;
            score += scorer.Delta(name: "ALICE", isContained: true);

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
            // We found a line containing "Alison" and we wonder if it refers to our member
            // Alice Smith, for whom "Alison" and "Allison" are nicknames.
            // The line does not contain "Smith", but it does contain "Bellevue" her town.

            var scorer = this.TestScorer();
            string[] firstNameList = new[] { "ALICE", "ALISON", "ALLISON" };
            bool[] firstNameIsContainedList = new[] { false, true, false };
            string[] lastNameList = new[] { "SMITH" };
            bool[] lastNameIsContainedList = new[] { false };
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };
            bool[] cityIsContainedList = new[] { true };

            // Prior
            var score = Scorer.DefaultPriorScore;

            // Evidence from First Name
            score += scorer.Delta(
                    nameList: firstNameList,
                    isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += scorer.Delta(
                    nameList: lastNameList,
                    isContainedList: lastNameIsContainedList);

            // Evidence from City
            score += Scorer.Delta(
                    probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
                    isContainedList: cityIsContainedList);

            // Results
            Assert.AreEqual(score, -7.15334, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability, 0.0007816309, delta: 0.0001);
        }

        [TestMethod]
        public void TestMethod7()
        {
            // We found a line containing "Alice" and we wonder if it refers to our member
            // Alice Smith, for whom "Alison" and "Allison" are nicknames.
            // The line does contain "Smith", but it does not contain "Bellevue" her town
            // even though there are city names in the results.

            var scorer = this.TestScorer();
            string[] firstNameList = new[] { "ALICE", "ALISON", "ALLISON" };
            bool[] firstNameIsContainedList = new[] { true, false, false };
            string[] lastNameList = new[] { "SMITH" };
            bool[] lastNameIsContainedList = new[] { true };
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };
            bool[] cityIsContainedList = new[] { false };

            // Prior
            var score = Scorer.DefaultPriorScore;

            // Evidence from First Name
            score += scorer.Delta(
                    nameList: firstNameList,
                    isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += scorer.Delta(
                    nameList: lastNameList,
                    isContainedList: lastNameIsContainedList);

            // Evidence from City
            score += Scorer.Delta(
                    probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
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

            var scorer = this.TestScorer();
            string[] firstNameList = new[] { "ALICE", "ALISON", "ALLISON" };
            bool[] firstNameIsContainedList = new[] { true, false, false };
            string[] lastNameList = new[] { "SMITH" };
            bool[] lastNameIsContainedList = new[] { true };

            // Prior
            var score = Scorer.DefaultPriorScore;

            // Evidence from First Name
            score += scorer.Delta(
                    nameList: firstNameList,
                    isContainedList: firstNameIsContainedList);

            // Evidence from Last Name
            score += scorer.Delta(
                    nameList: lastNameList,
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
            // We can also see that 300 of the 3000 result lines mention "Bellevue".

            var scorer = this.TestScorer();

            // We list each first name and related nicknames.
            // Behind the scenes it looks up the probability
            // that we might see this name on an output line even if this line
            // doesn't refer to our Alice Smith.
            string[] firstNameList = new[] { "ALICE", "ALISON", "ALLISON" };

            // We also tell which of her first names (if any) we saw.
            bool[] firstNameIsContainedList = new[] { true, false, false };

            // We provide the same info for her last name(s)
            string[] lastNameList = new[] { "SMITH" };

            // We found "Smith" on the line, so this is 'true'
            bool[] lastNameIsContainedList = new[] { true };

            // Finally, if the race results include city information, we include
            // the probability of seeing her city on the result line of someone else.
            double[] cityConcidenceList = new[] { (300.0 + 1.0) / (3000.0 + 2.0) };

            // We found "Bellevue" on the line, so this is 'true'
            bool[] cityIsContainedList = new[] { true };

            // Prior score of -11 -- without evidence we assume it is very unlikely that
            // this line refers to our Alice Smith.
            var score = Scorer.DefaultPriorScore;
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Alice" on the line moves the score up by 8 to -4.
            // We now think there is a 1.5% chance this line refers to our Alice.
            score += scorer.Delta(
                    nameList: firstNameList,
                    isContainedList: firstNameIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Smith" moves the score up by 4 to 0.
            // We now think there is a 52% chance this line refers to our Alice.
            score += scorer.Delta(
                    nameList: lastNameList,
                    isContainedList: lastNameIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Seeing "Bellevue" moves the score from 0 to 2.
            // We now think there is an 87% chance this line refers to our Alice.
            score += Scorer.Delta(
                    probabilityAppearsInLineByCoincidenceList: cityConcidenceList,
                    isContainedList: cityIsContainedList);
            Debug.WriteLine($"Score {score} -- probability {Scorer.ScoreToProbability(score):P3}");

            // Check Results
            Assert.AreEqual(score, 1.89510, delta: .001);
            var probability = Scorer.ScoreToProbability(score);
            Assert.AreEqual(probability, 0.8693, delta: 0.0001);
        }

        [TestMethod]
        public void TestBig1()
        {
            // Download from https://1drv.ms/u/s!AkoPP4cC5J64xMgga4595XA390eN5Q?e=neugfs
            string root = @"D:\OneDrive\Shares\RaceResults";

            //TODO make this a class
            var (nameToMemberSet, citySet) = this.Members(root + "/sample_members.tsv");

            var scorer = new Scorer(root + "/name_probability.tsv");
            Assert.AreEqual(scorer.Delta(name: "JOHN", isContained: true), 2.90924881, delta: .001);

            foreach (var (withCity, filename, minimumScore, expectedFile) in new[] {
                (true, "/sample_results_withcity.txt", -9.0, "/expected_withcity.txt"),
                (false, "/sample_results_nocity.txt", -3.0, "/expected_nocity.txt"), })
            {
                var cityToFrequency = MemberMatchTests.CityToFrequency(withCity, root + filename, citySet);

                // !!!TODO could look up delta for each name at the start
                // !!!TODO run in parallel
                var outputText = new StringBuilder();
                foreach (string line in File.ReadLines(root + filename).Skip(1))
                {
                    Debug.WriteLine(line);

                    var memberAndScoreList = this.MemberAndScoreList(scorer, nameToMemberSet, cityToFrequency, line, minimumScore);

                    if (memberAndScoreList.Count > 0)
                    {
                        outputText.AppendLine(line);

                        foreach (var (member, score) in memberAndScoreList)
                        {
                            outputText.AppendLine($"\t{score:0.00}\t{member}");
                            Debug.WriteLine($"   {score:0.00} : {member}");
                            Debug.WriteLine("TODO remove");
                        }
                    }
                }

                string expectedPath = root + expectedFile;
                if (File.Exists(expectedPath))
                {
                    string expected = File.ReadAllText(expectedPath);
                    Trace.Assert(expected == outputText.ToString(), "Output text is not as expected.");
                }
                else
                {
                    File.WriteAllText(expectedPath + ".temp", outputText.ToString());
                    Trace.Assert(false, "Can't find expected output.");
                }
            }
        }

        private List<(Member member, double score)> MemberAndScoreList(
            Scorer scorer,
            Dictionary<string, HashSet<Member>> nameToMemberSet,
            Dictionary<string, double> cityToFrequency,
            string line,
            double minimumScore
            )
        {
            var tokenizedLine = this.TokenizedLine(line);

            var candidateMembers = CandidateMembers(nameToMemberSet, tokenizedLine);


            var memberAndScoreList = (
                from member in candidateMembers
                let score = ScoreMember(scorer, cityToFrequency, member, tokenizedLine)
                where score > minimumScore
                orderby score descending
                select (member, score)
                ).ToList();
            return memberAndScoreList;
        }

        private static HashSet<Member> CandidateMembers(Dictionary<string, HashSet<Member>> nameToMemberSet, (string, HashSet<string>) tokenizedLine)
        {
            var candidateMembers = (
                from token in tokenizedLine.Item2
                let memberSet = nameToMemberSet.GetValueOrDefault(token)
                where memberSet != null
                from member in memberSet
                select member).
                ToHashSet();

            Debug.WriteLine($"-> {string.Join(", ", candidateMembers)}");

            return candidateMembers;
        }

        private (string, HashSet<string>) TokenizedLine(string line)
        {
            //!!!TODO split on all whitespace, too.
            string[] separatingStrings = { ".", ",", " ", "\t" };

            string upper_line = line.ToUpperInvariant();

            var tokenSet = (from token in upper_line.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)
                            from token2 in this.ProcessName(token)
                            select token2)
                            .ToHashSet();

            foreach (var token in tokenSet)
            {
                Debug.WriteLine($"  > '{token}'");
            }

            return (upper_line, tokenSet);
        }

        private static Dictionary<string, double> CityToFrequency(bool withCity, string filePath, HashSet<string> citySet)
        {
            if (!withCity)
            {
                return null;
            }

            var resultList = (
                from line in File.ReadLines(filePath).Skip(1)
                select line.ToUpperInvariant())
                .ToList();

            int total = resultList.Count;
            var cityToFrequency = (
                from city in citySet
                let count = (
                    from result in resultList
                    where result.Contains(city) // TODO OK that substrings will match?
                    select 1)
                    .Sum()
                select (city, (count + 1.0) / (total + 2.0)))
                .ToDictionary(pair => pair.city, pair => pair.Item2);

            return cityToFrequency;
        }

        private (Dictionary<string, HashSet<Member>>, HashSet<string>) Members(string filename)
        {
            var nameToMemberSet = new Dictionary<string, HashSet<Member>>();
            var citySet = new HashSet<string>();

            foreach (string line in File.ReadLines(filename).Skip(1))
            {
                var fields = line.Split('\t');
                Trace.Assert(fields.Length == 4, "Expect four fields in the 'sample_member.tsv' file");

                // TODO: don't use _ in name. Move this into class. Move class into its own file.
                var member = new Member
                {
                    FirstList = this.ProcessName(fields[0]).Concat(this.ProcessName(fields[2])).ToList(),
                    LastList = this.ProcessName(fields[1]),
                    City = fields[3].ToUpperInvariant(),
                };
                Debug.WriteLine(line);
                Debug.WriteLine($" {member}");

                AddMemberToIndex(nameToMemberSet, member);
                citySet.Add(member.City);
            }

            // TODO be consistent between using plurals "members" and "list" (e.g. memberList)
            foreach (var (name, memberList) in nameToMemberSet)
            {
                Debug.WriteLine($"{name} -> {string.Join(", ", memberList)}");
            }

            return (nameToMemberSet, citySet);
        }

        private static void AddMemberToIndex(Dictionary<string, HashSet<Member>> nameToMemberSet, Member member)
        {
            foreach (var name in member.FirstList.Concat(member.LastList))
            {
                var memberList = nameToMemberSet.GetValueOrDefault(name);
                if (memberList is null)
                {
                    nameToMemberSet[name] = new HashSet<Member> { member };
                }
                else
                {
                    memberList.Add(member);
                }
            }
        }

        private static double ScoreMember(Scorer scorer, Dictionary<string, double> cityToFrequency, Member member, (string, HashSet<string>) tokenizedLine)
        {
            var (upper_line, tokenSet) = tokenizedLine;

            var firstNameList = member.FirstList;
            var firstNameIsContainedList = (
                from first in member.FirstList
                select tokenSet.Contains(first)
                ).ToArray();
            var lastNameList = member.LastList;
            var lastNameIsContainedList = (
                from last in member.LastList
                select tokenSet.Contains(last)
                ).ToArray();

            var score = Scorer.DefaultPriorScore;
            score += scorer.Delta(
                                 nameList: firstNameList,
                                 isContainedList: firstNameIsContainedList);
            score += scorer.Delta(
                                 nameList: lastNameList,
                                 isContainedList: lastNameIsContainedList);
            if (cityToFrequency != null)
            {
                double cityFrequency = cityToFrequency[member.City];
                bool cityIsContained = upper_line.Contains(member.City); // TODO: This test function appears in two places and could get out of sync
                score += Scorer.Delta(probabilityAppearsInLineByCoincidence: cityFrequency, isContained: cityIsContained);
            }

            return score;
        }

        private List<string> ProcessName(string field)
        {
            /* Rules for names:
                Assume no accent marks !!!TODO
                trim spaces from ends !!!TODO
                capitalize everything
                make any middle name part of the first or last name via spaces
                Remove "." and "'"
                ignore any one character names
                Split on hyphens, slashes, and spaces and treat as nicknames
                treat the nickname column as a first name nickname
                remove empty strings
            */
            field = field.ToUpperInvariant().Trim();

            // TODO what about other single-quote like characters such as back quote
            field = field.Replace(".", string.Empty).Replace("'", string.Empty);

            // TODO and all whitespace?
            string[] names = field.Split(new[] { '-', ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries);
            var names2 =
                (from name in names
                 where name.Length > 1
                 select name).ToList();
            return names2;
        }


        private Scorer TestScorer()
        {
            var nameToProbability = new Dictionary<string, double>()
            {
                { "JOHN", .01 },
                { "ALICE", 32.75 / 100_000},
                { "SMITH",  823.92 / 100_000},
                { "ALISON", 154.27 / 100_000},
                { "ALLISON", 63.54 / 100_000},
            };
            return new Scorer(nameToProbability);
        }
    }

    public class Member
    {
        public List<string> FirstList;
        public List<string> LastList;
        public string City;

        public override string ToString()
        {
            return $"{string.Join("/", this.FirstList)} {string.Join("/", this.LastList)} @ {this.City}";
        }

    }

}
