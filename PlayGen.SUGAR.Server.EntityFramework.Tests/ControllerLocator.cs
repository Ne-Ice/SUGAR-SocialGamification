﻿using System.IO;
using Microsoft.Extensions.Configuration;
using PlayGen.SUGAR.Server.EntityFramework.Controllers;

namespace PlayGen.SUGAR.Server.EntityFramework.Tests
{
	public abstract class ControllerLocator
	{
		public static readonly SUGARContextFactory ContextFactory;

		private static AccountController _accountController;
		private static AccountSourceController _accountSourceController;
		private static ActorController _actorController;
		private static ActorClaimController _actorClaimController;
		private static ActorRoleController _actorRoleController;
		private static ClaimController _claimController;
		private static GameController _gameController;
		private static EvaluationDataController _evaluationDataController;
		private static GroupController _groupController;
		private static LeaderboardController _leaderboardController;
		private static EvaluationController _evaluationController;
		private static RoleController _roleController;
		private static RoleClaimController _roleClaimController;
		private static UserController _userController;
		private static MatchController _matchController;
		private static RelationshipController _relationshipController;

		public static AccountController AccountController
			=> _accountController ?? (_accountController = new AccountController(ContextFactory));

		public static AccountSourceController AccountSourceController
			=> _accountSourceController ?? (_accountSourceController = new AccountSourceController(ContextFactory));

		public static EvaluationController EvaluationController
			=> _evaluationController ?? (_evaluationController = new EvaluationController(ContextFactory));

		public static ActorController ActorController
			=> _actorController ?? (_actorController = new ActorController(ContextFactory));

		public static ActorClaimController ActorClaimController
			=> _actorClaimController ?? (_actorClaimController = new ActorClaimController(ContextFactory));

		public static ActorRoleController ActorRoleController
			=> _actorRoleController ?? (_actorRoleController = new ActorRoleController(ContextFactory));

		public static ClaimController ClaimController
			=> _claimController ?? (_claimController = new ClaimController(ContextFactory));

		public static GameController GameController
			=> _gameController ?? (_gameController = new GameController(ContextFactory));

		public static EvaluationDataController EvaluationDataController
			=> _evaluationDataController ?? (_evaluationDataController = new EvaluationDataController(ContextFactory));

		public static GroupController GroupController
			=> _groupController ?? (_groupController = new GroupController(ContextFactory));

		public static LeaderboardController LeaderboardController
			=> _leaderboardController ?? (_leaderboardController = new LeaderboardController(ContextFactory));

		public static RoleController RoleController
			=> _roleController ?? (_roleController = new RoleController(ContextFactory));

		public static RoleClaimController RoleClaimController
			=> _roleClaimController ?? (_roleClaimController = new RoleClaimController(ContextFactory));

		public static UserController UserController
			=> _userController ?? (_userController = new UserController(ContextFactory));

		public static MatchController MatchController
			=> _matchController ?? (_matchController = new MatchController(ContextFactory));

		public static RelationshipController RelationshipController
			=> _relationshipController ?? (_relationshipController = new RelationshipController(ContextFactory));

		static ControllerLocator()
		{
			const string environmentName = "Tests";

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile($"appsettings.{environmentName}.json")
				.Build();
			
			var connectionString = configuration.GetConnectionString("DefaultConnection");
			ContextFactory = new SUGARContextFactory(connectionString);
		}
	}
}