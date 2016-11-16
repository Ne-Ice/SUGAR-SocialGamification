﻿using System.Collections.Generic;

using PlayGen.SUGAR.Data.Model;
using System.Linq;

using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.Data.EntityFramework.Extensions;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
    public class RoleController : DbController
    {
        public RoleController(SUGARContextFactory contextFactory) 
			: base(contextFactory)
		{
        }

        public IEnumerable<Role> Get()
        {
            using (var context = ContextFactory.Create())
            {
                var roles = context.Roles.ToList();
                return roles;
            }
        }

        public IEnumerable<Role> Get(ClaimScope scope)
        {
            using (var context = ContextFactory.Create())
            {
                var roles = context.Roles.Where(r => r.ClaimScope == scope).ToList();
                return roles;
            }
        }

        public Role Get(string roleName)
        {
            using (var context = ContextFactory.Create())
            {
                var role = context.Roles.FirstOrDefault(r => r.Name == roleName);
                return role;
            }
        }

        public Role Get(int id)
        {
            using (var context = ContextFactory.Create())
            {
                var role = context.Roles.Find(context, id);
                return role;
            }
        }

        public Role Create(Role role)
        {
            using (var context = ContextFactory.Create())
            {
                context.Roles.Add(role);
                SaveChanges(context);

                return role;
            }
        }

        public void Delete(int id)
        {
            using (var context = ContextFactory.Create())
            {
                // todo also delete all data associated with this role?
                var role = context.Roles
                    .Where(r => id == r.Id);

                context.Roles.RemoveRange(role);
                SaveChanges(context);
            }
        }
    }
}
