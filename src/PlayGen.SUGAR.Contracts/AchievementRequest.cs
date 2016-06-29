﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayGen.SUGAR.Contracts
{
	/// <summary>
	/// Encapsulates achievement details.
	/// </summary>
	public class AchievementRequest
	{
		[Required]
		public int? GameId { get; set; }

		[Required]
		[StringLength(64)]
		public string Name { get; set; }

		[Required]
		public ActorType ActorType { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		public List<AchievementCriteria> CompletionCriteria { get; set; }

		[Required]
		public List<Reward> Reward { get; set; }
	}
}
