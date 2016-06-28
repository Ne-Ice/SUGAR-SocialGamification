﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
	public class UserAchievementController : DbController
	{
		public UserAchievementController(string nameOrConnectionString) : base(nameOrConnectionString)
		{
		}

		public IEnumerable<UserAchievement> GetByGame(int gameId)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var achievements = context.UserAchievements
					.Where(a => a.GameId == gameId).ToList();
				return achievements;
			}
		}

		public IEnumerable<UserAchievement> Get(int[] achievementIds)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var achievements = context.UserAchievements
					.Where(a => achievementIds.Contains(a.Id)).ToList();

				return achievements;
			}
		}

		public UserAchievement Create(UserAchievement achievement)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var hasConflicts = context.UserAchievements
					.Any(a => a.Name == achievement.Name
					&& a.GameId == achievement.GameId);

				if (hasConflicts)
				{
					throw new DuplicateRecordException(string.Format("An achievement with the name {0} for this game already exists.", achievement.Name));
				}

				var gameExists = context.Games
					.Any(g => g.Id == achievement.GameId);

				if (!gameExists)
				{
					throw new MissingRecordException(string.Format("The provided game does not exist."));
				}

				context.UserAchievements.Add(achievement);
				SaveChanges(context);
				return achievement;
			}
		}

		public void Delete(int id)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var achievement = context.UserAchievements
					.Where(g => id == g.Id);

				context.UserAchievements.RemoveRange(achievement);
				SaveChanges(context);
			}
		}
	}
}