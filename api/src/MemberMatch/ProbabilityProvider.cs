using RaceResults.Common.Models;
using RaceResults.Data.Core;
using System.Threading.Tasks;

namespace RaceResults.MemberMatch
{
    public class ProbabilityProvider
    {
        private MemberMatchContainerClient containerClient;

        public ProbabilityProvider(MemberMatchContainerClient containerClient)
        {
            this.containerClient = containerClient;
        }

        public async Task<double> GetProbability(string name)
        {
            MemberMatchRecord data = await this.containerClient.GetOneAsync(name, name);
            return data.Probability;
        }
    }
}
