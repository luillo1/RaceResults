using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            RaceContainerClient container = containerProvider.RaceContainer;
            Race result = await container.GetOneAsync(id, id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            IEnumerable<Race> result = await container.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            if (race.EventId == Guid.Empty)
            {
                race.EventId = Guid.NewGuid();
            }

            await container.AddOneAsync(race);
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }
    }
}
