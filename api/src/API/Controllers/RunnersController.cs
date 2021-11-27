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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            Runner result = await this.containerClient.GetItemAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Runner> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Runner runner)
        {
            await this.containerClient.AddItemAsync(runner);
            return CreatedAtAction(nameof(Create), new { id = runner.Id }, runner);
        }
    }
}
