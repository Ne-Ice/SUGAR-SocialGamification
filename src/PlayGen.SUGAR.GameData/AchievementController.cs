﻿using System.Linq;
using System.Collections.Generic;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.EntityFramework.Interfaces;
using PlayGen.SUGAR.Data.Model;


namespace PlayGen.SUGAR.GameData
{
	public class AchievementController : DataEvaluationController
	{
		protected readonly RewardController RewardController;
		protected readonly ActorController ActorController;

		public AchievementController(IGameDataController gameDataController, 
			GroupRelationshipController groupRelationshipController,
			UserRelationshipController userRelationshipController,
			ActorController actorController, 
			RewardController rewardController)
			: base(gameDataController, groupRelationshipController, userRelationshipController)
		{
			RewardController = rewardController;
			ActorController = actorController;
		}


		public IEnumerable<Achievement> FilterByActorType(IEnumerable<Achievement> achievements, int? actorId)
		{
			if (actorId.HasValue)
			{
				var provided = ActorController.Get(actorId.Value);
				if (provided == null)
				{
					achievements = achievements.Where(a => a.ActorType == ActorType.Undefined);
				}
				else
				{
					achievements = achievements.Where(a => a.ActorType == ActorType.Undefined || a.ActorType == provided.ActorType);
				}
			}
			
			return achievements;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="achievement"></param>
		/// <param name="actorId"></param>
		/// <returns></returns>
		public bool IsAchievementCompleted(Achievement achievement, int? actorId)
		{
			if (achievement == null)
			{
				throw new MissingRecordException("The provided achievement does not exist.");
			}
			if (actorId.HasValue) {
				var provided = ActorController.Get(actorId.Value);
				if (achievement.ActorType != ActorType.Undefined && (provided == null || provided.ActorType != achievement.ActorType))
				{
					throw new MissingRecordException("The provided ActorId cannot complete this achievement.");
				}
			}
			var key = string.Format(KeyConstants.AchievementCompleteFormat, achievement.Token);
			var completed =  GameDataController.KeyExists(achievement.GameId, actorId, key);

			if (!completed)
			{
				completed = IsCriteriaSatisified(achievement.GameId, actorId, achievement.CompletionCriteriaCollection, achievement.ActorType);
				if (completed)
				{
					ProcessAchievementRewards(achievement, actorId);
				}
			}

			return completed;
		}

		private void ProcessAchievementRewards(Achievement achievement, int? actorId)
		{
			var gameData = new Data.Model.GameData()
			{
				Key = string.Format(KeyConstants.AchievementCompleteFormat, achievement.Token),
				GameId = achievement.GameId,    //TODO: handle the case where a global achievement has been completed for a specific game
				ActorId = actorId,
				DataType = GameDataType.String,
				Value = null
			};
			GameDataController.Create(gameData);
			achievement.RewardCollection.All(reward => RewardController.AddReward(actorId, achievement.GameId, reward));
		}

		public void EvaluateAchievement(Achievement achievement, int? actorId)
		{
		}
	}
}