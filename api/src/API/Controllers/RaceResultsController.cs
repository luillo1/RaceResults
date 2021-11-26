using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceResults.Api.Controllers
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            RaceResult result = await this.containerClient.GetItemAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RaceResult> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RaceResult raceResult)
        {
            await this.containerClient.AddItemAsync(raceResult);
            return CreatedAtAction(nameof(Create), new { id = raceResult.Id }, raceResult);
        }

    }
}
