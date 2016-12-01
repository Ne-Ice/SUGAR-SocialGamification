﻿using System.Collections.Generic;
using PlayGen.SUGAR.Data.Model;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.Core.Authorization;

namespace PlayGen.SUGAR.Core.Controllers
{
	public class ActorClaimController
	{
		private readonly Data.EntityFramework.Controllers.ActorClaimController _actorClaimDbController;
		private readonly ActorRoleController _actorRoleController;
		private readonly RoleClaimController _roleClaimController;

		public ActorClaimController(Data.EntityFramework.Controllers.ActorClaimController actorClaimDbController,
					ActorRoleController actorRoleController,
					RoleClaimController roleClaimController)
		{
			_actorClaimDbController = actorClaimDbController;
			_actorRoleController = actorRoleController;
			_roleClaimController = roleClaimController;
		}

		public ActorClaim Get(int id)
		{
			var claim = _actorClaimDbController.Get(id);
			return claim;
		}

		public IEnumerable<ActorClaim> GetActorClaims(int actorId)
		{
			var claims = _actorClaimDbController.GetActorClaims(actorId).ToList();
			var roles = _actorRoleController.GetActorRoles(actorId, true);

			var roleClaims = roles
				.Select(r => new { actorRole = r, claims = r.Role.RoleClaims.Select(rc => rc.Claim) })
				.SelectMany(x => x.claims.Select(c => new ActorClaim { ActorId = x.actorRole.ActorId, EntityId = x.actorRole.EntityId, ClaimId = c.Id, Claim = c }));

			return claims.Concat(roleClaims).Distinct().ToList();
		}

		public IEnumerable<ActorClaim> GetActorClaimsByScope(int actorId, ClaimScope scope)
		{
			var claims = GetActorClaims(actorId).ToList();
			claims = claims.Where(c => c.Claim.ClaimScope == scope).ToList();
			return claims;
		}

		public IEnumerable<Claim> GetActorClaimsForEntity(int actorId, int? entityId, ClaimScope scope)
		{
			var claims = _actorClaimDbController.GetActorClaimsForEntity(actorId, entityId.Value, scope).ToList();
			var roleClaims = _actorRoleController.GetActorRolesForEntity(actorId, entityId.Value, scope).SelectMany(r => r.RoleClaims).Select(rc => rc.Claim).ToList();
			var totalClaims = claims.Concat(roleClaims).Distinct().ToList();
			return totalClaims;
		}

		public IEnumerable<Actor> GetClaimActors(int claimId, int? entityId)
		{
			var claimActors = _actorClaimDbController.GetClaimActors(claimId, entityId.Value);
			var claimRoles = _roleClaimController.GetRolesByClaim(claimId).Select(cr => cr.Id);
			var roleActors = claimRoles.SelectMany(cr => _actorRoleController.GetRoleActors(cr, entityId.Value)).Distinct();
			return claimActors.Concat(roleActors).Distinct().ToList();
		}

		public ActorClaim Create(ActorClaim newClaim)
		{
			newClaim = _actorClaimDbController.Create(newClaim);
			return newClaim;
		}

		public void Delete(int id)
		{
			_actorClaimDbController.Delete(id);
		}
	}
}
