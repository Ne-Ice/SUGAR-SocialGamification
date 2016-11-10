﻿using PlayGen.SUGAR.Common.Shared.Permissions;

namespace PlayGen.SUGAR.Data.Model
{
    public class Claim
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public string Description { get; set; }

        public ClaimScope PermissionType { get; set; }
}
}
