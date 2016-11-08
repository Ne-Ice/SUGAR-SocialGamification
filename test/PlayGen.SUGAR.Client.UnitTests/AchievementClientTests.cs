﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using NUnit.Framework;
using CompletionCriteria = PlayGen.SUGAR.Contracts.Shared.CompletionCriteria;

namespace PlayGen.SUGAR.Client.UnitTests
{
	public class AchievementClientTests 
	{
		#region Configuration
		private readonly AchievementClient _achievementClient;
		private readonly GameDataClient _gameDataClient;
		private readonly UserClient _userClient;
		private readonly GameClient _gameClient;

		public AchievementClientTests()
		{
			var testSugarClient = new TestSUGARClient();
			_achievementClient = testSugarClient.Achievement;
			_gameDataClient = testSugarClient.GameData;
			_userClient = testSugarClient.User;
			_gameClient = testSugarClient.Game;

			RegisterAndLogin(testSugarClient.Account);
		}

		private void RegisterAndLogin(AccountClient client)
		{
			var accountRequest = new AccountRequest
			{
				Name = "AchievementClientTests",
				Password = "AchievementClientTestsPassword",
				AutoLogin = true,
			};

			try
			{
				client.Login(accountRequest);
			}
			catch
			{
				client.Register(accountRequest);
			}
		}
		#endregion

		#region Tests
		[Test]
		public void CanCreateAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanCreateAchievement",
				ActorType = ActorType.User,
				Token = "CanCreateAchievement",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria
					{
						Key = "CanCreateAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			Assert.AreEqual(achievementRequest.Token, response.Token);
			Assert.AreEqual(achievementRequest.ActorType, response.ActorType);
		}

		public void CanCreateGlobalAchievement()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanCreateGlobalAchievement",
				ActorType = ActorType.User,
				Token = "CanCreateGlobalAchievement",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanCreateGlobalAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			Assert.AreEqual(achievementRequest.Token, response.Token);
			Assert.AreEqual(achievementRequest.ActorType, response.ActorType);
		}

