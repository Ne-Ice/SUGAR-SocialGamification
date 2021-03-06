﻿using System.ComponentModel.DataAnnotations;

namespace PlayGen.SUGAR.Contracts
{
	/// <summary>
	/// Encapsulates log-in source details.
	/// </summary>
	/// <example>
	/// JSON
	/// {
	/// "Description" : "SUGAR",
	/// "Token" : "SUGAR",
	/// "RequiresPassword" : true,
	/// "AutoRegister" : false
	/// }
	/// </example>
	public class AccountSourceRequest
    {
        /// <summary>
        /// The source description.
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Description { get; set; }

        /// <summary>
        /// The source token.
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Token { get; set; }

		/// <summary>
		/// Whether the user needs to pass a password when logging in via this source
		/// </summary>
		[Required]
		public bool? RequiresPassword { get; set; }

		/// <summary>
		/// Whether an account is created if one does not already exist for this source
		/// </summary>
		[Required]
		public bool? AutoRegister { get; set; }
    }
}

