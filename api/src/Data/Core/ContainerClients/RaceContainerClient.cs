using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class RaceContainerClient : ContainerClient<Race>
    {
        public RaceContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient.GetContainer(ContainerConstants.RaceContainerName))
        {
        }
    }
}
