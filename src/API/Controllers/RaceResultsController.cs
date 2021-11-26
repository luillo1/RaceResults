using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Data.CosmosDb;
using RaceResults.Common.Models;
using System;

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
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RaceResult> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create(RaceResult raceResult)
        {
            return CreatedAtAction(nameof(Create), new { id = raceResult.Id }, raceResult);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return NoContent();
        }
    }
}
