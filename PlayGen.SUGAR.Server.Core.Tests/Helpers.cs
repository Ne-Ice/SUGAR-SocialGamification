﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.Core.Controllers;
using PlayGen.SUGAR.Server.Model;
using EvaluationCriteria = PlayGen.SUGAR.Server.Model.EvaluationCriteria;
using DbControllerLocator = PlayGen.SUGAR.Server.EntityFramework.Tests.ControllerLocator;

namespace PlayGen.SUGAR.Server.Core.Tests
{
	public static class Helpers
	{
		public static User GetOrCreateUser(string name)
		{
			var existingUser = ControllerLocator.UserController.GetExistingUser(name);

			if (existingUser != null)
			{
				return existingUser;
			}
			else
			{
				return ControllerLocator.UserController.Create(new User
				{
					Name = name,
				});
			}
		}

		public static Game GetOrCreateGame(string name)
		{
			Game game;
			var games = ControllerLocator.GameController.Search(name);

			if (games.Any())
			{
				game = games.ElementAt(0);
			}
			else
			{
				game = ControllerLocator.GameController.Create(new Game
				{
					Name = name,
				}, 1);
			}

			return game;
		}

		public static Evaluation ComposeGenericAchievement(string key, int gameId, int evaluationCriteriaCount = 1)
		{
			var evaluationCriterias = new List<EvaluationCriteria>();
			for (var i = 0; i < evaluationCriteriaCount; i++)
			{
				evaluationCriterias.Add(new EvaluationCriteria
				{
					EvaluationDataKey = $"{key}_{i}",
					EvaluationDataType = EvaluationDataType.Long,
					CriteriaQueryType = CriteriaQueryType.Sum,
					ComparisonType = ComparisonType.GreaterOrEqual,
					Scope = CriteriaScope.Actor,
					Value = "100"
				});
			}

			return new Achievement
			{
				// Arrange
				Token = key,

				Name = key,
				Description = key,

				ActorType = ActorType.User,
				GameId = gameId,

				EvaluationCriterias = evaluationCriterias
			};
		}

		public static List<EvaluationData> ComposeAchievementGameDatas(int actorId, Evaluation evaluation, string value = "50")
		{
			var gameDatas = new List<EvaluationData>();

			foreach (var criteria in evaluation.EvaluationCriterias)
			{
				gameDatas.Add(ComposeEvaluationData(actorId, criteria, evaluation.GameId, value));
			}

			return gameDatas;
		}

		public static EvaluationData ComposeEvaluationData(int actorId, EvaluationCriteria evaluationCriteria, int gameId, string value = "50")
		{
			return new EvaluationData
			{
				Key = evaluationCriteria.EvaluationDataKey,
				EvaluationDataType = evaluationCriteria.EvaluationDataType,

				ActorId = actorId,
				GameId = gameId,

				Value = value
			};
		}

		public static Evaluation CreateGenericAchievement(string key, int gameId)
		{
			return ControllerLocator.EvaluationController.Create(ComposeGenericAchievement(key, gameId));
		}

		public static void CreateGenericAchievementGameData(Evaluation evaluation, int actorId, string value = "50")
		{
			var gameDatas = ComposeAchievementGameDatas(actorId, evaluation, value);
			
			var evaluationDataController = new EvaluationDataController(new NullLogger<EvaluationDataController>(), DbControllerLocator.ContextFactory, evaluation.EvaluationCriterias[0].EvaluationDataCategory);
			evaluationDataController.Add(gameDatas);
		}

		public static void CompleteGenericAchievement(Evaluation evaluation, int actorId)
		{
			var gameDatas = ComposeAchievementGameDatas(actorId, evaluation, "100");

			var evaluationDataController = new EvaluationDataController(new NullLogger<EvaluationDataController>(), DbControllerLocator.ContextFactory, evaluation.EvaluationCriterias[0].EvaluationDataCategory);
			evaluationDataController.Add(gameDatas);
		}

		public static Evaluation CreateAndCompleteGenericAchievement(string key, int actorId, int gameId)
		{
			var evaluation = CreateGenericAchievement(key, gameId);
			CompleteGenericAchievement(evaluation, actorId);

			return evaluation;
		}
	}
}