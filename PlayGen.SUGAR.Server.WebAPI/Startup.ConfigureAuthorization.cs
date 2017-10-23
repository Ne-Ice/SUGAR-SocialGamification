﻿using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PlayGen.SUGAR.Core.Authorization;
using PlayGen.SUGAR.ServerAuthentication;

namespace PlayGen.SUGAR.WebAPI
{
    public partial class Startup
    {
        private void ConfigureAuthorization(IServiceCollection services, TimeSpan validityTimeout)
        {
            var keyBytes = Encoding.ASCII.GetBytes(Configuration["APIKey"]);
            key = new SymmetricSecurityKey(keyBytes);

            tokenOptions = new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                ValidityTimeout = validityTimeout,
            };

            services.AddSingleton(tokenOptions);

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerWithNull>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerWithoutEntity>();

            services.AddScoped<ClaimController>();
            var container = services.BuildServiceProvider();
            var claimController = container.GetService<ClaimController>();
            claimController.GetAuthorizationClaims();
        }
    }
}