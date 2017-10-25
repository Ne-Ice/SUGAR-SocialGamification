﻿using System;
using Microsoft.EntityFrameworkCore;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;

namespace PlayGen.SUGAR.Server.EntityFramework.Controllers
{
	public abstract class DbController
	{
		protected readonly SUGARContextFactory ContextFactory;
		protected readonly DbExceptionHandler DbExceptionHandler = new DbExceptionHandler();

		protected DbController(SUGARContextFactory contextFactory)
		{
			ContextFactory = contextFactory;
		}

		protected void SaveChanges(DbContext context)
		{
			try
			{
				context.SaveChanges();
			}
			catch (Exception exception)
			{
				DbExceptionHandler.Handle(exception);
			}
		}
	}
}
