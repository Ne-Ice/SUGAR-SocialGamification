﻿using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model.Interfaces;

namespace PlayGen.SUGAR.Data.Model
{
	public class Achievement
	{
		public int GameId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public ActorType ActorType { get; set; }

		public string Token { get; set; }

		public virtual AchievementCriteriaCollection CompletionCriteriaCollection { get; set; }

		public virtual RewardCollection RewardCollection { get; set; }
	}
}
