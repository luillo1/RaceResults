using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    /// <summary>
    ///     Client used to interact with the container for <see cref="Race"/> models.
    /// </summary>
    public class RaceContainerClient
        : ContainerClient<Race>
    {
        /// <inheritdoc/>
        protected override string ContainerName => ContainerConstants.RaceContainerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RaceContainerClient"/> class.
        /// </summary>
        /// <param name="cosmosDbClient">
        ///     The <see cref="ICosmosDbClient"/> used to get an instance of a <see cref="Container"/>.
        /// </param>
        public RaceContainerClient(ICosmosDbClient cosmosDbClient)
            : base(cosmosDbClient)
        {
        }
    }
}
