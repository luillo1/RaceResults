using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    /// <summary>
    ///     Client used to interact with the container for <see cref="Organization"/> models.
    /// </summary>
    public class OrganizationContainerClient
        : ContainerClient<Organization>
    {
        /// <inheritdoc/>
        protected override string ContainerName => ContainerConstants.OrganizationContainerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationContainerClient"/> class.
        /// </summary>
        /// <param name="cosmosDbClient">
        ///     The <see cref="ICosmosDbClient"/> used to get an instance of a <see cref="Container"/>.
        /// </param>
        public OrganizationContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient)
        {
        }
    }
}
