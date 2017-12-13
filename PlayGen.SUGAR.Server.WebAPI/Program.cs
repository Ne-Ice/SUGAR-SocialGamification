﻿#if !DEBUG
using System;
#endif
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using PlayGen.SUGAR.Server.Core.Authorization;
using PlayGen.SUGAR.Server.Core.EvaluationEvents;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.EntityFramework.Extensions;

namespace PlayGen.SUGAR.Server.WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

#if !DEBUG
			try
			{
#endif
			var host = BuildWebHost(args);

			Setup(host);

			using (var scope = host.Services.CreateScope())
			{
				var environment = scope.ServiceProvider.GetService<IHostingEnvironment>();
				logger.Debug("ContentRootPath: {0}", environment.ContentRootPath);
				logger.Debug("WebRootPath: {0}", environment.WebRootPath);
			}

			host.Run();
#if !DEBUG
			}
			catch (Exception initFailure)
			{
				logger.Error(initFailure);
				throw;
			}
#endif
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseNLog()
				.Build();
		
		public static void Setup(IWebHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var contextFactory = scope.ServiceProvider.GetService<SUGARContextFactory>();
				var environment = scope.ServiceProvider.GetService<IHostingEnvironment>();

				using (var context = contextFactory.Create())
				{
					if (environment.IsEnvironment("Tests"))
					{
						context.Database.EnsureDeleted();
					}

					context.Database.Migrate();
					context.EnsureSeeded();

					var claimController = scope.ServiceProvider.GetService<ClaimController>();
					claimController.GetAuthorizationClaims();

					if (environment.IsEnvironment("Tests"))
					{
						context.EnsureTestsSeeded();
					}

					var evaluationTracker = scope.ServiceProvider.GetService<EvaluationTracker>();
					evaluationTracker.MapExistingEvaluations();
				}
			}
		}
	}
}
