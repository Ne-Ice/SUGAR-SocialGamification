﻿using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using PlayGen.SUGAR.Core.Sessions;
using PlayGen.SUGAR.ServerAuthentication.Extensions;
using PlayGen.SUGAR.WebAPI.Attributes;
using PlayGen.SUGAR.WebAPI.Exceptions;
using PlayGen.SUGAR.WebAPI.Extensions;

namespace PlayGen.SUGAR.WebAPI.Filters
{
    public class SessionFilter : IResourceFilter
    {
        private readonly SessionTracker _sessionTracker;

        public SessionFilter(SessionTracker sessionTracker)
        {
            _sessionTracker = sessionTracker;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!HasValidateSessionAttribute(context.ActionDescriptor)) return; 

            int sessionId;

            if (!context.HttpContext.Request.TryGetClaim("SessionId", out sessionId))
            {
                throw new InvalidSessionException("No \"SessionId\" set in the token's claims.");
            }

            if (!_sessionTracker.IsActive(sessionId))
            {
                throw new InvalidSessionException($"Session with id \"{sessionId}\" is not active.");
            }

            _sessionTracker.SetLastActive(sessionId, DateTime.UtcNow);
        }

        private bool HasValidateSessionAttribute(ActionDescriptor actionDescriptor)
        {
            // todo possibly cache these values - do a test first to see the performance benefit with caching vs non caching
            if (actionDescriptor.GetCustomClassAttribute<ValidateSessionAttribute>() != null)
            {
                return true;
            }

            return actionDescriptor.GetCustomMethodAttribute<ValidateSessionAttribute>() != null;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
