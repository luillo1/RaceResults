using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Data;
using RaceResults.Models;

namespace RaceResults.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunnersController : ControllerBase
    {
        private readonly ICosmosDbContainerClient<Runner> containerClient;

        private readonly ILogger<RunnersController> logger;

        public RunnersController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RunnersController> logger)
        {
            this.containerClient = cosmosDbContainerProvider.RunnerContainer;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Runner>> Get()
        {
            IEnumerable<Runner> result = await this.containerClient.GetItemsAsync();
            return result;
        }
    }
}
