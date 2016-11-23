﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PlayGen.SUGAR.Authorization;
using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.WebAPI.Filters;
using System.Linq;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates ActorClaim specific operations.
	/// </summary>
	[Route("api/[controller]")]
	[Authorize("Bearer")]
	public class ActorClaimController : Controller
	{
		private readonly IAuthorizationService _authorizationService;
		private readonly Core.Controllers.ActorClaimController _actorClaimCoreController;

		public ActorClaimController(Core.Controllers.ActorClaimController actorClaimCoreController,
					IAuthorizationService authorizationService)
		{
			_actorClaimCoreController = actorClaimCoreController;
			_authorizationService = authorizationService;
		}

		/// <summary>
		/// Get a list of all Actors for this Claim and Entity.
		/// 
		/// Example Usage: GET api/actorclaim/claim/1/entity/1
		/// </summary>
		/// <returns>A list of <see cref="ActorClaimResponse"/> that hold ActorClaim details.</returns>
		[HttpGet("claim/{claimId:int}/entity/{entityId:int}")]
		//[ResponseType(typeof(IEnumerable<ActorClaimResponse>))]
		[Authorization(ClaimScope.Global, AuthorizationOperation.Get, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationOperation.Get, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationOperation.Get, AuthorizationOperation.ActorClaim)]
		public IActionResult GetClaimActors([FromRoute]int claimId, [FromRoute]int entityId)
		{
			if (_authorizationService.AuthorizeAsync(User, -1, (AuthorizationRequirement)HttpContext.Items["GlobalRequirements"]).Result ||
			   _authorizationService.AuthorizeAsync(User, entityId, (AuthorizationRequirement)HttpContext.Items["GroupRequirements"]).Result ||
				_authorizationService.AuthorizeAsync(User, entityId, (AuthorizationRequirement)HttpContext.Items["GameRequirements"]).Result)
			{
				var actors = _actorClaimCoreController.GetClaimActors(claimId, entityId);
				var actorContract = actors.ToContractList();
				return new ObjectResult(actorContract);
			}
			return Forbid();
		}

		/// <summary>
		/// Create a new ActorClaim.
		/// 
		/// Example Usage: POST api/actorclaim
		/// </summary>
		/// <param name="newClaim"><see cref="ActorClaimRequest"/> object that contains the details of the new ActorClaim.</param>
		/// <returns>A <see cref="ActorClaimResponse"/> containing the new ActorClaim details.</returns>
		[HttpPost]
		//[ResponseType(typeof(ActorClaimResponse))]
		[ArgumentsNotNull]
		[Authorization(ClaimScope.Global, AuthorizationOperation.Create, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationOperation.Create, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationOperation.Create, AuthorizationOperation.ActorClaim)]
		public IActionResult Create([FromBody]ActorClaimRequest newClaim)
		{
			if (_authorizationService.AuthorizeAsync(User, -1, (AuthorizationRequirement)HttpContext.Items["GlobalRequirements"]).Result ||
				_authorizationService.AuthorizeAsync(User, newClaim.EntityId, (AuthorizationRequirement)HttpContext.Items["GroupRequirements"]).Result ||
				_authorizationService.AuthorizeAsync(User, newClaim.EntityId, (AuthorizationRequirement)HttpContext.Items["GameRequirements"]).Result)
			{
				var creatorClaims = _actorClaimCoreController.GetActorClaimsForEntity(int.Parse(User.Identity.Name), newClaim.EntityId);
				if (creatorClaims.Select(cc => cc.Id).Contains(newClaim.ClaimId))
				{
					var claim = newClaim.ToModel();
					_actorClaimCoreController.Create(claim);
					var claimContract = claim.ToContract();
					return new ObjectResult(claimContract);
				}
			}
			return Forbid();
		}

		/// <summary>
		/// Delete ActorClaim with the ID provided.
		/// 
		/// Example Usage: DELETE api/actorclaim/1
		/// </summary>
		/// <param name="id">ActorClaim ID.</param>
		[HttpDelete("{id:int}")]
		[Authorization(ClaimScope.Global, AuthorizationOperation.Delete, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationOperation.Delete, AuthorizationOperation.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationOperation.Delete, AuthorizationOperation.ActorClaim)]
		public IActionResult Delete([FromRoute]int id)
		{
			var actorClaim = _actorClaimCoreController.Get(id);
			if (_authorizationService.AuthorizeAsync(User, -1, (AuthorizationRequirement)HttpContext.Items["GlobalRequirements"]).Result ||
				_authorizationService.AuthorizeAsync(User, actorClaim.EntityId, (AuthorizationRequirement)HttpContext.Items["GroupRequirements"]).Result ||
				_authorizationService.AuthorizeAsync(User, actorClaim.EntityId, (AuthorizationRequirement)HttpContext.Items["GameRequirements"]).Result)
			{
				var claimCount = _actorClaimCoreController.GetClaimActors(actorClaim.ClaimId, actorClaim.EntityId.Value).Count();
				if (claimCount > 1)
				{
					_actorClaimCoreController.Delete(id);
					return Ok();
				}
			}
			return Forbid();
		}
	}
}
