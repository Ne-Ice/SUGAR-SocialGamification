﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.Core.Exceptions;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.Core.Controllers
{
	public class EvaluationDataController
	{
		public static Action<EvaluationData> EvaluationDataAddedEvent;

		private readonly ILogger _logger;
		private readonly EntityFramework.Controllers.EvaluationDataController _evaluationDataDbController;
		private readonly EvaluationDataCategory _category;

		public EvaluationDataController(
			ILogger<EvaluationDataController> logger,
			SUGARContextFactory contextFactory, 
			EvaluationDataCategory category)
		{
			_logger = logger;
			_category = category;
			_evaluationDataDbController = new EntityFramework.Controllers.EvaluationDataController(contextFactory, category);
		}

		public void Add(EvaluationData[] datas)
		{
			var dataList = new List<EvaluationData>();
			var i = 0;
			var chunkSize = 1000;

			var uniqueAddedData = new Dictionary<string, EvaluationData>();

			do
			{
				var newData = datas[i];
				ValidateData(newData);
				dataList.Add(newData);
				uniqueAddedData[$"{newData.GameId}_{newData.Key}_{newData.Category}"] = newData;

				if (dataList.Count >= chunkSize || i == datas.Length - 1)
				{
					_evaluationDataDbController.Create(dataList);
					dataList.Clear();
				}

				i++;

			} while (dataList.Count > 0 && i < datas.Length);

			foreach (var addedData in uniqueAddedData.Values)
			{
				EvaluationDataAddedEvent?.Invoke(addedData);
			}

			_logger.LogInformation($"Added: {datas?.Length} Evaluation Datas.");
		}

		public EvaluationData Add(EvaluationData newData)
		{
			ValidateData(newData);

			newData = _evaluationDataDbController.Create(newData);

			_logger.LogInformation($"{newData?.Id}");

			EvaluationDataAddedEvent?.Invoke(newData);

			return newData;
		}

		public EvaluationData Update(EvaluationData data)
		{
			ValidateData(data);

			data = _evaluationDataDbController.Update(data);

			_logger.LogInformation($"Updated: {data?.Id}");

			return data;
		}

		public bool KeyExists(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime),
			DateTime end = default(DateTime))
		{
			var keyExists = _evaluationDataDbController.KeyExists(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Key Exists: {keyExists} for GameId: {gameId}, ActorId: {actorId}, Key: {key}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return keyExists;
		}

		public List<EvaluationData> Get(ICollection<int> ids)
		{
			var datas = _evaluationDataDbController.Get(ids);

			_logger.LogInformation($"{datas.Count} Evaluation Datas for Ids: {string.Join(", ", ids)}");

			return datas;
		}

		public List<EvaluationData> Get(int entityId, string[] keys = null)
		{
			var datas = _evaluationDataDbController.Get(entityId, keys);

			_logger.LogInformation($"{datas?.Count} Evaluation Datas for EntityId {entityId}" +
						(keys != null ? $", Keys: {string.Join(", ", keys)}" : ""));

			return datas;
		}

		public List<EvaluationData> Get(int? gameId = null, int? actorId = null, string[] keys = null)
		{
			var datas = _evaluationDataDbController.Get(gameId, actorId, keys);

			_logger.LogInformation($"{datas?.Count} Evaluation Datas for GameId: {gameId}, ActorId {actorId}" +
						(keys != null ? $", Keys: {string.Join(", ", keys)}" : ""));

			return datas;
		}

		public List<EvaluationData> Get(int? gameId, int? entityId, int? actorId, string[] keys)
		{
			var datas = _evaluationDataDbController.Get(gameId, entityId, actorId, keys);

			_logger.LogInformation($"{datas?.Count} Evaluation Datas for GameId: {gameId}, EntityId: {entityId}, ActorId {actorId}" +
						(keys != null ? $", Keys: {string.Join(", ", keys)}" : ""));

			return datas;
		}

		public List<int?> GetGameActors(int? gameId)
		{
			var actors = _evaluationDataDbController.GetGameActors(gameId);

			_logger.LogInformation($"{actors?.Count} Actors for GameId {gameId}");

			return actors;
		}

		// TODO remove nullable
		public List<int?> GetGameKeyActors(int? gameId, string key)
		{
			var actors = _evaluationDataDbController.GetGameKeyActors(gameId, key);

			_logger.LogInformation($"{actors?.Count} Actors for GameId {gameId} and Key {key}");

			return actors;
		}

		public List<KeyValuePair<string, EvaluationDataType>> GetGameKeys(int? gameId)
		{
			var datas = _evaluationDataDbController.GetGameKeys(gameId);

			_logger.LogInformation($"{datas?.Count} Evaluation Data Keys for GameId {gameId}");

			return datas;
		}

		public List<EvaluationData> GetActorData(int? actorId)
		{
			var datas = _evaluationDataDbController.GetActorData(actorId);

			_logger.LogInformation($"{datas?.Count} Evaluation Data Keys for ActorId {actorId}");

			return datas;
		}

		public List<T> All<T>(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType,
			EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime),
			DateTime end = default(DateTime))
		{
			var all = _evaluationDataDbController.All<T>(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start,
				end);

			_logger.LogDebug(
				$"{all?.Count} Values for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return all;
		}

		public T Sum<T>(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime),
			DateTime end = default(DateTime))
		{
			var sum = _evaluationDataDbController.Sum<T>(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug($"Sum: {sum} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return sum;
		}

		public T Max<T>(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var max = _evaluationDataDbController.Max<T>(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Highest: {max} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return max;
		}

		public T Min<T>(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var min = _evaluationDataDbController.Min<T>(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Highest: {min} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return min;
		}

		public bool TryGetLatest<T>(int? gameId, int? actorId, string key, out T latest, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory,
			DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var didGetLatest = _evaluationDataDbController.TryGetLatest<T>(gameId, actorId, key, out latest, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Latest {latest} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return didGetLatest;
		}

		public EvaluationData GetEvaluationDataByHighestFloat(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var highest = _evaluationDataDbController.GetEvaluationDataByHighestFloat(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				highest != null
					? $"HighestId: {highest.Id} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}"
					: $"Highest not found for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return highest;
		}

		public EvaluationData GetEvaluationDataByHighestLong(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory, DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var highest = _evaluationDataDbController.GetEvaluationDataByHighestLong(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				highest != null
					? $"HighestId: {highest.Id} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}"
					: $"Highest not found for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return highest;
		}

		public int CountKeys(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory,
			DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var count = _evaluationDataDbController.CountKeys(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Count: {count} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return count;
		}

		// todo change to bool TryGet[name](out value) pattern
		public DateTime TryGetEarliestKey(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory,
			DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var didGetEarliestKey = _evaluationDataDbController.TryGetEarliestKey(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start,
				end);

			_logger.LogDebug(
				$"Earliest: {didGetEarliestKey} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return didGetEarliestKey;
		}

		// todo change to bool TryGet[name](out value) pattern
		public DateTime TryGetLatestKey(int? gameId, int? actorId, string key, EvaluationDataType evaluationDataType, EvaluationDataCategory evaluationDataCategory,
			DateTime start = default(DateTime), DateTime end = default(DateTime))
		{
			var didGetLatestKey = _evaluationDataDbController.TryGetLatestKey(gameId, actorId, key, evaluationDataType, evaluationDataCategory, start, end);

			_logger.LogDebug(
				$"Earliest: {didGetLatestKey} for: GameId: {gameId}, ActorId {actorId}, Key: {key}, EvaluationDataType: {evaluationDataType}, EvaludationDataCategory: {evaluationDataCategory}, Start: {start}, End: {end}");

			return didGetLatestKey;
		}

		protected static bool ParseCheck(EvaluationData data)
		{
			switch (data.EvaluationDataType)
			{
				case EvaluationDataType.String:
					return true;

				case EvaluationDataType.Long:
					return long.TryParse(data.Value, out _);

				case EvaluationDataType.Float:
					return float.TryParse(data.Value, out _);

				case EvaluationDataType.Boolean:
					return bool.TryParse(data.Value, out _);

				default:
					return false;
			}
		}

		protected void ValidateData(EvaluationData data)
		{
			if (data.Category != _category)
			{
				throw new InvalidDataException(
					$"Cannot save data with category: {data.Category} with controller for mismatched category: {_category}");
			}

			if (!IsValid(data, out var failure))
			{
				throw new ArgumentException(failure);
			}
		}

		public static bool IsValid(EvaluationData data, out string failure)
		{
			failure = null;

			if (!RegexUtil.IsAlphaNumericUnderscoreNotEmpty(data.Key))
			{
				failure = $"Invalid Key {data.Key}. Keys must be non-empty strings containing only alpha-numeric characters and underscores.";
			} 
			else if (!ParseCheck(data))
			{
				failure = $"Invalid Value {data.Value} for EvaluationDataType {data.EvaluationDataType}";
			}

			return failure == null;
		}
	}
}