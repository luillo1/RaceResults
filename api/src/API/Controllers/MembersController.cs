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
    // [Authorize]
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
            IMemberContainerClient container = containerProvider.MemberContainer;
            IEnumerable<Member> result = await container.GetAllMembersAsync(orgId);
            return Ok(result);
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetOneMember(string orgId, string memberId)
        {
            IMemberContainerClient container = containerProvider.MemberContainer;
            Member result = await container.GetMemberAsync(orgId, memberId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewMember(string orgId, Member member)
        {
            member.Id = Guid.NewGuid();
            if (member.OrganizationId != Guid.Parse(orgId))
            {
                return BadRequest();
            }

            IMemberContainerClient container = containerProvider.MemberContainer;
            await container.AddMemberAsync(orgId, member);
            return CreatedAtAction(nameof(CreateNewMember), new { id = member.Id }, member);
        }
    }
}