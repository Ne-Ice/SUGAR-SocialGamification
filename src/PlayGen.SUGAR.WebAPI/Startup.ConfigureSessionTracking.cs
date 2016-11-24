﻿using Microsoft.Extensions.DependencyInjection;
using PlayGen.SUGAR.Core.Sessions;

namespace PlayGen.SUGAR.WebAPI
{
    public partial class Startup
    {
        private void ConfigureSessionTracking(IServiceCollection services)
        {
            services.AddSingleton<SessionTracker>();
        }
    }
}
