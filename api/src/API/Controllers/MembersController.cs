using System;
using System.Collections.Generic;
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
    [Route("organizations/{orgId}/members")]
    public class MembersController : ControllerBase
    {
        private readonly ICosmosDbContainerProvider containerProvider;

        private readonly ILogger<MembersController> logger;

        public MembersController(
                ICosmosDbContainerProvider cosmosDbContainerProvider,
                ILogger<MembersController> logger)
        {
            this.containerProvider = cosmosDbContainerProvider;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers(string orgId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            IEnumerable<Member> result = await container.GetAllMembersAsync(orgId);
            return Ok(result);
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetOneMember(string orgId, string memberId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            Member result = await container.GetOneAsync(memberId, orgId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("ids/{orgAssignedMemberId}")]
        public async Task<IActionResult> ConvertMemberId(string orgId, string orgAssignedMemberId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            Member result = await container.GetOneMemberAsync(orgAssignedMemberId, orgId);

            return Ok(result.Id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewMember(string orgId, Member member)
        {
            member.Id = Guid.NewGuid();
            if (member.OrganizationId != Guid.Parse(orgId))
            {
                return BadRequest();
            }

            MemberContainerClient container = containerProvider.MemberContainer;
            await container.AddOneAsync(member);
            return CreatedAtAction(nameof(CreateNewMember), new { id = member.Id }, member);
        }
    }
}
