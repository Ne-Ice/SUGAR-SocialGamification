﻿using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.EntityFramework.Extensions;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.EntityFramework.Controllers
{
	public class MatchController : DbController
	{
		public MatchController(SUGARContextFactory contextFactory)
			: base(contextFactory)
		{
		}

		public Match Get(int matchId)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.First(m => m.Id == matchId);
			}
		}

		public List<Match> GetByTime(DateTime? start, DateTime? end)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.FilterByDateTimeRange(start, end)
					.ToList();
			}
		}

		public List<Match> GetByGame(int gameId)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.GameId == gameId)
					.ToList();
			}
		}

		public List<Match> GetByGame(int gameId, DateTime? start, DateTime? end)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.GameId == gameId)
					.FilterByDateTimeRange(start, end)
					.ToList();
			}
		}

		public List<Match> GetByCreator(int creatorId)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.CreatorId == creatorId)
					.ToList();
			}
		}

		public List<Match> GetByCreator(int creatorId, DateTime? start, DateTime? end)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.CreatorId == creatorId)
					.FilterByDateTimeRange(start, end)
					.ToList();
			}
		}

		public List<Match> GetByGameAndCreator(int gameId, int creatorId)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.GameId == gameId && m.CreatorId == creatorId)
					.ToList();
			}
		}

		public List<Match> GetByGameAndCreator(int gameId, int creatorId, DateTime? start, DateTime? end)
		{
			using (var context = ContextFactory.Create())
			{
				return context.Matches
					.IncludeAll()
					.Where(m => m.GameId == gameId && m.CreatorId == creatorId)
					.FilterByDateTimeRange(start, end)
					.ToList();
			}
		}

		public Match Create(Match match)
		{
			using (var context = ContextFactory.Create())
			{
				context.Matches.Add(match);
				context.SaveChanges();

				return context.Matches
					.IncludeAll()
					.First(m => m.Id == match.Id);
			}
		}

		public Match Update(Match match)
		{
			using (var context = ContextFactory.Create())
			{
				context.Matches.Update(match);
				context.SaveChanges();

				return context.Matches
					.IncludeAll()
					.First(m => m.Id == match.Id);
			}
		}

		public void Delete(int matchId)
		{
			using (var context = ContextFactory.Create())
			{
				var match = context.Matches.Find(matchId);
				if (match == null)
				{
					throw new MissingRecordException($"No Match exists with Id: {matchId}");
				}
				context.Matches.Remove(match);
				context.SaveChanges();
			}
		}
	}
}
