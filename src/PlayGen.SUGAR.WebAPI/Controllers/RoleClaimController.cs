﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Authorization;
using PlayGen.SUGAR.Common.Permissions;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Core.Authorization;
using PlayGen.SUGAR.WebAPI.Attributes;
using PlayGen.SUGAR.WebAPI.Extensions;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	///     Web Controller that facilitates RoleClaim specific operations.
	/// </summary>
	[Route("api/[controller]")]
	[Authorize("Bearer")]
	[ValidateSession]
	public class RoleClaimController : Controller
	{
		private readonly Core.Controllers.ActorClaimController _actorClaimController;
		private readonly IAuthorizationService _authorizationService;
		private readonly ClaimController _claimController;
		private readonly Core.Controllers.RoleClaimController _roleClaimCoreController;
		private readonly Core.Controllers.RoleController _roleController;

		public RoleClaimController(Core.Controllers.RoleClaimController roleClaimCoreController,
			ClaimController claimController,
			Core.Controllers.RoleController roleController,
			Core.Controllers.ActorClaimController actorClaimController,
			IAuthorizationService authorizationService)
		{
			_roleClaimCoreController = roleClaimCoreController;
			_claimController = claimController;
			_roleController = roleController;
			_actorClaimController = actorClaimController;
			_authorizationService = authorizationService;
		}

		/// <summary>
		///     Get a list of all Claims for this Role.
		///     Example Usage: GET api/roleclaim/role/1
		/// </summary>
		/// <returns>A list of <see cref="ClaimResponse" /> that hold Claim details.</returns>
		[HttpGet("role/{id:int}")]
		//[ResponseType(typeof(IEnumerable<ClaimResponse>))]
		[Authorization(ClaimScope.Role, AuthorizationOperation.Get, AuthorizationOperation.RoleClaim)]
		public IActionResult GetRoleClaims([FromRoute] int id)
		{
			if (_authorizationService.AuthorizeAsync(User, id, (AuthorizationRequirement) HttpContext.Items["Requirements"])
				.Result)
			{
				var roles = _roleClaimCoreController.GetClaimsByRole(id);
				var roleContract = roles.ToCollectionContract();
				return new ObjectResult(roleContract);
			}
			return Forbid();
		}

		/// <summary>
		///     Create a new RoleClaim.
		///     Example Usage: POST api/roleclaim
		/// </summary>
		/// <param name="newRoleClaim"><see cref="RoleClaimRequest" /> object that contains the details of the new RoleClaim.</param>
		/// <returns>A <see cref="RoleClaimResponse" /> containing the new RoleClaim details.</returns>
		[HttpPost]
		//[ResponseType(typeof(RoleClaimResponse))]
		[ArgumentsNotNull]
		[Authorization(ClaimScope.Role, AuthorizationOperation.Create, AuthorizationOperation.RoleClaim)]
		public IActionResult Create([FromBody] RoleClaimRequest newRoleClaim)
		{
			if (_authorizationService.AuthorizeAsync(User,newRoleClaim.RoleId, (AuthorizationRequirement) HttpContext.Items["Requirements"]).Result)
			{
				var role = _roleController.GetById(newRoleClaim.RoleId);
				if (!role.Default)
				{
					var claimScope = _claimController.Get(newRoleClaim.ClaimId)
						.ClaimScope;
					if (role.ClaimScope == claimScope)
					{
						var claims = _actorClaimController.GetActorClaims(int.Parse(User.Identity.Name))
							.Select(c => c.ClaimId);
						if (claims.Contains(newRoleClaim.ClaimId))
						{
							var roleClaim = newRoleClaim.ToModel();
							_roleClaimCoreController.Create(roleClaim);
							var roleContract = roleClaim.ToContract();
							return new ObjectResult(roleContract);
						}
					}
				}
			}
			return Forbid();
		}

		/// <summary>
		///     Delete RoleClaim with the ID provided.
		///     Example Usage: DELETE api/roleclaim/role/1/claim/1
		/// </summary>
		/// <param name="roleId">Role ID.</param>
		/// <param name="claimId">Claim ID.</param>
		[HttpDelete("role/{roleId:int}/claim/{claimId:int}")]
		[Authorization(ClaimScope.Role, AuthorizationOperation.Delete, AuthorizationOperation.RoleClaim)]
		public IActionResult Delete([FromRoute] int roleId, [FromRoute] int claimId)
		{
			if (_authorizationService.AuthorizeAsync(User, roleId, (AuthorizationRequirement) HttpContext.Items["Requirements"]).Result)
			{
				var role = _roleController.GetById(roleId);
				if (!role.Default)
				{
					//Todo: May need to check claims don't become inaccessible due to deletion
					_roleClaimCoreController.Delete(roleId, claimId);
					return Ok();
				}
			}
			return Forbid();
		}
	}
}