﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.EntityFramework.Controllers;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.Model;
using Xunit;

namespace PlayGen.SUGAR.Server.EntityFramework.Tests
{
	public class AchievementControllerTests : EntityFrameworkTestBase
	{
		#region Configuration
		private readonly EvaluationController _evaluationController = ControllerLocator.EvaluationController;
		private readonly GameController _gameController = ControllerLocator.GameController;
		#endregion

		#region Tests
		[Fact]
		public void CreateAndGetAchievement()
		{
			var achievementName = "CreateAchievement";

			var newAchievement = Helpers.CreateAchievement(achievementName);

			var achievement = _evaluationController.Get(newAchievement.Token, newAchievement.GameId, EvaluationType.Achievement);

			Assert.Equal(achievementName, achievement.Name);
		}

		[Fact]
		public void CreateAndGetGlobalAchievement()
		{
			var achievementName = "CreateGlobalAchievement";

			var newAchievement = Helpers.CreateAchievement(achievementName, 0);

			var achievement = _evaluationController.Get(newAchievement.Token, newAchievement.GameId, EvaluationType.Achievement);

			Assert.Equal(achievementName, achievement.Name);
		}

		[Fact]
		public void CreateDuplicateAchievement()
		{
			var achievementName = "CreateDuplicateAchievement";

			var firstachievement = Helpers.CreateAchievement(achievementName);

			Assert.Throws<DuplicateRecordException>(() => Helpers.CreateAchievement(achievementName, firstachievement.GameId));
		}

		[Fact]
		public void GetAchievementsByGame()
		{
			var baseAchievement = Helpers.CreateAchievement("GetAchievementsByBaseGame");

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
				Helpers.CreateAchievement(name, gameId);
			}

			var achievements = _evaluationController.GetByGame(gameId);

			var matching = achievements.Where(a => names.Contains(a.Name));

			Assert.Equal(names.Length, matching.Count());
		}

		[Fact]
		public void GetAchievementsByNonExistingGame()
		{
			var achievements = _evaluationController.GetByGame(int.MinValue);

			Assert.Empty(achievements);
		}

		[Fact]
		public void GetNonExistingAchievement()
		{
			var achievement = _evaluationController.Get("GetNonExistingAchievement", -1, EvaluationType.Achievement);

			Assert.Null(achievement);
		}

		[Fact]
		public void UpdateAchievement()
		{
			var achievementName = "UpdateExistingAchievement";

			var newAchievement = Helpers.CreateAchievement(achievementName);

			var foundAchievement = _evaluationController.Get(newAchievement.Token, newAchievement.GameId, EvaluationType.Achievement);

			Assert.NotNull(foundAchievement);
			Assert.NotEqual(newAchievement.Name + "Updated", foundAchievement.Name);

			foundAchievement.Name = newAchievement.Name + "Updated";

			_evaluationController.Update(foundAchievement);

			var updatedAchievement = _evaluationController.Get(newAchievement.Token, newAchievement.GameId, EvaluationType.Achievement);

			Assert.NotEqual(achievementName, updatedAchievement.Name);
			Assert.Equal(foundAchievement.Name, updatedAchievement.Name);
		}

		[Fact]
		public void UpdateAchievementToDuplicateName()
		{
			var achievementName = "UpdateAchievementToDuplicateName";

			var newAchievement = Helpers.CreateAchievement(achievementName);

			var newAchievementDuplicate = Helpers.CreateAchievement(achievementName + " Two", newAchievement.GameId);

			var update = new Achievement {
				Name = achievementName,
				Token = newAchievementDuplicate.Token,
				GameId = newAchievementDuplicate.GameId,
				ActorType = newAchievementDuplicate.ActorType,
				EvaluationCriterias = newAchievementDuplicate.EvaluationCriterias,
				Rewards = newAchievementDuplicate.Rewards
			};

			Assert.Throws<DbUpdateException>(() => _evaluationController.Update(update));
		}

		[Fact]
		public void UpdateNonExistingAchievement()
		{
			var achievementName = "UpdateNonExistingAchievement";

			var achievement = new Achievement
			{
				Id = int.MinValue,
				Name = achievementName,
				Token = achievementName,
				GameId = -1,
				ActorType = ActorType.User,
				EvaluationCriterias = new List<Model.EvaluationCriteria>(),
				Rewards = new List<Model.Reward>()
			};

			Assert.Throws<DbUpdateConcurrencyException>(() => _evaluationController.Update(achievement));
		}

		[Fact]
		public void DeleteExistingAchievement()
		{
			var achievementName = "DeleteExistingAchievement";

			var achievement = Helpers.CreateAchievement(achievementName);

			var achievementReturned = _evaluationController.Get(achievement.Token, achievement.GameId, EvaluationType.Achievement);
			Assert.NotNull(achievementReturned);
			Assert.Equal(achievementReturned.Name, achievementName);

			_evaluationController.Delete(achievement.Token, achievement.GameId, EvaluationType.Achievement);
			achievementReturned = _evaluationController.Get(achievement.Token, achievement.GameId, EvaluationType.Achievement);

			Assert.Null(achievementReturned);
		}

		[Fact]
		public void DeleteNonExistingGroupAchievement()
		{
			_evaluationController.Delete("DeleteNonExistingGroupAchievement", -1, EvaluationType.Achievement);
		}
		#endregion
	}
}
