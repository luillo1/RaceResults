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
    public class OrganizationsController : ControllerBase
    {
        private readonly ICosmosDbContainerClient<Organization> containerClient;

        private readonly ILogger<OrganizationsController> logger;

        public OrganizationsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<OrganizationsController> logger)
        {
            this.containerClient = cosmosDbContainerProvider.OrganizationContainer;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            Organization result = await this.containerClient.GetItemAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Organization> result = await this.containerClient.GetItemsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Organization organization)
        {
            await this.containerClient.AddItemAsync(organization);
            return CreatedAtAction(nameof(Create), new { id = organization.Id }, organization);
        }
    }
}
