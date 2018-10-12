﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.EntityFramework.Controllers
{
	public class ActorRoleController : DbController
	{
		public ActorRoleController(SUGARContextFactory contextFactory)
			: base(contextFactory)
		{
		}

		public ActorRole Get(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var role = context.ActorRoles.Find(id);
				return role;
			}
		}

		public List<ActorRole> GetActorRolesForEntity(int actorId, int entityId, ClaimScope scope, bool includeClaims = false)
		{
			using (var context = ContextFactory.Create())
			{
				if (includeClaims)
				{
					var roles = context.ActorRoles
						.Include(r => r.Role)
						.ThenInclude(r => r.RoleClaims)
						.ThenInclude(rc => rc.Claim)
						.Where(ar => ar.ActorId == actorId 
									&& (ar.EntityId == entityId || ar.EntityId == Platform.AllId) 
									&& ar.Role.ClaimScope == scope)
						.ToList();

					return roles;
				}
				else
				{
					var roles = context.ActorRoles
					.Where(ar => ar.ActorId == actorId 
								&& (ar.EntityId == entityId || ar.EntityId == Platform.AllId) 
								&& ar.Role.ClaimScope == scope)
						.ToList();

					return roles;
				}
			}
		}

		public List<ActorRole> GetActorRoles(int actorId, bool includeClaims = false)
		{
			using (var context = ContextFactory.Create())
			{
				if (includeClaims)
				{
					var roles = context.ActorRoles
						.Include(r => r.Role)
						.ThenInclude(r => r.RoleClaims)
						.ThenInclude(rc => rc.Claim)
						.Where(ar => ar.ActorId == actorId)
						.ToList();

					return roles;
				}
				else
				{
					var roles = context.ActorRoles
						.Where(ar => ar.ActorId == actorId)
						.ToList();

					return roles;
				}
			}
		}

		public List<Actor> GetRoleActors(int roleId, int entityId)
		{
			using (var context = ContextFactory.Create())
			{
				var actors = context.ActorRoles
					.Where(ar => ar.RoleId == roleId && ar.EntityId == entityId)
					.Select(ar => ar.Actor)
					.Distinct()
					.ToList();

				return actors;
			}
		}

		public ActorRole Create(ActorRole actorRole, SUGARContext context = null)
		{
			var didCreate = false;
			if (context == null)
			{
				context = ContextFactory.Create();
				didCreate = true;
			}

			context.ActorRoles.Add(actorRole);

			if (didCreate)
			{
				context.SaveChanges();
				context.Dispose();
			}

			return actorRole;
		}

		public void Delete(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var actorRole = context.ActorRoles.Find(id);
				if (actorRole == null)
				{
					throw new MissingRecordException($"No ActorRole exists with Id: {id}");
				}
				context.ActorRoles.Remove(actorRole);
				context.SaveChanges();
			}
		}
	}
}
