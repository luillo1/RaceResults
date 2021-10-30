using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Data;
using RaceResults.Models;

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

        [HttpPost]
        public async Task<RaceResult> Post(RaceResult raceResult)
        {
            await this.containerClient.AddItemAsync(raceResult);
            return raceResult;
        }

        [HttpGet]
        public async Task<IEnumerable<RaceResult>> Get()
        {
            IEnumerable<RaceResult> result = await this.containerClient.GetItemsAsync();
            return result;
        }
    }
}
