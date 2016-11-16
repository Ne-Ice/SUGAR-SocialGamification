﻿using PlayGen.SUGAR.Common.Shared.Permissions;

namespace PlayGen.SUGAR.Data.Model
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ClaimScope ClaimScope { get; set; }
    }
}
