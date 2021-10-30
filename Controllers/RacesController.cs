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
        public async Task<IEnumerable<Race>> Get()
        {
            IEnumerable<Race> result = await this.containerClient.GetItemsAsync();
            return result;
        }
    }
}
