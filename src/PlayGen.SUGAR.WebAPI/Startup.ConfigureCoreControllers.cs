﻿using Microsoft.Extensions.DependencyInjection;
using PlayGen.SUGAR.Core.Controllers;

namespace PlayGen.SUGAR.WebAPI
{
    public partial class Startup
    {
        private void ConfigureCoreControllers(IServiceCollection services)
        {
            services.AddScoped<AccountController>();
            services.AddScoped<EvaluationController>();
            services.AddScoped<GameController>();
            services.AddScoped<GroupController>();
            services.AddScoped<GroupMemberController>();
            services.AddScoped<UserController>();
            services.AddScoped<UserFriendController>();
        }
    }
}
