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
    public class RunnersController : ControllerBase
    {
        private readonly ICosmosDbContainerClient<Runner> containerClient;

        private readonly ILogger<RunnersController> logger;

        public RunnersController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RunnersController> logger)
        {
            this.containerClient = cosmosDbContainerProvider.RunnerContainer;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Runner> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create(Runner runner)
        {
            return CreatedAtAction(nameof(Create), new { id = runner.Id }, runner);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            return NoContent();
        }
    }
}
