﻿using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Extensions.Logging;
using PlayGen.SUGAR.Core.Utilities;
using PlayGen.SUGAR.ServerAuthentication;
using PlayGen.SUGAR.WebAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.PlatformAbstractions;
using PlayGen.SUGAR.Core.Authorization;
using PlayGen.SUGAR.ServerAuthentication.Filters;

namespace PlayGen.SUGAR.WebAPI
{
	public partial class Startup
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		const string TokenAudience = "User";
		const string TokenIssuer = "SUGAR";
		private RsaSecurityKey key;
		private TokenAuthOptions tokenOptions;


		public Startup(IHostingEnvironment env)
		{
			//AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			#region Logging

			env.ConfigureNLog("NLog.config");
			Logger.Debug("ContentRootPath: {0}", env.ContentRootPath);
			Logger.Debug("WebRootPath: {0}", env.WebRootPath);

			#endregion

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);


			if (env.IsEnvironment("Development"))
			{
				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			builder.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		//private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		//{
		//	Logger.Error($"AppDomain UnhandledException: {e.ExceptionObject}");
		//}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var validityTimeout = JsonConvert.DeserializeObject<TimeSpan>(Configuration["TokenValidityTimeout"]);
			var timeoutCheckInterval = JsonConvert.DeserializeObject<TimeSpan>(Configuration["TimeoutCheckInterval"]);


			//todo: Remove random key. Change to load file from secure file.

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				//var apiKey = Configuration["APIKey"];
				// On desktop CLR, use RSACryptoServiceProvider.
				using (var rsa = new RSACryptoServiceProvider(2048))
				{
					try
					{
						key = new RsaSecurityKey(rsa.ExportParameters(true));
					}
					finally
					{
						rsa.PersistKeyInCsp = false;
					}
				}
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				using (var rsa = new RSAOpenSsl(2048))
				{
					key = new RsaSecurityKey(rsa.ExportParameters(true));
				}
			}

			tokenOptions = new TokenAuthOptions()
			{
				Audience = TokenAudience,
				Issuer = TokenIssuer,
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature),
				ValidityTimeout = validityTimeout,
			};

			services.AddSingleton(tokenOptions);

			services.AddScoped((_) => new PasswordEncryption());
			services.AddApplicationInsightsTelemetry(Configuration);

			services.AddAuthorization(auth =>
			{
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser().Build());
			});

			services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerWithNull>();
			services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerWithoutEntity>();

			// Add framework services.
			services.AddMvc(options =>
			{
				options.Filters.Add(new ModelValidationFilter());
				options.Filters.Add(new ExceptionFilter());
				options.Filters.Add(typeof(WrapResponseFilter));
				options.Filters.Add(typeof(TokenReissueFilter));
				options.Filters.Add(typeof(SessionFilter));
			})
			.AddJsonOptions(json =>
			{
				json.SerializerSettings.Converters.Add(new StringEnumConverter());
				json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			});

			ConfigureDbContextFactory(services);
			ConfigureDbControllers(services);
			ConfigureCoreControllers(services);
			ConfigureGameDataControllers(services);
			ConfigureRouting(services);
			ConfigureDocumentationGeneratorServices(services);
			ConfigureAuthorization(services);
			ConfigureEvaluationEvents(services);
			ConfigureSessionTracking(services, validityTimeout, timeoutCheckInterval);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseJwtBearerAuthentication(new JwtBearerOptions
			{
				// Basic settings - signing key to validate with, audience and issuer.
				TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = key,
					ValidAudience = tokenOptions.Audience,
					ValidIssuer = tokenOptions.Issuer,
					// When receiving a token, check that we've signed it.
					ValidateIssuer = true,
					ValidateIssuerSigningKey = true,
					// When receiving a token, check that it is still valid.
					ValidateLifetime = true,
					// This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
					// when validating the lifetime. As we're creating the tokens locally and validating them on the same 
					// machines which should have synchronised time, this can be set to zero. Where external tokens are
					// used, some leeway here could be useful.
					ClockSkew = TimeSpan.FromMinutes(0),
				}
			});

			app.UseCors("AllowAll");
			app.UseApplicationInsightsRequestTelemetry();
			app.UseApplicationInsightsExceptionTelemetry();
			app.UseMvc();

			ConfigureDocumentationGenerator(app);
		}
	}
}