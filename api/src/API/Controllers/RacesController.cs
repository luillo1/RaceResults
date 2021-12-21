using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    [Authorize]
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

        [HttpPatch]
        public async Task<IActionResult> Update(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            var updatedRace = await container.UpdateOneAsync(race);
            return Ok(updatedRace);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;

            if (!(await PublicRacesController.InitAndVerifyRace(race, container)))
            {
                return BadRequest();
            }

            await container.AddOneAsync(race);
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }
    }
}
