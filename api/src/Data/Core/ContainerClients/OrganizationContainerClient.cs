using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public class OrganizationContainerClient : ContainerClient<Organization>
    {
        public OrganizationContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient, ContainerConstants.OrganizationContainerName)
        {
        }
    }
}
