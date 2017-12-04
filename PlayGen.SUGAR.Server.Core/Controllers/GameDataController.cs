﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.Core.Controllers
{
	public class GameDataController
	{
		private readonly ILogger _logger;
		private readonly EvaluationDataController _evaluationDataController;
		private readonly ActorController _actorController;

		public GameDataController(
			ILogger<GameDataController> logger,
			ILogger<EvaluationDataController> evaluationDataLogger,
			SUGARContextFactory contextFactory, 
			ActorController actorController)
		{
			_logger = logger;
			_evaluationDataController = new EvaluationDataController(evaluationDataLogger, contextFactory, EvaluationDataCategory.GameData);
			_actorController = actorController;
		}

		public List<EvaluationData> Get(int? gameId, int? actorId, string[] keys)
		{
			return _evaluationDataController.Get(gameId, actorId, keys);
		}

		public List<Actor> GetGameActors(int? gameId)
		{
			var ids = _evaluationDataController.GetGameActors(gameId);
			return ids.Where(a => a != null).Select(a => _actorController.Get(a.Value)).ToList();
		}

		public List<KeyValuePair<string, EvaluationDataType>> GetGameKeys(int? gameId)
		{
			return _evaluationDataController.GetGameKeys(gameId);
		}

		public List<EvaluationData> GetActorData(int? actorId)
		{
			return _evaluationDataController.GetActorData(actorId);
		}

		public EvaluationData Add(EvaluationData newData)
		{
			return _evaluationDataController.Add(newData);
		}

		public void Add(EvaluationData[] evaluationData)
		{
			_evaluationDataController.Add(evaluationData);
		}

		public EvaluationData GetEvaluationDataByHighestFloat(int? gameId, int? actorId, string key, DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			return _evaluationDataController.GetEvaluationDataByHighestFloat(gameId, actorId, key, EvaluationDataType.Float, EvaluationDataCategory.GameData, start, end);
		}

		public EvaluationData GetEvaluationDataByHighestLong(int? gameId, int? actorId, string key,
			DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			return _evaluationDataController.GetEvaluationDataByHighestLong(gameId, actorId, key, EvaluationDataType.Float, EvaluationDataCategory.GameData, start, end);
		}
	}
}