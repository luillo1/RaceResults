using RaceResults.Common.Models;
using RaceResults.Data.Core;
using System.Threading.Tasks;

namespace RaceResults.MemberMatch
{
    public class ProbabilityProvider
    {
        private ContainerClient<MemberMatchRecord> containerClient;

        public ProbabilityProvider(ContainerClient<MemberMatchRecord> containerClient)
        {
            this.containerClient = containerClient;
        }
    }
}
