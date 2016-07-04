﻿using System.Linq;
using System.Collections.Generic;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
	public class LeaderboardController : DbController
	{
		public LeaderboardController(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
		}

		public IEnumerable<Leaderboard> GetGlobal()
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var leaderboards = context.Leaderboards.Where(l => l.GameId == null).ToList();
				return leaderboards;
			}
		}


		public IEnumerable<Leaderboard> GetByGame(int gameId)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var leaderboards = context.Leaderboards.Where(l => l.GameId == gameId).ToList();
				return leaderboards;
			}
		}

		public Leaderboard Get(int leaderboardId)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var leaderboard = context.Leaderboards.Find(leaderboardId);
				return leaderboard;
			}
		}

		public Leaderboard Create(Leaderboard leaderboard)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				//TODO: refine duplicate text for actor type and game id
				var hasConflicts = context.Leaderboards.Any(l => (l.Name == leaderboard.Name && l.GameId == leaderboard.GameId)
									|| (l.Token == leaderboard.Token && l.GameId == leaderboard.GameId));

				if (hasConflicts)
				{
					throw new DuplicateRecordException($"An leaderboard with the name {leaderboard.Name} or token {leaderboard.Token} for this game already exists.");
				}

				context.Leaderboards.Add(leaderboard);
				SaveChanges(context);
				return leaderboard;
			}
		}

		public void Delete(int id)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var leaderboard = context.Leaderboards.Find(id);
				if (leaderboard != null)
				{
					context.Leaderboards.Remove(leaderboard);
					SaveChanges(context);
				}
			}
		}
	}
}