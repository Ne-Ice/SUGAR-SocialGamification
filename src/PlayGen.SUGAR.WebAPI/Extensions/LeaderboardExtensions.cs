﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class LeaderboardExtensions
	{
		public static LeaderboardResponse ToContract(this Leaderboard leaderboardModel)
		{
			if (leaderboardModel == null)
				return null;


			return new LeaderboardResponse
			{
				GameId = leaderboardModel.GameId,
				Name = leaderboardModel.Name,
				Token = leaderboardModel.Token,
				EvaluationDataCategory = leaderboardModel.EvaluationDataCategory,
				Key = leaderboardModel.EvaluationDataKey,
				ActorType = leaderboardModel.ActorType,
				EvaluationDataType = leaderboardModel.EvaluationDataType,
				CriteriaScope = leaderboardModel.CriteriaScope,
				LeaderboardType = leaderboardModel.LeaderboardType
			};
		}

		public static CollectionResponse ToCollectionContract(this IEnumerable<Leaderboard> models)
		{
			return new CollectionResponse() {
				Items = models.Select(ToContract).ToArray(),
			};
		}

		public static Leaderboard ToModel(this LeaderboardRequest leaderboardContract)
		{
			return new Leaderboard
			{
				GameId = leaderboardContract.GameId ?? 0,
				Name = leaderboardContract.Name,
				Token = leaderboardContract.Token,
				EvaluationDataCategory = leaderboardContract.EvaluationDataCategory,
				EvaluationDataKey = leaderboardContract.Key,
				ActorType = leaderboardContract.ActorType,
				EvaluationDataType = leaderboardContract.EvaluationDataType,
				CriteriaScope = leaderboardContract.CriteriaScope,
				LeaderboardType = leaderboardContract.LeaderboardType
			};
		}
	}
}