﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.Data.EntityFramework.Extensions
{
	public static class GameDataExtensions
	{
		public static IQueryable<GameData> GetCategoryData(this SGAContext context, GameDataCategory category)
		{
			return context.GameData.Where(gd => gd.Category == category);
		}

		public static IQueryable<GameData> FilterByGameId(this IQueryable<GameData> gameDataQueryable, int? gameId)
		{
			return gameDataQueryable.Where(gd => gd.GameId == gameId);
		}

		public static IQueryable<GameData> FilterByActorId(this IQueryable<GameData> gameDataQueryable, int? actorId)
		{
			return gameDataQueryable.Where(gd => gd.ActorId == actorId);
		}

		public static IQueryable<GameData> FilterByKey(this IQueryable<GameData> gameDataQueryable, string key)
		{
			return gameDataQueryable.Where(gd => gd.Key.Equals(key));
		}

		public static IQueryable<GameData> FilterByKeys(this IQueryable<GameData> gameDataQueryable, IEnumerable<string> keys)
		{
			return gameDataQueryable.Where(gd => keys.Contains(gd.Key));
		}

		public static IQueryable<GameData> FilterByDataType(this IQueryable<GameData> gameDataQueryable, GameDataType type)
		{
			return gameDataQueryable.Where(gd => gd.DataType == type);
		}

		public static GameData LatestOrDefault(this IQueryable<GameData> gameDataQueryable)
		{
			return gameDataQueryable
				.OrderByDescending(s => s.DateModified)
				.FirstOrDefault();
		}
	}
}