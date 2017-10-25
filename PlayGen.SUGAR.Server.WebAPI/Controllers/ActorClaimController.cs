﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Server.Authorization;
using PlayGen.SUGAR.Server.WebAPI.Attributes;
using PlayGen.SUGAR.Server.WebAPI.Extensions;

namespace PlayGen.SUGAR.Server.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates ActorClaim specific operations.
	/// </summary>
	[Route("api/[controller]")]
	[Authorize("Bearer")]
	[ValidateSession]
	public class ActorClaimController : Controller
	{
		private readonly IAuthorizationService _authorizationService;
		private readonly Core.Controllers.ActorClaimController _actorClaimCoreController;
		private readonly Core.Authorization.ClaimController _claimCoreController;

		public ActorClaimController(Core.Controllers.ActorClaimController actorClaimCoreController,
					Core.Authorization.ClaimController claimCoreController,
					IAuthorizationService authorizationService)
		{
			_actorClaimCoreController = actorClaimCoreController;
			_claimCoreController = claimCoreController;
			_authorizationService = authorizationService;
		}

		/// <summary>
		/// Get a list of all Actors for this Claim and Entity.
		/// 
		/// Example Usage: GET api/actorclaim/claim/1/entity/1
		/// </summary>
		/// <returns>A list of <see cref="ActorResponse"/> that hold ActorClaim details.</returns>
		[HttpGet("claim/{claimId:int}/entity/{entityId:int}")]
		//[ResponseType(typeof(IEnumerable<ActorClaimResponse>))]
		[Authorization(ClaimScope.Global, AuthorizationAction.Get, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Get, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Get, AuthorizationEntity.ActorClaim)]
		public async Task<IActionResult> GetClaimActors([FromRoute]int claimId, [FromRoute]int entityId)
		{
			if (await _authorizationService.AuthorizeAsync(User, Platform.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Global)]) ||
				await _authorizationService.AuthorizeAsync(User, entityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Group)]) || 				
				await _authorizationService.AuthorizeAsync(User, entityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Game)]))
			{
				var actors = _actorClaimCoreController.GetClaimActors(claimId, entityId);
				var actorContract = actors.ToActorContractList();
				return new ObjectResult(actorContract);
			}
			return Forbid();
		}

		/// <summary>
		/// Get a list of all Claims for this Actor.
		/// 
		/// Example Usage: GET api/actorclaim/actor/1/
		/// </summary>
		/// <returns>A list of <see cref="ActorClaimResponse"/> that hold ActorClaim details.</returns>
		[HttpGet("actor/{id:int}")]
		//[ResponseType(typeof(IEnumerable<ActorClaimResponse>))]
		[Authorization(ClaimScope.Group, AuthorizationAction.Get, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.User, AuthorizationAction.Get, AuthorizationEntity.ActorClaim)]
		public async Task<IActionResult> GetActorClaims([FromRoute]int id)
		{
			if (await _authorizationService.AuthorizeAsync(User, id, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Group)]) ||
				await _authorizationService.AuthorizeAsync(User, id, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.User)]))
			{
				var claims = _actorClaimCoreController.GetActorClaims(id);
				var claimsContract = claims.ToContractList();
				return new ObjectResult(claimsContract);
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
		[Authorization(ClaimScope.Global, AuthorizationAction.Create, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Create, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Create, AuthorizationEntity.ActorClaim)]
		public async Task<IActionResult> Create([FromBody]ActorClaimRequest newClaim)
		{
			if (await _authorizationService.AuthorizeAsync(User, Platform.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Global)]) ||
				await _authorizationService.AuthorizeAsync(User, newClaim.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Group)]) ||
				await _authorizationService.AuthorizeAsync(User, newClaim.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Game)]))
			{
				var claimScope = _claimCoreController.Get(newClaim.ClaimId).ClaimScope;
				var creatorClaims = _actorClaimCoreController.GetActorClaimsForEntity(int.Parse(User.Identity.Name), newClaim.EntityId, claimScope);
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
		[Authorization(ClaimScope.Global, AuthorizationAction.Delete, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Group, AuthorizationAction.Delete, AuthorizationEntity.ActorClaim)]
		[Authorization(ClaimScope.Game, AuthorizationAction.Delete, AuthorizationEntity.ActorClaim)]
		public async Task<IActionResult> Delete([FromRoute]int id)
		{
			var actorClaim = _actorClaimCoreController.Get(id);
			if (await _authorizationService.AuthorizeAsync(User, Platform.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Global)]) ||
				await _authorizationService.AuthorizeAsync(User, actorClaim.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Group)]) ||
				await _authorizationService.AuthorizeAsync(User, actorClaim.EntityId, (IAuthorizationRequirement)HttpContext.Items[AuthorizationAttribute.Key(ClaimScope.Game)]))
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
