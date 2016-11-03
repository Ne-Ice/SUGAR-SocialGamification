﻿using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.GameData;
using AchievementController = PlayGen.SUGAR.Data.EntityFramework.Controllers.AchievementController;
using LeaderboardController = PlayGen.SUGAR.Data.EntityFramework.Controllers.LeaderboardController;
using SkillController = PlayGen.SUGAR.Data.EntityFramework.Controllers.SkillController;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	public static class TestEnvironment
	{
		private static readonly string _dbName = "sugarunittests";
		private static readonly SUGARContextFactory ContextFactory;

		private static AccountController _accountController;
		private static AchievementController _achievementController;
		private static ActorController _actorController;
		private static GameController _gameController;
		private static GameDataController _gameDataController;
		private static GroupController _groupController;
		private static GroupRelationshipController _groupRelationshipController;
		private static LeaderboardController _leaderboardController;
		private static ResourceController _resourceController;
		private static SkillController _skillController;
		private static UserController _userController;
		private static UserRelationshipController _userRelationshipController;

		public static  AccountController AccountController
			=> _accountController ?? (_accountController = new AccountController(ContextFactory));
		public static  AchievementController AchievementController
			=> _achievementController ?? (_achievementController = new AchievementController(ContextFactory));
		public static  ActorController ActorController
			=> _actorController ?? (_actorController = new ActorController(ContextFactory));
		public static  GameController GameController 
			=> _gameController ?? (_gameController = new GameController(ContextFactory));
		public static  GameDataController GameDataController 
			=> _gameDataController ?? (_gameDataController = new GameDataController(ContextFactory));
		public static  GroupController GroupController
			=> _groupController ?? (_groupController = new GroupController(ContextFactory));
		public static  GroupRelationshipController GroupRelationshipController
			=>_groupRelationshipController ?? (_groupRelationshipController = new GroupRelationshipController(ContextFactory));
		public static  LeaderboardController LeaderboardController
			=> _leaderboardController ?? (_leaderboardController = new LeaderboardController(ContextFactory));
		public static  ResourceController ResourceController 
			=> _resourceController ?? (_resourceController = new ResourceController(ContextFactory));
		public static  SkillController SkillController
			=> _skillController ?? (_skillController = new SkillController(ContextFactory));
		public static  UserController UserController 
			=> _userController ?? (_userController = new UserController(ContextFactory));
		public static  UserRelationshipController UserRelationshipController
			=> _userRelationshipController ?? (_userRelationshipController = new UserRelationshipController(ContextFactory));
		
		static TestEnvironment()
		{
			var connectionString = GetNameOrConnectionString(_dbName);
			DeleteDatabase();
			ContextFactory = new SUGARContextFactory(connectionString);

		}

		private static string GetNameOrConnectionString(string dbName)
		{
			return "Server=localhost;" +
					"Port=3306;" +
					$"Database={dbName};" +
					"Uid=root;" +
					"Pwd=t0pSECr3t;" +
					"Convert Zero Datetime=true;" +
					"Allow Zero Datetime=true";
		}

		public static void DeleteDatabase()
		{
			using (var context = ContextFactory.Create())
			{
				context.Database.EnsureDeleted();
			}
		}
	}
}