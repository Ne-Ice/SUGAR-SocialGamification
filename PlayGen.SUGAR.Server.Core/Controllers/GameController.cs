﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using PlayGen.SUGAR.Common.Permissions;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.Core.Controllers
{
	public class GameController
	{
		private static Logger Logger = LogManager.GetCurrentClassLogger();
		public static event Action<int> GameDeletedEvent;

		private readonly EntityFramework.Controllers.GameController _gameDbController;
		private readonly ActorClaimController _actorClaimController;
		private readonly ActorRoleController _actorRoleController;

		public GameController(EntityFramework.Controllers.GameController gameDbController,
					ActorClaimController actorClaimController,
					ActorRoleController actorRoleController)
		{
			_gameDbController = gameDbController;
			_actorClaimController = actorClaimController;
			_actorRoleController = actorRoleController;
		}

		public List<Game> Get()
		{
			var games = _gameDbController.Get();

			Logger.Info($"Got: {games?.Count} Games");

			return games;
		}

		public List<Game> GetByPermissions(int actorId)
		{
			var games = Get();
			var permissions = _actorClaimController.GetActorClaimsByScope(actorId, ClaimScope.Game).Select(p => p.EntityId).ToList();
			games = games.Where(g => permissions.Contains(g.Id)).ToList();

			Logger.Info($"Got: {games?.Count} Games, for ActorId: {actorId}");

			return games;
		}

		public Game Get(int id)
		{
			var game = _gameDbController.Get(id);

			Logger.Info($"Got: Game: {game?.Id}, for Id: {id}");

			return game;
		}

		public List<Game> Search(string name)
		{
			var games = _gameDbController.Search(name);

			Logger.Info($"Got: {games?.Count} Games, for Name: {name}");

			return games;
		}

		public Game Create(Game newGame, int creatorId)
		{
			newGame = _gameDbController.Create(newGame);
			_actorRoleController.Create(ClaimScope.Game.ToString(), creatorId, newGame.Id);

			Logger.Info($"Created: Game: {newGame?.Id}, for CreatorId: {creatorId}");

			return newGame;
		}

		public void Update(Game game)
		{
			_gameDbController.Update(game);

			Logger.Info($"{game?.Id}");
		}

		public void Delete(int id)
		{
			GameDeletedEvent?.Invoke(id);

			_gameDbController.Delete(id);

			Logger.Info($"{id}");
		}
	}
}
