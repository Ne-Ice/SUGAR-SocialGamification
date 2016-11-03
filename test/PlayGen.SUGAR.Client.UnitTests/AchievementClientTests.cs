﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using Xunit;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Client.IntegrationTests
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
		[Fact]
		public void CanCreateAchievement()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanCreateAchievement",
				ActorType = ActorType.User,
				Token = "CanCreateAchievement",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.Equal(achievementRequest.Token, response.Token);
			Assert.Equal(achievementRequest.ActorType, response.ActorType);
		}

		public void CanCreateGlobalAchievement()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanCreateGlobalAchievement",
				ActorType = ActorType.User,
				Token = "CanCreateGlobalAchievement",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.Equal(achievementRequest.Token, response.Token);
			Assert.Equal(achievementRequest.ActorType, response.ActorType);
		}

		[Fact]
		public void CannotCreateDuplicateAchievement()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateDuplicateAchievement",
				ActorType = ActorType.User,
				Token = "CannotCreateDuplicateAchievement",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotCreateAchievementWithNoName()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoName",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotCreateAchievementWithNoToken()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoToken",
				ActorType = ActorType.User,
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotCreateAchievementWithNoCompletionCriteria()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoCompletionCriteria",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoCompletionCriteria",
				GameId = game.Id,
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Fact]
		public void CannotCreateAchievementWithNoAchievementCriteriaKey()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoAchievementCriteriaKey",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoAchievementCriteriaKey",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotCreateAchievementWithNoAchievementCriteriaValue()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoAchievementCriteriaValue",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoAchievementCriteriaValue",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotCreateAchievementWithNoAchievementCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Create(achievementRequest));
		}

		[Fact]
		public void CannotCreateAchievementWithNoAchievementCriteriaDataTypeMismatch()
		{
			var game = GetOrCreateGame("Create");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotCreateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				ActorType = ActorType.User,
				Token = "CannotCreateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotCreateAchievementWithNoAchievementCriteriaValue",
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

		[Fact]
		public void CanGetAchievementsByGame()
		{
			var game = GetOrCreateGame("GameGet");

			var achievementRequestOne = new AchievementRequest()
			{
				Name = "CanGetAchievementsByGameOne",
				ActorType = ActorType.User,
				Token = "CanGetAchievementsByGameOne",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.Equal(2, getAchievement.Count());
		}

		[Fact]
		public void CanGetAchievementByKeys()
		{
			var game = GetOrCreateGame("Get");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetAchievementByKeys",
				ActorType = ActorType.User,
				Token = "CanGetAchievementByKeys",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.Equal(response.Name, getAchievement.Name);
			Assert.Equal(achievementRequest.Name, getAchievement.Name);
		}

		[Fact]
		public void CannotGetNotExistingAchievementByKeys()
		{
			var game = GetOrCreateGame("Get");

			var getAchievement = _achievementClient.GetById("CannotGetNotExistingAchievementByKeys", game.Id);

			Assert.Null(getAchievement);
		}

		[Fact]
		public void CannotGetAchievementByEmptyToken()
		{
			var game = GetOrCreateGame("Get");

			Assert.Throws<ClientException>(() => _achievementClient.GetById("", game.Id));
		}

		[Fact]
		public void CanGetAchievementByKeysThatContainSlashes()
		{
            // todo this test seems incorrect
			var game = GetOrCreateGame("Get");

			var getAchievement = _achievementClient.GetById("Can/Get/Achievement/By/Keys/That/Contain/Slashes", game.Id);

			Assert.Null(getAchievement);
		}

		[Fact]
		public void CanGetGlobalAchievementByToken()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetGlobalAchievementByToken",
				ActorType = ActorType.User,
				Token = "CanGetGlobalAchievementByToken",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.Equal(response.Name, getAchievement.Name);
			Assert.Equal(achievementRequest.Name, getAchievement.Name);
		}

		[Fact]
		public void CannotGetNotExistingGlobalAchievementByKeys()
		{
			var getAchievement = _achievementClient.GetGlobalById("CannotGetNotExistingGlobalAchievementByKeys");

			Assert.Null(getAchievement);
		}

		[Fact]
		public void CannotGetGlobalAchievementByEmptyToken()
		{
			Assert.Throws<ClientException>(() => _achievementClient.GetGlobalById(""));
		}

		[Fact]
		public void CanGetGlobalAchievementByKeysThatContainSlashes()
		{
            // todo this test seems incorrect
			var getAchievement = _achievementClient.GetGlobalById("Can/Get/Achievement/By/Keys/That/Contain/Slashes");

            // todo should this not be an exception?
			Assert.Null(getAchievement);
		}

		[Fact]
		public void CannotGetByAchievementsByNotExistingGameId()
		{
			var getAchievements = _achievementClient.GetByGame(-1);

			Assert.Empty(getAchievements);
		}

		[Fact]
		public void CanUpdateAchievement()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanUpdateAchievement",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CanUpdateAchievement",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

			Assert.NotEqual(response.Name, updateResponse.Name);
			Assert.Equal("CanUpdateAchievement Updated", updateResponse.Name);
		}

		[Fact]
		public void CannotUpdateAchievementToDuplicateName()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequestOne = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementToDuplicateNameOne",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementToDuplicateNameOne",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateNonExistingAchievement()
		{
			var game = GetOrCreateGame("Update");

			var updateRequest = new AchievementRequest()
			{
				Name = "CannotUpdateNonExistingAchievement",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateNonExistingAchievement",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateAchievemenWithNoName()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievemenWithNoName",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievemenWithNoName",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateAchievementWithNoToken()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoToken",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoToken",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateAchievementWithNoCompletionCriteria()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoCompletionCriteria",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoCompletionCriteria",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateAchievementWithNoAchievementCriteriaKey()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaKey",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaKey",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotUpdateAchievementWithNoAchievementCriteriaKey",
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
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaKey",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaKey",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotUpdateAchievementWithNoAchievementCriteriaValue()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
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
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotUpdateAchievementWithNoAchievementCriteriaValue",
						ComparisonType = ComparisonType.Equals,
						CriteriaQueryType = CriteriaQueryType.Any,
						DataType = GameDataType.Float,
						Scope = CriteriaScope.Actor,
					}
				},
			};

			Assert.Throws<ClientException>(() => _achievementClient.Update(updateRequest));
		}

		[Fact]
		public void CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch()
		{
			var game = GetOrCreateGame("Update");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				GameId = game.Id,
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
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
				Name = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				ActorType = ActorType.User,
				Token = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
					{
						Key = "CannotUpdateAchievementWithNoAchievementCriteriaDataTypeMismatch",
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

		[Fact]
		public void CanDeleteAchievement()
		{
			var game = GetOrCreateGame("Delete");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanDeleteAchievement",
				ActorType = ActorType.User,
				Token = "CanDeleteAchievement",
				GameId = game.Id,
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotDeleteNonExistingAchievement()
		{
			var game = GetOrCreateGame("Delete");

			_achievementClient.Delete("CannotDeleteNonExistingAchievement", game.Id);
		}

		[Fact]
		public void CannotDeleteAchievementByEmptyToken()
		{
			var game = GetOrCreateGame("Delete");

			Assert.Throws<ClientException>(() => _achievementClient.Delete("", game.Id));
		}

		[Fact]
		public void CanDeleteGlobalAchievement()
		{
			var achievementRequest = new AchievementRequest()
			{
				Name = "CanDeleteGlobalAchievement",
				ActorType = ActorType.User,
				Token = "CanDeleteGlobalAchievement",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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

		[Fact]
		public void CannotDeleteNonExistingGlobalAchievement()
		{
			_achievementClient.DeleteGlobal("CannotDeleteNonExistingGlobalAchievement");
		}

		[Fact]
		public void CannotDeleteGlobalAchievementByEmptyToken()
		{
			Assert.Throws<ClientException>(() => _achievementClient.DeleteGlobal(""));
		}

		[Fact]
		public void CanGetGlobalAchievementProgress()
		{
			var user = GetOrCreateUser("ProgressGet");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetGlobalAchievementProgress",
				ActorType = ActorType.Undefined,
				Token = "CanGetGlobalAchievementProgress",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
			Assert.Equal(1, progressGame.Count());

			var progressAchievement = _achievementClient.GetGlobalAchievementProgress(response.Token, user.Id);
			Assert.Equal(0, progressAchievement.Progress);

			var gameData = new GameDataRequest()
			{
				Key = "CanGetGlobalAchievementProgress",
				Value = "1",
				ActorId = user.Id,
				GameDataType = GameDataType.Float
			};

			_gameDataClient.Add(gameData);

			progressAchievement = _achievementClient.GetGlobalAchievementProgress(response.Token, user.Id);
			Assert.Equal(1, progressAchievement.Progress);
		}

		[Fact]
		public void CannotGetNotExistingGlobalAchievementProgress()
		{
			var user = GetOrCreateUser("ProgressGet");

			Assert.Throws<ClientException>(() => _achievementClient.GetGlobalAchievementProgress("CannotGetNotExistingGlobalAchievementProgress", user.Id));
		}

		[Fact]
		public void CanGetAchievementProgress()
		{
			var user = GetOrCreateUser("ProgressGet");
			var game = GetOrCreateGame("ProgressGet");

			var achievementRequest = new AchievementRequest()
			{
				Name = "CanGetAchievementProgress",
				GameId = game.Id,
				ActorType = ActorType.Undefined,
				Token = "CanGetAchievementProgress",
				CompletionCriteria = new List<AchievementCriteria>()
				{
					new AchievementCriteria()
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
			Assert.Equal(1, progressGame.Count());

			var progressAchievement = _achievementClient.GetAchievementProgress(response.Token, game.Id, user.Id);
			Assert.Equal(0, progressAchievement.Progress);

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
			Assert.Equal(1, progressAchievement.Progress);
		}

		[Fact]
		public void CannotGetNotExistingAchievementProgress()
		{
			var user = GetOrCreateUser("ProgressGet");
			var game = GetOrCreateGame("ProgressGet");

			Assert.Throws<ClientException>(() => _achievementClient.GetAchievementProgress("CannotGetNotExistingAchievementProgress", game.Id, user.Id));
		}
		#endregion
		#region Helpers
		private ActorResponse GetOrCreateUser(string suffix)
		{
			string name = "AchievementControllerTests" + suffix ?? $"_{suffix}";
			var users = _userClient.Get(name, true);
			ActorResponse user;

			if (users.Any())
			{
				user = users.Single();
			}
			else
			{
				user = _userClient.Create(new ActorRequest
				{
					Name = name
				});
			}

			return user;
		}

		private GameResponse GetOrCreateGame(string suffix)
		{
			string name = "AchievementControllerTests" + suffix ?? $"_{suffix}";
			var games = _gameClient.Get(name);
			GameResponse game;

			if (games.Any())
			{
				game = games.Single();
			}
			else
			{
				game = _gameClient.Create(new GameRequest
				{
					Name = name
				});
			}

			return game;
		}
		#endregion
	}
}
