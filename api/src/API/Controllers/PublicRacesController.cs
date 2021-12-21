using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    [ApiController]
    [Route("races/public")]
    public class PublicRacesController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<RacesController> logger;

        public PublicRacesController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RacesController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            IEnumerable<Race> result = await container.GetManyAsync(it => it.Where(race => race.Public));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Race race)
        {
            RaceContainerClient container = containerProvider.RaceContainer;
            if (!(await InitAndVerifyRace(race, container)))
            {
                return BadRequest();
            }
            
            race.Public = false;

            await container.AddOneAsync(race);
            return CreatedAtAction(nameof(Create), new { id = race.Id }, race);
        }

        internal static async Task<bool> InitAndVerifyRace(Race race, RaceContainerClient container)
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
                    return false;
                }
                else
                {
                    var toCompare = racesInEvent.First();
                    if (toCompare.Name != race.Name ||
                        toCompare.Date != race.Date ||
                        toCompare.Location != race.Location)
                    {
                        return false;
                    }
                }
            }

            race.Submitted = DateTime.UtcNow;
            return true;
        }
    }
}
