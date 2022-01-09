using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Api.Authorization;
using RaceResults.Api.MemberProviders.WildApricot;
using RaceResults.Api.Parameters;
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
        public async Task<IActionResult> GetMembers([OrganizationId] string orgId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            IEnumerable<Member> result = (await container.GetAllMembersAsDictAsync(orgId)).Values;
            return Ok(result);
        }

        [HttpGet("orgAssignedMemberId/{orgAssignedMemberId}")]
        [ServiceFilter(typeof(RequireOrganizationAuthorizationAttribute))]
        public async Task<IActionResult> GetMemberByOrgAssignedMemberId([OrganizationId] string orgId, string orgAssignedMemberId)
        {
            try
            {
                return Ok(await containerProvider.MemberContainer.GetOneMemberAsync(orgAssignedMemberId, orgId));
            }
            catch (MemberIdNotFoundException)
            {
                return BadRequest();
            }
        }

        [HttpPost("orgAssignedMemberId/{orgAssignedMemberId}")]
        [ServiceFilter(typeof(RequireOrganizationAuthorizationAttribute))]
        public async Task<IActionResult> CreateMemberByOrgAssignedMemberId([OrganizationId] string orgId, string orgAssignedMemberId)
        {
            var org = await containerProvider.OrganizationContainer.GetOneAsync(orgId, orgId);
            Member memberToCreate;
            switch (org.AuthType)
            {
                case AuthType.WildApricot:
                    var response = await WildApricotApi.GetMemberModelForLoggedInUser(Request);
                    if (!response.success)
                    {
                        return BadRequest();
                    }
                    memberToCreate = (Member)response.member;
                    memberToCreate.OrganizationId = Guid.Parse(orgId);

                    MemberContainerClient container = containerProvider.MemberContainer;
                    var addedMember = await container.AddOneAsync(memberToCreate);
                    return CreatedAtAction(nameof(CreateMemberByOrgAssignedMemberId), new { id = addedMember.Id }, addedMember);
                case AuthType.RaceResults:
                    // TODO
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException();
            }
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetOneMember(string orgId, string memberId)
        {
            MemberContainerClient container = containerProvider.MemberContainer;
            Member result = await container.GetOneAsync(memberId, orgId);
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

            MemberContainerClient container = containerProvider.MemberContainer;
            var addedMember = await container.AddOneAsync(member);
            return CreatedAtAction(nameof(CreateNewMember), new { id = addedMember.Id }, addedMember);
        }
    }
}
