using System;
using System.Collections.Generic;
using System.Linq;
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

        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<IActionResult> GetAllPublic()
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            IEnumerable<Race> result = await container.GetManyAsync(it => it.Where(race => race.IsPublic));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;

            var (validRace, updatedRace) = await InitAndVerifyRace(race, container);
            if (!validRace)
            {
                return BadRequest();
            }

            var addedRace = await container.AddOneAsync(updatedRace);
            return CreatedAtAction(nameof(Create), new { id = addedRace.Id }, addedRace);
        }

        [AllowAnonymous]
        [HttpPost("public")]
        public async Task<IActionResult> CreatePublic(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;

            var (validRace, updatedRace) = await InitAndVerifyRace(race, container);
            if (!validRace)
            {
                return BadRequest();
            }

            updatedRace.IsPublic = false;
            var addedRace = await container.AddOneAsync(updatedRace);
            return CreatedAtAction(nameof(Create), new { id = addedRace.Id }, addedRace);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            Race updatedRace = await container.UpdateOneAsync(race);
            return Ok(updatedRace);
        }

        private static async Task<(bool, Race)> InitAndVerifyRace(Race race, RaceContainerClient container)
        {
            if (race.EventId == Guid.Empty)
            {
                race.EventId = Guid.NewGuid();
            }
            else
            {
                var racesInEvent = await container.GetManyAsync(it => it.Where(other => other.EventId == race.EventId));
                if (!racesInEvent.Any())
                {
                    return (false, default(Race));
                }
                else
                {
                    var toCompare = racesInEvent.First();
                    if (toCompare.Name != race.Name ||
                        toCompare.Date != race.Date ||
                        toCompare.Location != race.Location)
                    {
                        return (false, default(Race));
                    }
                }
            }

            race.Submitted = DateTime.UtcNow;
            return (true, race);
        }
    }
}
