﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.WebAPI.Exceptions;

namespace PlayGen.SUGAR.Server.WebAPI.Filters
{
	/// <summary>
	/// 
	/// </summary>
	public class ExceptionFilter : ExceptionFilterAttribute
	{
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
		{
			var exception = context.Exception;
			var handled = false;

			if (exception is DuplicateRecordException)
			{
				context.Result = new ObjectResult("Invalid data provided.");
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
				handled = true;
			}

			if (exception is InvalidAccountDetailsException)
			{
				context.Result = new ObjectResult(exception.Message);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				handled = true;
			}

			if (exception is InvalidSessionException)
			{
				context.Result = new ObjectResult(exception.Message);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				handled = true;
			}

			if (!handled)
			{ 
				context.Result = new ObjectResult(context.Exception.Message);
				context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}

            Logger.Error(exception.Message);

            context.Exception = null;
			base.OnException(context);
		}
	}
}