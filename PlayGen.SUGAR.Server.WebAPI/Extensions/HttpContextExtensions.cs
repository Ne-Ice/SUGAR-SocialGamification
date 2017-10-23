﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using PlayGen.SUGAR.Server.Authentication.Extensions;

namespace PlayGen.SUGAR.Server.WebAPI.Extensions
{
	public static class HttpContextExtensions
	{
		public static int GetSessionId(this HttpContext context)
		{
			int sessionId;

			if (!context.Request.Headers.TryGetClaim("SessionId", out sessionId))
			{
				throw new ArgumentException("Couldn't get claim \"SessionId\".");
			}

			return sessionId;
		}

		public static IEnumerable<T> GetCustomMethodAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
		{
			var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
			if (controllerActionDescriptor != null)
			{
				return controllerActionDescriptor.MethodInfo.GetCustomAttributes<T>(true);
			}

			return Enumerable.Empty<T>();
		}

		public static T GetCustomMethodAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
		{
			return actionDescriptor.GetCustomMethodAttributes<T>().FirstOrDefault();
		}

		public static IEnumerable<T> GetCustomClassAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
		{
			var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
			if (controllerActionDescriptor != null)
			{
				return controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes<T>(true);
			}

			return Enumerable.Empty<T>();
		}

		public static T GetCustomClassAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
		{
			return actionDescriptor.GetCustomClassAttributes<T>().FirstOrDefault();
		}
	}
}
