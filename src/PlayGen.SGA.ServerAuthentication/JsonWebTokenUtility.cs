﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;

namespace PlayGen.SGA.ServerAuthentication
{
    public class JsonWebTokenUtility
    {
        private readonly string _secretKey;
        private readonly JwtHashAlgorithm _hashAlgorithm;

        public JsonWebTokenUtility(string secretKey, JwtHashAlgorithm hashAlgorithm = JwtHashAlgorithm.HS256)
        {
            _secretKey = secretKey;
            _hashAlgorithm = hashAlgorithm;
        }

        public string CreateToken(Dictionary<string, object> claims)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expiry = Math.Round((DateTime.UtcNow.AddHours(2) - unixEpoch).TotalSeconds);
            var issuedAt = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
            var notBefore = Math.Round((DateTime.UtcNow.AddMonths(6) - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>(claims)
            {
                {"nbf", notBefore},
                {"iat", issuedAt},
                {"exp", expiry},
            };

            return JsonWebToken.Encode(payload, _secretKey, _hashAlgorithm);
        }
    }
}
