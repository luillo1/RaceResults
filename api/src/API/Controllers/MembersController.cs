using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.Extensions.Logging;
using RaceResults.Common.Exceptions;
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
            IEnumerable<Member> result = (await container.GetAllMembersAsDictAsync(orgId)).Values;
            return Ok(result);
        }

        [HttpGet("orgAssignedMemberId/{orgAssignedMemberId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOneMemberFromOrgAssignedMemberId(string orgId, string orgAssignedMemberId)
        {
            if (!await WildApricotController.Authorized(Request, orgAssignedMemberId))
            {
                return Unauthorized();
            }

            MemberContainerClient container = containerProvider.MemberContainer;
            try
            {
                Member result = await container.GetOneMemberAsync(orgAssignedMemberId, orgId);
                return Ok(result);
            }
            catch (MemberIdNotFoundException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetOneMember(string orgId, string memberId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            Member result = await container.GetOneAsync(memberId, orgId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateNewMember(string orgId, Member member)
        {
            if (!await WildApricotController.Authorized(Request, member.OrgAssignedMemberId))
            {
                return Unauthorized();
            }

            member.Id = Guid.NewGuid();
            if (member.OrganizationId != Guid.Parse(orgId))
            {
                return BadRequest();
            }

            MemberContainerClient container = containerProvider.MemberContainer;
            var addedMember = await container.AddOneAsync(member);
            return CreatedAtAction(nameof(CreateNewMember), new { id = addedMember.Id }, addedMember);
        }
    }
}
