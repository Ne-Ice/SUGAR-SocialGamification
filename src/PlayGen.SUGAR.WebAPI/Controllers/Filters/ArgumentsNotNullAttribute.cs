﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlayGen.SUGAR.WebAPI.Controllers.Filters
{
    public class ArgumentsNotNullAttribute : ActionFilterAttribute
    {
	    public override void OnActionExecuting(ActionExecutingContext context)
	    {
		    if (context.ActionArguments.Count == 0)
		    {
			    context.Result = new BadRequestObjectResult("An argument must be supplied.");
		    }
		    else if (context.ActionArguments.Values.Contains(null))
			{
				context.Result = new BadRequestObjectResult("Arguments must not be null.");
			}
	    }
	}
}
