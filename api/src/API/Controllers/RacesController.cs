using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            Race result = await this.containerClient.GetItemAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Race> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            await this.containerClient.AddItemAsync(race);
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }
    }
}
