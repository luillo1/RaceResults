using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Data.CosmosDb;
using RaceResults.Common.Models;

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
        public async Task<IActionResult> Get()
        {
            IEnumerable<Runner> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Runner runner)
        {
            await this.containerClient.AddItemAsync(runner);
            return CreatedAtAction(nameof(Post), new { id = runner.Id }, runner);
        }
    }
}
