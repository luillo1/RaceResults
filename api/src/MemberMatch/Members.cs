using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RaceResults.MemberMatch
{
    public class Members
    {
        private Dictionary<string, HashSet<Member>> nameToMemberSet;
        private HashSet<string> citySet;

        public Members(string filename)
        {
            this.nameToMemberSet = new Dictionary<string, HashSet<Member>>();
            this.citySet = new HashSet<string>();

            foreach (string line in File.ReadLines(filename).Skip(1))
            {
                var fields = line.Split('\t');
                Trace.Assert(fields.Length == 4, "Expect four fields in the 'sample_member.tsv' file");

                var member = new Member(
                    firstList: Member.CanonicalizeField(fields[0]).Concat(Member.CanonicalizeField(fields[2])).ToList(),
                    lastList: Member.CanonicalizeField(fields[1]),
                    city: fields[3].ToUpperInvariant());
                Debug.WriteLine(line);
                Debug.WriteLine($" {member}");

                this.AddMemberToIndex(member);
                citySet.Add(member.City);
            }

            // TODO be consistent between using plurals "members" and "list" (e.g. memberList)
            foreach (var (name, memberList) in nameToMemberSet)
            {
                Debug.WriteLine($"{name} -> {string.Join(", ", memberList)}");
            }
        }

        public HashSet<Member> CandidateMembers(HashSet<string> tokenSet)
        {
            var candidateMembers = tokenSet
                .Select(token => nameToMemberSet.GetValueOrDefault(token))
                .Where(memberSet => memberSet != null)
                .SelectMany(memberSet => memberSet)
                .ToHashSet();

            Debug.WriteLine($"-> {string.Join(", ", candidateMembers)}");

            return candidateMembers;
        }

        public Dictionary<string, double> CityToProbability(bool withCity, string filePath)
        {
            if (!withCity)
            {
                return null;
            }

            var resultList = File.ReadLines(filePath)
                .Skip(1)
                .Select(line => line.ToUpperInvariant())
                .ToList();

            int total = resultList.Count;

            // TODO OK that substrings of city will match?

            // We add use the "Select" to ensure that the two inputs of ToDictionary will get the city values
            // in the same order. (If HashSet guaranteed deterministic enumeration, this would not be needed).
            var cityToProbability = citySet
                .Select(city => city)
                .ToDictionary(
                    city => city,
                    city =>
                    {
                        int count = resultList.Count(result => result.Contains(city));
                        double probability = (count + 1.0) / (total + 2.0);
                        return probability;
                    });

            return cityToProbability;
        }

        private void AddMemberToIndex(Member member)
        {
            foreach (var name in member.FirstList.Concat(member.LastList))
            {
                var memberList = this.nameToMemberSet.GetValueOrDefault(name);
                if (memberList is null)
                {
                    this.nameToMemberSet[name] = new HashSet<Member> { member };
                }
                else
                {
                    memberList.Add(member);
                }
            }
        }
    }
}
