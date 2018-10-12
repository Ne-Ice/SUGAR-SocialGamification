﻿namespace PlayGen.SUGAR.Contracts
{
	/// <summary>
	/// Encapsulates log-in source details.
	/// </summary>
	/// <example>
	/// JSON
	/// {
	/// "Id" : 1,
	/// "Description" : "SUGAR",
	/// "Token" : "SUGAR",
	/// "RequiresPassword" : true,
	/// "AutoRegister" : false
	/// }
	/// </example>
	public class AccountSourceResponse
	{
		/// <summary>
		/// The unqiue identifier for the game.
		/// </summary>
		public int Id { get; set; }

        /// <summary>
        /// The source description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The source token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Whether the user needs to pass a password when logging in via this source
        /// </summary>
        public bool RequiresPassword { get; set; }

        /// <summary>
        /// Whether an account is created if one does not already exist for this source
        /// </summary>
        public bool AutoRegister { get; set; }
    }
}
