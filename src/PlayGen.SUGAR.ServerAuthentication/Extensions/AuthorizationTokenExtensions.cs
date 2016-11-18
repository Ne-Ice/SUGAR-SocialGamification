﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PlayGen.SUGAR.ServerAuthentication.Extensions
{
    public static class AuthorizationTokenExtensions
    {
        public static bool TryGetClaim(this HttpRequest request, string type, out int value)
        {
            string claimValue;
            value = default(int);

            if (TryGetClaim(request, type, ClaimValueTypes.Integer, out claimValue))
            {
                value = int.Parse(claimValue);
                return true;
            }

            return false;
        }

        public static bool TryGetClaim(this HttpRequest request, string type, out DateTime value)
        {
            string claimValue;
            value = default(DateTime);

            if(TryGetClaim(request, type, ClaimValueTypes.DateTime, out claimValue))
            {
                value = DateTime.Parse(claimValue);
                return true;
            }

            return false;
        }

        private static bool TryGetClaim(this HttpRequest request, string type, string valueType, out string value)
        {
            value = null;
            var didGetClaim = false;

            var serializedToken = request.GetAuthorizationToken();

            if (string.IsNullOrWhiteSpace(serializedToken)) return false;

            var handler = new JwtSecurityTokenHandler();
            
            var token = handler.ReadJwtToken(serializedToken);

            foreach (var claim in token.Claims)
            {
                if (claim.Type == type && claim.ValueType == valueType)
                {
                    value = claim.Value;
                    didGetClaim = true;
                    break;
                }
            }

            return didGetClaim;
        }
    }
}
