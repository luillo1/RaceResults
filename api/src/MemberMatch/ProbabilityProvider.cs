using RaceResults.Common.Models;
using RaceResults.Data.Core;
using System.Threading.Tasks;

namespace RaceResults.MemberMatch
{
    public class ProbabilityProvider
    {
        private ICosmosDbContainerClient<MemberMatchRecord> containerClient;

        public ProbabilityProvider(ICosmosDbContainerClient<MemberMatchRecord> containerClient)
        {
            this.containerClient = containerClient;
        }

        public async Task<double> GetProbability(string name)
        {
            MemberMatchRecord data = await this.containerClient.GetItemAsync(name);
            return data.Probability;
        }
    }
}
