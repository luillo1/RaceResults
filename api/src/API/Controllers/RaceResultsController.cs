using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.ResponseObjects;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResults.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("organizations/{orgId}/members/{memberId}/raceresults")]
    public class RaceResultsController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<RaceResultsController> logger;

        public RaceResultsController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<RaceResultsController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRaceResults(string orgId, string memberId)
        {
            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            IEnumerable<RaceResult> result = await container.GetRaceResultsForMemberAsync(memberId);
            return Ok(result);
        }

        [HttpGet("/organizations/{orgId}/raceresults")]
        public async Task<IActionResult> GetAllRaceResults(string orgId)
        {
            MemberContainerClient memberContainer = containerProvider.MemberContainer;
            IEnumerable<Member> members = await memberContainer.GetAllMembersAsync(orgId);
            IEnumerable<string> memberIds = members.Select(member => member.Id.ToString());

            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            IEnumerable<RaceResult> raceResults = await container.GetRaceResultsForMembersAsync(memberIds);

            var racesNeeded = raceResults.Select(result => result.RaceId).ToHashSet();
            var membersNeeded = raceResults.Select(result => result.MemberId);

            var membersInResponse = (await memberContainer.GetMembersAsync(orgId, membersNeeded))
                .ToDictionary(member => member.Id, member => member);
            var racesInResponse = (await containerProvider.RaceContainer.GetManyAsync(it => it.Where(race => racesNeeded.Contains(race.Id))))
                .ToDictionary(race => race.Id, race => race);

            var result = raceResults.Select(raceResult => new RaceResultResponse()
            {
                RaceResult = raceResult,
                Member = membersInResponse[raceResult.MemberId],
                Race = racesInResponse[raceResult.RaceId],
            });
            return Ok(result);
        }

        [HttpGet("{resultId}")]
        public async Task<IActionResult> GetOneRaceResult(string orgId, string memberId, string resultId)
        {
            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            RaceResult result = await container.GetOneAsync(resultId, memberId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(string orgId, string memberId, RaceResult raceResult)
        {
            raceResult.Id = Guid.NewGuid();
            raceResult.Submitted = DateTime.UtcNow;

            // if (raceResult.MemberId != Guid.Parse(memberId))
            // {
            //     return BadRequest();
            // }

            // TODO: Validate that member exists under organization
            // TODO: Validate that race exists
            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            await container.AddOneAsync(raceResult);
            return CreatedAtAction(nameof(Create), new { id = raceResult.Id }, raceResult);
        }
    }
}
