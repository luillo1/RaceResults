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
            RaceContainerClient raceContainer = containerProvider.RaceContainer;
            MemberContainerClient memberContainer = containerProvider.MemberContainer;

            IDictionary<Guid, Member> members = await memberContainer.GetAllMembersAsDictAsync(orgId);
            IEnumerable<string> memberIds = members.Values.Select(member => member.Id.ToString());

            RaceResultContainerClient raceResultContainer = containerProvider.RaceResultContainer;

            string startDateParam = Request.Query["startDate"];
            string endDateParam = Request.Query["endDate"];

            DateTime? startDate = null;
            if (DateTime.TryParse(startDateParam, out var parsedStart))
            {
                startDate = parsedStart;
            }

            DateTime? endDate = null;
            if (DateTime.TryParse(endDateParam, out var parsedEnd))
            {
                endDate = parsedEnd;
            }

            IEnumerable<RaceResult> raceResults = 
                await raceResultContainer.GetRaceResultsForMembersAsync(memberIds, startDate, endDate);

            var racesNeeded = raceResults.Select(result => result.RaceId).ToHashSet();
            var racesInResponse = await raceContainer.GetManyAsDictAsync(it => it.Where(race => racesNeeded.Contains(race.Id)));

            var result = raceResults.Select(raceResult => new RaceResultResponse()
            {
                RaceResult = raceResult,
                Member = members[raceResult.MemberId],
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
            if (raceResult.MemberId != Guid.Parse(memberId))
            {
                return BadRequest();
            }

            // Verify member and race exist
            MemberContainerClient memberContainer = containerProvider.MemberContainer;
            RaceContainerClient raceContainer = containerProvider.RaceContainer;

            if (!(await memberContainer.ItemExistsAsync(raceResult.MemberId.ToString(), orgId)))
            {
                return BadRequest($"A member with ID {raceResult.MemberId.ToString()} could not be found");
            }

            if (!(await raceContainer.ItemExistsAsync(raceResult.RaceId.ToString(), raceResult.RaceId.ToString())))
            {
                return BadRequest($"A race with ID {raceResult.RaceId.ToString()} could not be found");
            }

            raceResult.Id = Guid.NewGuid();
            raceResult.Submitted = DateTime.UtcNow;

            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            var addedRaceResult = await container.AddOneAsync(raceResult);
            return CreatedAtAction(nameof(Create), new { id = addedRaceResult.Id }, addedRaceResult);
        }

        [HttpDelete("{resultId}")]
        public async Task<IActionResult> Delete(string orgId, string memberId, string resultId)
        {
            if (!(await MemberBelongsToOrg(orgId, memberId)))
            {
                return BadRequest();
            }

            RaceResultContainerClient container = containerProvider.RaceResultContainer;
            await container.DeleteOneAsync(resultId, memberId);
            return new OkResult();
        }

        private async Task<bool> MemberBelongsToOrg(string orgId, string memberId)
        {
            MemberContainerClient memberContainer = containerProvider.MemberContainer;

            try
            {
                var member = await memberContainer.GetOneAsync(memberId, orgId);
                return member.OrganizationId == Guid.Parse(orgId);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
