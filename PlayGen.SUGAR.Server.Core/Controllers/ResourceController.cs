﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.Core.Controllers
{
	public class ResourceController
	{
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		private readonly EvaluationDataController _evaluationDataController;

		public ResourceController(SUGARContextFactory contextFactory)
		{
			_evaluationDataController = new EvaluationDataController(contextFactory, EvaluationDataCategory.Resource);
		}

		public List<EvaluationData> Get(int? gameId = null, int? actorId = null, string[] keys = null)
		{
			var results = _evaluationDataController.Get(gameId, actorId, keys);

			return results;
		}

		public EvaluationData Transfer(int? gameId, int? fromActorId, int? toActorId, string key, long transferQuantity, out EvaluationData fromResource)
		{
			fromResource = GetExistingResource(gameId, fromActorId, key);

			string message;
			if (!IsTransferValid(long.Parse(fromResource.Value), transferQuantity, out message))
			{
				throw new ArgumentException(message);
			}

			fromResource = AddQuantity(fromResource.Id, -transferQuantity);

			EvaluationData toResource;
			var foundResources = _evaluationDataController.Get(gameId, toActorId, new[] { fromResource.Key });

			if (foundResources.Any())
			{
				toResource = foundResources.Single();
				toResource = AddQuantity(toResource.Id, transferQuantity);
			}
			else
			{
				toResource = new EvaluationData {
					GameId = gameId,
					ActorId = toActorId,
					Key = fromResource.Key,
					Value = transferQuantity.ToString(),
					Category = fromResource.Category,
					EvaluationDataType = fromResource.EvaluationDataType,
				};
				Create(toResource);
			}

			Logger.Info($"{fromResource?.Id} -> {toResource?.Id} for GameId: {gameId}, FromActorId: {fromActorId}, ToActorId: {toActorId}, Key: {key}, Quantity: {transferQuantity}");

			return toResource;
		}

		public EvaluationData AddResource(int? gameId, int? ActorId, string key, long Quantity)
		{

			EvaluationData resource;
			var foundResources = _evaluationDataController.Get(gameId, ActorId, new[] {key });

			if (foundResources.Any())
			{
				resource = foundResources.Single();
				if(long.Parse(resource.Value) + Quantity < 0.0)
				{
					Quantity = -long.Parse(resource.Value);
				}
				resource = AddQuantity(resource.Id, Quantity);
			}
			else
			{
				if (Quantity < 0.0)
				{
					Quantity = (long) 0;
				}
				resource = new EvaluationData {
					GameId = gameId,
					ActorId = ActorId,
					Key = key,
					Value = Quantity.ToString(),
					Category = EvaluationDataCategory.Resource,
					//At the moment hard coded to just be longs. Need to think about if boolean and string would make sense
					//for a resource. Floats definitely should be implemented
					EvaluationDataType = EvaluationDataType.Long,
				};
				Create(resource);
			}

			Logger.Info($"{resource?.Id} for GameId: {gameId}, ToActorId: {ActorId}, Key: {key}, Quantity: {Quantity}");

			return resource;
		}

		public void Create(EvaluationData data)
		{
			var existingEntries = _evaluationDataController.Get(data.GameId, data.ActorId, new[] { data.Key });

			if (existingEntries.Any())
			{
				throw new DuplicateRecordException();
			}

			_evaluationDataController.Add(data);

			Logger.Info($"{data?.Id}");
		}

		public EvaluationData AddQuantity(int resourceId, long addAmount)
		{
			var resource = _evaluationDataController.Get(new[] { resourceId }).Single();

			var currentValue = long.Parse(resource.Value);
			resource.Value = (currentValue + addAmount).ToString();

			_evaluationDataController.Update(resource);

			Logger.Info($"{resource?.Id} with Amount: {addAmount}");

			return resource;
		}

		private EvaluationData GetExistingResource(int? gameId, int? ownerId, string key)
		{
			var foundResources = _evaluationDataController.Get(gameId, ownerId, new[] { key });

			if (!foundResources.Any())
			{
				throw new MissingRecordException("No resource with the specified ID was found.");
			}

			var found = foundResources.Single();

			Logger.Info($"{found?.Id}");

			return found;
		}

		private static bool IsTransferValid(long current, long transfer, out string message)
		{
			message = string.Empty;

			if (current < transfer)
			{
				message = "The quantity to transfer cannot be greater than the quantity available to transfer.";
			}
			else if (transfer < 1)
			{
				message = "The quantity to transfer cannot be less than one.";
			}

			var result = message == string.Empty;

			Logger.Debug($"{result} with Message: \"{message}\" for Current: {current}, Transfer {transfer}");

			return result;
		}
	}
}