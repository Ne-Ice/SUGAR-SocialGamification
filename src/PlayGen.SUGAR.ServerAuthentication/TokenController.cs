﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using PlayGen.SUGAR.Common.Shared.Extensions;
using PlayGen.SUGAR.Core.Sessions;
using PlayGen.SUGAR.ServerAuthentication.Extensions;

namespace PlayGen.SUGAR.ServerAuthentication
{
	public class TokenController
	{
		private readonly TokenAuthOptions _tokenOptions;

		public TokenController(TokenAuthOptions token)
		{
			_tokenOptions = token;
		}

        public void IssueToken(HttpContext context, int sessionId, int gameId, int userId)
        {
            var token = CreateToken(sessionId, gameId, userId);
            context.Response.SetAuthorizationToken(token);
        }

        public void IssueToken(HttpContext context, Session session)
	    {
            var token = CreateToken(session.Id, session.GameId, session.ActorId);
            context.Response.SetAuthorizationToken(token);
        }
        
        private string CreateToken(int sessionId, int? gameId, int userId)
        {
            var expiry = DateTime.UtcNow.Add(_tokenOptions.ValidityTimeout);
            var tok = CreateToken(sessionId.ToString(), gameId.ToInt().ToString(), userId.ToString(), expiry);
            return tok;
        }

        private string CreateToken(string sessionId, string gameId, string userId, DateTime expires)
		{
			var handler = new JwtSecurityTokenHandler();

			var identity = new ClaimsIdentity(
                new GenericIdentity(userId, "TokenAuth"), 
                new[] 
                {
                    new Claim("SessionId", sessionId, ClaimValueTypes.Integer),
                    new Claim("GameId", gameId, ClaimValueTypes.Integer),
                    new Claim("UserId", userId, ClaimValueTypes.Integer),
                    new Claim("Expiry", expires.ToString(), ClaimValueTypes.DateTime),
                });

			var securityToken = handler.CreateToken(new SecurityTokenDescriptor
			{
                Issuer = _tokenOptions.Issuer,
                Audience = _tokenOptions.Audience,
                SigningCredentials = _tokenOptions.SigningCredentials,
				Subject = identity,
				Expires = expires,
			});

			return handler.WriteToken(securityToken);
		}
	}
}