		[Test]
		public void CannotCreateDuplicateAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateDuplicateAchievement",
				ActorType = ActorType.User,
				Token = "CannotCreateDuplicateAchievement",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotCreateDuplicateAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			_achievementClient.Create(achievementRequest);

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoName()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoName",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotCreateAchievementWithNoName",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoToken()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoToken",
				ActorType = ActorType.User,
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotCreateAchievementWithNoToken",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoCompletionCriteria()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoCompletionCriteria",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoCompletionCriteria",
				GameId = game.Id,
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoCompletionCriteriaKey()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoCompletionCriteriaKey",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoCompletionCriteriaKey",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoCompletionCriteriaValue()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoCompletionCriteriaValue",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoCompletionCriteriaValue",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotCreateAchievementWithNoCompletionCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CannotCreateAchievementWithNoCompletionCriteriaDataTypeMismatch()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotCreateAchievementWithNoCompletionCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "A string"
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Test]
		public void CanGetAchievementsByGame()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "GameGet");

			var achievementRequestOne = new AchievementRequest()
			{
				Name = "CanGetAchievementsByGameOne",
				ActorType = ActorType.User,
				Token = "CanGetAchievementsByGameOne",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetAchievementsByGameOne",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var responseOne = _achievementClient.Create(achievementRequestOne);

			var achievementRequestTwo = new AchievementRequest()
			{
				Name = "CanGetAchievementsByGameTwo",
				ActorType = ActorType.User,
				Token = "CanGetAchievementsByGameTwo",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetAchievementsByGameTwo",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var responseTwo = _achievementClient.Create(achievementRequestTwo);

			var getAchievement = _achievementClient.GetByGame(game.Id);

			Assert.AreEqual(2, getAchievement.Count());
		}

		[Test]
		public void CanGetAchievementByKeys()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetAchievementByKeys",
				ActorType = ActorType.User,
				Token = "CanGetAchievementByKeys",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetAchievementByKeys",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var getAchievement = _achievementClient.GetById(achievementRequest.Token, achievementRequest.GameId.Value);

			Assert.AreEqual(response.Name, getAchievement.Name);
			Assert.AreEqual(achievementRequest.Name, getAchievement.Name);
		}

		[Test]
		public void CannotGetNotExistingAchievementByKeys()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var getAchievement = _achievementClient.GetById("CannotGetNotExistingAchievementByKeys", game.Id);

			Assert.Null(getAchievement);
		}

		[Test]
		public void CannotGetAchievementByEmptyToken()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			Assert.Throws<ClientException>(() => _achievementClient.GetById("", game.Id));
		}

		[Test]
		public void CanGetAchievementByKeysThatContainSlashes()
		{
            // todo this test seems incorrect
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var getAchievement = _achievementClient.GetById("Can/Get/Achievement/By/Keys/That/Contain/Slashes", game.Id);

			Assert.Null(getAchievement);
		}

		[Test]
		public void CanGetGlobalAchievementByToken()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetGlobalAchievementByToken",
				ActorType = ActorType.User,
				Token = "CanGetGlobalAchievementByToken",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetGlobalAchievementByToken",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var getAchievement = _achievementClient.GetGlobalById(achievementRequest.Token);

			Assert.AreEqual(response.Name, getAchievement.Name);
			Assert.AreEqual(achievementRequest.Name, getAchievement.Name);
		}

		[Test]
		public void CannotGetNotExistingGlobalAchievementByKeys()
		{
			var getAchievement = _achievementClient.GetGlobalById("CannotGetNotExistingGlobalAchievementByKeys");

			Assert.Null(getAchievement);
		}

		[Test]
		public void CannotGetGlobalAchievementByEmptyToken()
		{
			Assert.Throws<ClientException>(() => _achievementClient.GetGlobalById(""));
		}

		[Test]
		public void CanGetGlobalAchievementByKeysThatContainSlashes()
		{
            // todo this test seems incorrect
			var getAchievement = _achievementClient.GetGlobalById("Can/Get/Achievement/By/Keys/That/Contain/Slashes");

            // todo should this not be an exception?
			Assert.Null(getAchievement);
		}

		[Test]
		public void CannotGetByAchievementsByNotExistingGameId()
		{
			var getAchievements = _achievementClient.GetByGame(-1);

			Assert.IsEmpty(getAchievements);
		}

		[Test]
		public void CanUpdateAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanUpdateAchievement",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CanUpdateAchievement",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanUpdateAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CanUpdateAchievement Updated",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CanUpdateAchievement",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanUpdateAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			_achievementClient.Update(updateRequest);

			var updateResponse = _achievementClient.GetById(achievementRequest.Token, achievementRequest.GameId.Value);

			Assert.AreNotEqual(response.Name, updateResponse.Name);
			Assert.AreEqual("CanUpdateAchievement Updated", updateResponse.Name);
		}

		[Test]
		public void CannotUpdateAchievementToDuplicateName()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequestOne = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementToDuplicateNameOne",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementToDuplicateNameOne",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementToDuplicateNameOne",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var responseOne = _achievementClient.Create(achievementRequestOne);

			var achievementRequestTwo = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementToDuplicateNameTwo",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementToDuplicateNameTwo",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementToDuplicateNameTwo",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var responseTwo = _achievementClient.Create(achievementRequestTwo);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementToDuplicateNameOne",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementToDuplicateNameTwo",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementToDuplicateNameTwo",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateNonExistingAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateNonExistingAchievement",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateNonExistingAchievement",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateNonExistingAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievemenWithNoName()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievemenWithNoName",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievemenWithNoName",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievemenWithNoName",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievemenWithNoName",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievemenWithNoName",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievementWithNoToken()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoToken",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoToken",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoToken",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoToken",
				ActorType = ActorType.User,
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoToken",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievementWithNoCompletionCriteria()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteria",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteria",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteria",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteria",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteria",
				GameId = game.Id,
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievementWithNoCompletionCriteriaKey()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaKey",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaKey",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteriaKey",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaKey",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaKey",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievementWithNoCompletionCriteriaValue()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CannotUpdateAchievementWithNoCompletionCriteriaDataTypeMismatch",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "A string"
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Test]
		public void CanDeleteAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Delete");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanDeleteAchievement",
				ActorType = ActorType.User,
				Token = "CanDeleteAchievement",
				GameId = game.Id,
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanDeleteAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var getAchievement = _achievementClient.GetById(achievementRequest.Token, achievementRequest.GameId.Value);

			Assert.NotNull(getAchievement);

			_achievementClient.Delete(achievementRequest.Token, achievementRequest.GameId.Value);

			getAchievement = _achievementClient.GetById(achievementRequest.Token, achievementRequest.GameId.Value);

			Assert.Null(getAchievement);
		}

		[Test]
		public void CannotDeleteNonExistingAchievement()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Delete");

			_achievementClient.Delete("CannotDeleteNonExistingAchievement", game.Id);
		}

		[Test]
		public void CannotDeleteAchievementByEmptyToken()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Delete");

			Assert.Throws<ClientException>(() => _achievementClient.Delete("", game.Id));
		}

		[Test]
		public void CanDeleteGlobalAchievement()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanDeleteGlobalAchievement",
				ActorType = ActorType.User,
				Token = "CanDeleteGlobalAchievement",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanDeleteGlobalAchievement",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"

					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var getAchievement = _achievementClient.GetGlobalById(achievementRequest.Token);

			Assert.NotNull(getAchievement);

			_achievementClient.DeleteGlobal(achievementRequest.Token);

			getAchievement = _achievementClient.GetGlobalById(achievementRequest.Token);

			Assert.Null(getAchievement);
		}

		[Test]
		public void CannotDeleteNonExistingGlobalAchievement()
		{
			_achievementClient.DeleteGlobal("CannotDeleteNonExistingGlobalAchievement");
		}

		[Test]
		public void CannotDeleteGlobalAchievementByEmptyToken()
		{
			Assert.Throws<ClientException>(() => _achievementClient.DeleteGlobal(""));
		}

		[Test]
		public void CanGetGlobalAchievementProgress()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "ProgressGet");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetGlobalAchievementProgress",
				ActorType = ActorType.Undefined,
				Token = "CanGetGlobalAchievementProgress",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetGlobalAchievementProgress",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"
					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var progressGame = _achievementClient.GetGlobalProgress(user.Id);
			Assert.AreEqual(1, progressGame.Count());

			var progressAchievement = _achievementClient.GetGlobalAchievementProgress(response.Token, user.Id);
			Assert.AreEqual(0, progressAchievement.Progress);

			var gameData = new GameDataRequest()
			{
				Key = "CanGetGlobalAchievementProgress",
				Value = "1",
				ActorId = user.Id,
				GameDataType = GameDataType.Float
			};

			_gameDataClient.Add(gameData);

			progressAchievement = _achievementClient.GetGlobalAchievementProgress(response.Token, user.Id);
			Assert.AreEqual(1, progressAchievement.Progress);
		}

		[Test]
		public void CannotGetNotExistingGlobalAchievementProgress()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "ProgressGet");

			Assert.Throws<ClientException>(() => _achievementClient.GetGlobalAchievementProgress("CannotGetNotExistingGlobalAchievementProgress", user.Id));
		}

		[Test]
		public void CanGetAchievementProgress()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "ProgressGet");
			var game = Helpers.GetOrCreateGame(_gameClient, "ProgressGet");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetAchievementProgress",
				GameId = game.Id,
				ActorType = ActorType.Undefined,
				Token = "CanGetAchievementProgress",
				CompletionCriterias = new List<CompletionCriteria>()
				{
					new CompletionCriteria()
					{
						Key = "CanGetAchievementProgress",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
						Value = "1"
					}
				},
			};

			var response = _achievementClient.Create(achievementRequest);

			var progressGame = _achievementClient.GetGameProgress(game.Id, user.Id);
			Assert.AreEqual(1, progressGame.Count());

			var progressAchievement = _achievementClient.GetAchievementProgress(response.Token, game.Id, user.Id);
			Assert.AreEqual(0, progressAchievement.Progress);

			var gameData = new GameDataRequest()
			{
				Key = "CanGetAchievementProgress",
				Value = "1",
				ActorId = user.Id,
				GameId = game.Id,
				GameDataType = GameDataType.Float
			};

			_gameDataClient.Add(gameData);

			progressAchievement = _achievementClient.GetAchievementProgress(response.Token, game.Id, user.Id);
			Assert.AreEqual(1, progressAchievement.Progress);
		}

		[Test]
		public void CannotGetNotExistingAchievementProgress()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "ProgressGet");
			var game = Helpers.GetOrCreateGame(_gameClient, "ProgressGet");

			Assert.Throws<ClientException>(() => _achievementClient.GetAchievementProgress("CannotGetNotExistingAchievementProgress", game.Id, user.Id));
		}
		#endregion
	}
}
