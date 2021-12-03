using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                // TODO: don't use _ in name. Move this into class. Move class into its own file.
                var member = new Member
                {
                    FirstList = Member.ProcessName(fields[0]).Concat(Member.ProcessName(fields[2])).ToList(),
                    LastList = Member.ProcessName(fields[1]),
                    City = fields[3].ToUpperInvariant(),
                };
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

        public HashSet<Member> CandidateMembers((string, HashSet<string>) tokenizedLine)
        {
            var candidateMembers = (
                from token in tokenizedLine.Item2
                let memberSet = this.nameToMemberSet.GetValueOrDefault(token)
                where memberSet != null
                from member in memberSet
                select member).
                ToHashSet();

            Debug.WriteLine($"-> {string.Join(", ", candidateMembers)}");

            return candidateMembers;
        }

        public Dictionary<string, double> CityToFrequency(bool withCity, string filePath)
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
                from city in this.citySet
                let count = (
                    from result in resultList
                    where result.Contains(city) // TODO OK that substrings will match?
                    select 1)
                    .Sum()
                select (city, (count + 1.0) / (total + 2.0)))
                .ToDictionary(pair => pair.city, pair => pair.Item2);

            return cityToFrequency;
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
