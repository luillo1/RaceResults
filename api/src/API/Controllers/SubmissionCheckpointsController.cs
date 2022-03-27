using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.Authorization;
using RaceResults.Api.Parameters;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations/{orgId}/submissionCheckpoints")]
    public class SubmissionCheckpointsController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<SubmissionCheckpointsController> logger;

        public SubmissionCheckpointsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<SubmissionCheckpointsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(RequireOrganizationAuthorizationAttribute))]
        public async Task<IActionResult> GetSubmissionCheckpoints([OrganizationId] string orgId)
        {
            SubmissionCheckpointContainerClient container = containerProvider.SubmissionCheckpointContainer;
            IEnumerable<SubmissionCheckpoint> result = await container.GetAllSubmissionCheckpointsAsync(orgId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmissionCheckpoint(string orgId, SubmissionCheckpoint checkpoint)
        {
            if (checkpoint.OrganizationId != Guid.Parse(orgId))
            {
                return BadRequest();
            }

            SubmissionCheckpointContainerClient container = containerProvider.SubmissionCheckpointContainer;
            var addedCheckpoint = await container.AddOneAsync(checkpoint);
            return CreatedAtAction(nameof(CreateSubmissionCheckpoint), new { id = addedCheckpoint.Id }, addedCheckpoint);
        }
    }
}
