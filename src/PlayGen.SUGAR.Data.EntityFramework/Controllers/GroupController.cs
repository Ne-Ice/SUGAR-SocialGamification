﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.EntityFramework.Extensions;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
	public class GroupController : DbController
	{
		public GroupController(SUGARContextFactory contextFactory) 
			: base(contextFactory)
		{
		}

		public IEnumerable<Group> Get()
		{
			using (var context = ContextFactory.Create())
			{
				var groups = context.Groups.ToList();

				return groups;
			}
		}

		public IEnumerable<Group> Search(string name)
		{
			using (var context = ContextFactory.Create())
			{
				var groups = context.Groups
					.Where(g => g.Name.ToLower().Contains(name.ToLower())).ToList();

				return groups;
			}
		}

		public Group Search(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var group = context.Groups.Find(id);

				return group;
			}
		}

		public void Create(Group group)
		{
			using (var context = ContextFactory.Create())
			{
				context.Groups.Add(group);
				SaveChanges(context);
			}
		}

		public void Update(Group group)
		{
			using (var context = ContextFactory.Create())
			{
				var existing = context.Groups.Find(group.Id);

				if (existing != null)
				{
					context.Entry(existing).State = EntityState.Modified;
					existing.Name = group.Name;
					SaveChanges(context);
				}
				else
				{
					throw new MissingRecordException($"The existing group with ID {group.Id} could not be found.");
				}
			}
		}

		public void Delete(int id)
		{
			using (var context = ContextFactory.Create())
			{
				var group = context.Groups
					.Where(g => id == g.Id);

				context.Groups.RemoveRange(group);
				SaveChanges(context);
			}
		}
	}
}
