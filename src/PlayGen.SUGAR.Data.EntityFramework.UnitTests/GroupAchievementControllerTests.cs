﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using Xunit;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	public class GroupAchievementControllerTests : TestController
	{
		#region Configuration
		private readonly AchievementController _achievementDbController;

		public GroupAchievementControllerTests()
		{
			_achievementDbController = new AchievementController(NameOrConnectionString);
		}
		#endregion


		#region Tests
		[Fact]
		public void CreateAndGetGroupAchievement()
		{
			string groupAchievementName = "CreateGroupAchievement";

			var newAchievement = CreateGroupAchievement(groupAchievementName);

			var groupAchievements = _achievementDbController.Get(new int[] { newAchievement.Id });

			int matches = groupAchievements.Count(g => g.Name == groupAchievementName && g.GameId == newAchievement.GameId);

			Assert.Equal(matches, 1);
		}

		[Fact]
		public void CreateGroupAchievementWithNonExistingGame()
		{
			string groupAchievementName = "CreateGroupAchievementWithNonExistingGame";

			bool hadException = false;

			try
			{
				CreateGroupAchievement(groupAchievementName, -1);
			}
			catch (MissingRecordException)
			{
				hadException = true;
			}

			Assert.True(hadException);
		}

		[Fact]
		public void CreateDuplicateGroupAchievement()
		{
			string groupAchievementName = "CreateDuplicateGroupAchievement";

			var firstachievement = CreateGroupAchievement(groupAchievementName);

			bool hadDuplicateException = false;

			try
			{
				CreateGroupAchievement(groupAchievementName, firstachievement.GameId);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}

		[Fact]
		public void GetNonExistingGroupAchievements()
		{
			var groupAchievements = _achievementDbController.Get(new int[] { -1 });

			Assert.Empty(groupAchievements);
		}

		[Fact]
		public void DeleteExistingGroupAchievement()
		{
			string groupAchievementName = "DeleteExistingGroupAchievement";

			var groupAchievement = CreateGroupAchievement(groupAchievementName);
			var groupId = groupAchievement.Id;

			var groupAchievements = _achievementDbController.Get(new int[] { groupId });
			Assert.Equal(groupAchievements.Count(), 1);
			Assert.Equal(groupAchievements.ElementAt(0).Name, groupAchievementName);

			_achievementDbController.Delete(new[] { groupAchievement.Id });
			groupAchievements = _achievementDbController.Get(new int[] { groupId });

			Assert.Empty(groupAchievements);
		}

		[Fact]
		public void DeleteNonExistingGroupAchievement()
		{
			bool hadException = false;

			try
			{
				_achievementDbController.Delete(new int[] { -1 });
			}
			catch (Exception)
			{
				hadException = true;
			}

			Assert.False(hadException);
		}
		#endregion

		#region Helpers
		private Achievement CreateGroupAchievement(string name, int gameId = 0)
		{
			GameController gameDbController = new GameController(NameOrConnectionString);
			if (gameId == 0) {
				Game game = new Game
				{
					Name = name
				};
				gameDbController.Create(game);
				gameId = game.Id;
			}

			var groupAchievement = new Achievement
			{
				Name = name,
				GameId = gameId,
				CompletionCriteriaCollection = new AchievementCriteriaCollection()
			};
			_achievementDbController.Create(groupAchievement);

			return groupAchievement;
		}
		#endregion
	}
}
