﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.EntityFramework.Controllers
{
	public class RoleClaimController : DbController
	{
		public RoleClaimController(SUGARContextFactory contextFactory)
			: base(contextFactory)
		{
		}

		public List<Claim> GetClaimsByRole(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var claims = context.RoleClaims.Where(rc => id == rc.RoleId).Select(rc => rc.Claim).Distinct().ToList();
				return claims;
			}
		}

		public List<Claim> GetClaimsByRoles(List<int> ids)
		{
			using (var context = ContextFactory.Create())
			{
				var claims = context.RoleClaims.Where(rc => ids.Contains(rc.RoleId)).Select(rc => rc.Claim).Distinct().ToList();
				return claims;
			}
		}

		public List<Role> GetRolesByClaim(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var roles = context.RoleClaims.Where(rc => id == rc.ClaimId).Select(rc => rc.Role).Distinct().ToList();
				return roles;
			}
		}

		public RoleClaim Create(RoleClaim roleClaim)
		{
			using (var context = ContextFactory.Create())
			{
				context.RoleClaims.Add(roleClaim);
				context.SaveChanges();

				return roleClaim;
			}
		}

		public void Delete(int role, int claim)
		{
			using (var context = ContextFactory.Create())
			{
				var roleClaim = context.RoleClaims.SingleOrDefault(r => role == r.RoleId && claim == r.ClaimId);

				context.RoleClaims.Remove(roleClaim);
				context.SaveChanges();
			}
		}
	}
}
