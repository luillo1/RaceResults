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
    public class RacesController : ControllerBase
    {
        private readonly ICosmosDbContainerClient<Race> containerClient;

        private readonly ILogger<RacesController> logger;

        public RacesController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RacesController> logger)
        {
            this.containerClient = cosmosDbContainerProvider.RaceContainer;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Race> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create(Race race)
        {
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return NoContent();
        }
    }
}
