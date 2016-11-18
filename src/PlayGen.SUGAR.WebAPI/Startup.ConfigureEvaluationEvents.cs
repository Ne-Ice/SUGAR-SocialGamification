﻿using Microsoft.Extensions.DependencyInjection;
using PlayGen.SUGAR.Core.EvaluationEvents;

namespace PlayGen.SUGAR.WebAPI
{
    public partial class Startup
    {
        private void ConfigureEvaluationEvents(IServiceCollection services)
        {
            services.AddScoped<EvaluationGameDataMapper>();
            services.AddScoped<ProgressCache>();
            services.AddScoped<ProgressNotificationCache>();
            services.AddScoped<CriteriaEvaluator>();

            services.AddSingleton<EvaluationTracker>();
        }
    }
}
