﻿using System.ComponentModel.DataAnnotations;

namespace PlayGen.SUGAR.Contracts
{
	/// <summary>
	/// Encapsulates relationship details, including updated status of the relationship.
	/// </summary>
	public class RelationshipStatusUpdate
	{
		[Required]
		public int RequestorId { get; set; }

		[Required]
		public int AcceptorId { get; set; }

		[Required]
		public bool Accepted { get; set; }
	}
}
