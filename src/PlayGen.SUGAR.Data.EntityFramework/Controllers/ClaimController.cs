﻿using System.Collections.Generic;
using System.Reflection;
using PlayGen.SUGAR.Common.Shared.Permissions;
using System.Linq;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Extensions;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
    public class ClaimController : DbController
    {
        public ClaimController(SUGARContextFactory contextFactory) 
			: base(contextFactory)
		{
        }

        public IEnumerable<Claim> Get()
        {
            using (var context = ContextFactory.Create())
            {
                var claims = context.Claims.ToList();
                return claims;
            }
        }

        public void Create(IEnumerable<Claim> claims)
        {
            using (var context = ContextFactory.Create())
            {
                context.Claims.AddRange(claims);
                SaveChanges(context);
            }
        }
    }
}
