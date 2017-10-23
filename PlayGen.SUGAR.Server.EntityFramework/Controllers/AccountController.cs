﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.EntityFramework.Extensions;
using PlayGen.SUGAR.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
	public class AccountController : DbController
	{
		public AccountController(SUGARContextFactory contextFactory)
			: base(contextFactory)
		{
		}

		public Account Create(Account account)
		{
			using (var context = ContextFactory.Create())
			{
				context.HandleDetatchedActor(account.User);

				context.Accounts.Add(account);
				SaveChanges(context);

				return account;
			}
		}

		public List<Account> Get(string[] names, int sourceId)
		{
			using (var context = ContextFactory.Create())
			{
				var accounts = context.Accounts
					.Where(a => names.Contains(a.Name) && a.AccountSourceId == sourceId)
					.Include(a => a.User);

				return accounts.ToList();
			}
		}

		public void Delete(int id)
		{
			using (var context = ContextFactory.Create())
			{

				var account = context.Accounts.Find(context, id);

				if (account == null)
				{
					throw new MissingRecordException($"No account exsits with Id: {id}");
				}

				context.Accounts.Remove(account);
				SaveChanges(context);
			}
		}
	}
}
