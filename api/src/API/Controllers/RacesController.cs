using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    [ApiController]
    [Route("races")]
    public class RacesController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<RacesController> logger;

        public RacesController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RacesController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            IRaceContainerClient container = containerProvider.RaceContainer;
            Race result = await container.GetRaceAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IRaceContainerClient container = containerProvider.RaceContainer;
            IEnumerable<Race> result = await container.GetAllRacesAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            IRaceContainerClient container = containerProvider.RaceContainer;
            await container.AddRaceAsync(race);
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }
    }
}
