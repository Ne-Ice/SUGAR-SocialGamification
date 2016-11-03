﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using Xunit;
using PlayGen.SUGAR.Common.Shared;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
    [Collection("Project Fixture Collection")]
    public class AchievementControllerTests
	{
		#region Configuration
		private readonly AchievementController _achievementController = ControllerLocator.AchievementController;
		private readonly GameController _gameController = ControllerLocator.GameController;
		#endregion
		
		#region Tests
		[Fact]
		public void CreateAndGetAchievement()
		{
			var achievementName = "CreateAchievement";

			var newAchievement = CreateAchievement(achievementName);

			var achievement = _achievementController.Get(newAchievement.Token, newAchievement.GameId);

			Assert.Equal(achievementName, achievement.Name);
		}

		[Fact]
		public void CreateAndGetGlobalAchievement()
		{
			var achievementName = "CreateGlobalAchievement";

			var newAchievement = CreateAchievement(achievementName, 0);

			var achievement = _achievementController.Get(newAchievement.Token, newAchievement.GameId);

			Assert.Equal(achievementName, achievement.Name);
		}

		[Fact]
		public void CreateDuplicateAchievement()
		{
			var achievementName = "CreateDuplicateAchievement";

			var firstachievement = CreateAchievement(achievementName);

			Assert.Throws<DuplicateRecordException>(() => CreateAchievement(achievementName, firstachievement.GameId));
		}

		[Fact]
		public void GetAchievementsByGame()
		{
			var baseAchievement = CreateAchievement("GetAchievementsByBaseGame");

			var gameId = baseAchievement.GameId;

			var names = new[]
			{
				"GetAchievementsByGame1",
				"GetAchievementsByGame2",
				"GetAchievementsByGame3",
				"GetAchievementsByGame4",
			};

			foreach (var name in names)
			{
				CreateAchievement(name, gameId);
			}

			var achievements = _achievementController.GetByGame(gameId);

			var matching = achievements.Where(a => names.Contains(a.Name));

			Assert.Equal(names.Length, matching.Count());
		}

		[Fact]
		public void GetAchievementsByNonExistingGame()
		{
			var achievements = _achievementController.GetByGame(-1);

			Assert.Empty(achievements);
		}

		[Fact]
		public void GetNonExistingAchievement()
		{
			var achievement = _achievementController.Get("GetNonExistingAchievement", -1);

			Assert.Null(achievement);
		}

		[Fact]
		public void UpdateAchievement()
		{
			var achievementName = "UpdateExistingAchievement";

			var newAchievement = CreateAchievement(achievementName);

			var foundAchievement = _achievementController.Get(newAchievement.Token, newAchievement.GameId);

			Assert.NotNull(foundAchievement);

			var update = new Achievement
			{
				Name = newAchievement.Name + "Updated",
				Token = newAchievement.Token,
				GameId = newAchievement.GameId,
				ActorType = newAchievement.ActorType,
				CompletionCriterias = newAchievement.CompletionCriterias,
				Rewards = newAchievement.Rewards
			};

			_achievementController.Update(update);

			var updatedAchievement = _achievementController.Get(newAchievement.Token, newAchievement.GameId);

			Assert.NotEqual(foundAchievement.Name, updatedAchievement.Name);
			Assert.Equal(foundAchievement.Name + "Updated", updatedAchievement.Name);
		}

		[Fact]
		public void UpdateAchievementToDuplicateName()
		{
			var achievementName = "UpdateAchievementToDuplicateName";

			var newAchievement = CreateAchievement(achievementName);

			var newAchievementDuplicate = CreateAchievement(achievementName + " Two", newAchievement.GameId);

			var update = new Achievement
			{
				Name = achievementName,
				Token = newAchievementDuplicate.Token,
				GameId = newAchievementDuplicate.GameId,
				ActorType = newAchievementDuplicate.ActorType,
				CompletionCriterias = newAchievementDuplicate.CompletionCriterias,
				Rewards = newAchievementDuplicate.Rewards
			};

			Assert.Throws<DuplicateRecordException>(() => _achievementController.Update(update));
		}

		[Fact]
		public void UpdateNonExistingAchievement()
		{
			var achievementName = "UpdateNonExistingAchievement";

			var achievement = new Achievement
			{
				Name = achievementName,
				Token = achievementName,
				GameId = -1,
				ActorType = ActorType.User,
				CompletionCriterias = new List<Model.CompletionCriteria>(),
				Rewards = new List<Model.Reward>()
			};

			Assert.Throws<MissingRecordException>(() => _achievementController.Update(achievement));
		}

		[Fact]
		public void DeleteExistingAchievement()
		{
			var achievementName = "DeleteExistingAchievement";

			var achievement = CreateAchievement(achievementName);

			var achievementReturned = _achievementController.Get(achievement.Token, achievement.GameId);
			Assert.NotNull(achievementReturned);
			Assert.Equal(achievementReturned.Name, achievementName);

			_achievementController.Delete(achievement.Token, achievement.GameId);
			achievementReturned = _achievementController.Get(achievement.Token, achievement.GameId);

			Assert.Null(achievementReturned);
		}

		[Fact]
		public void DeleteNonExistingGroupAchievement()
		{
			_achievementController.Delete("DeleteNonExistingGroupAchievement", -1);
		}
		#endregion

		#region Helpers
		private Achievement CreateAchievement(string name, int? gameId = null, bool addCriteria = true)
		{
			if (gameId == null) {
				var game = new Game
				{
					Name = name
				};
				_gameController.Create(game);
				gameId = game.Id;
			}

			var achievement = new Achievement
			{
				Name = name,
				Token = name,
				GameId = gameId.Value,
				ActorType = ActorType.User,
				CompletionCriterias = new List<Model.CompletionCriteria>(),
				Rewards = new List<Model.Reward>()
			};
			if (addCriteria)
			{
				var criteria = new List<Model.CompletionCriteria>
				{
					new Model.CompletionCriteria
                    {
						Key = "CreateAchievementKey",
						DataType = GameDataType.String,
						CriteriaQueryType = CriteriaQueryType.Any,
						ComparisonType = ComparisonType.Equals,
						Scope = CriteriaScope.Actor,
						Value = "CreateAchievementValue"
					}
				};
				achievement.CompletionCriterias = criteria;
			}

			_achievementController.Create(achievement);

			return achievement;
		}
		#endregion
	}
}
 