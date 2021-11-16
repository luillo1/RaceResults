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
    public class RaceResultsController : ControllerBase
    {
        private readonly ICosmosDbContainerClient<RaceResult> containerClient;

        private readonly ILogger<RaceResultsController> logger;

        public RaceResultsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RaceResultsController> logger)
        {
            this.containerClient = cosmosDbContainerProvider.RaceResultContainer;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<RaceResult> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(RaceResult raceResult)
        {
            await this.containerClient.AddItemAsync(raceResult);
            return CreatedAtAction(nameof(Post), new { id = raceResult.Id }, raceResult);
        }

    }
}
