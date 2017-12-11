﻿using System;
using System.Linq;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Contracts;
using Xunit;

namespace PlayGen.SUGAR.Client.Tests
{
	public class GameDataClientTests : ClientTestBase
	{
		[Fact]
		public void CanCreate()
		{
			var key = "GameData_CanCreate";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			var response = Fixture.SUGARClient.GameData.Add(evaluationDataRequest);

			Assert.Equal(evaluationDataRequest.CreatingActorId, response.CreatingActorId);
			Assert.Equal(evaluationDataRequest.GameId, response.GameId);
			Assert.Equal(evaluationDataRequest.Key, response.Key);
			Assert.Equal(evaluationDataRequest.Value, response.Value);
			Assert.Equal(evaluationDataRequest.EvaluationDataType, response.EvaluationDataType);
		}

		[Fact]
		public void CannotCreateWithoutGameId()
		{
			var key = "GameData_CanCreateWithoutGameId";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CannotCreateWithoutActorId()
		{
			var key = "GameData_CannotCreateWithoutActorId";
			Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				GameId = game.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CannotCreateWithoutKey()
		{
			var key = "GameData_CannotCreateWithoutKey";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			Assert.Throws<ArgumentException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CannotCreateWithoutValue()
		{
			var key = "GameData_CannotCreateWithoutValue";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				EvaluationDataType = EvaluationDataType.String
			};

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CannotCreateWithoutEvaluationDataType()
		{
			var key = "GameData_CannotCreateWithoutEvaluationDataType";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				Value = "Test Value"
			};

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CannotCreateWithMismatchedData()
		{
			var key = "GameData_CannotCreateWithMismatchedData";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.Float
			};

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));

			evaluationDataRequest.EvaluationDataType = EvaluationDataType.Long;

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));

			evaluationDataRequest.EvaluationDataType = EvaluationDataType.Boolean;

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));

			evaluationDataRequest.EvaluationDataType = EvaluationDataType.Float;
			evaluationDataRequest.Value = "True";

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));

			evaluationDataRequest.EvaluationDataType = EvaluationDataType.Long;

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));

			evaluationDataRequest.EvaluationDataType = EvaluationDataType.Boolean;
			evaluationDataRequest.Value = "2";

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		[Fact]
		public void CanGetGameData()
		{
			var key = "GameData_CanGetGameData";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			var response = Fixture.SUGARClient.GameData.Add(evaluationDataRequest);

			var get = Fixture.SUGARClient.GameData.Get(loggedInAccount.User.Id, game.Id, new [] { key });

			Assert.Equal(1, get.Count());
			Assert.Equal(get.First().CreatingActorId, response.CreatingActorId);
			Assert.Equal(get.First().GameId, response.GameId);
			Assert.Equal(get.First().Key, response.Key);
			Assert.Equal(get.First().Value, response.Value);
			Assert.Equal(get.First().EvaluationDataType, response.EvaluationDataType);
		}

		[Fact]
		public void CannotGetGameDataWithoutActorId()
		{
			var key = "GameData_CannotGetGameDataWithoutActorId";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			Fixture.SUGARClient.GameData.Add(evaluationDataRequest);

			Assert.Throws<ClientHttpException>(() => Fixture.SUGARClient.GameData.Get(Platform.GlobalId, game.Id, new[] { key }));
		}

		[Fact]
		public void CanGetGlobalGameData()
		{
			var key = "GameData_CanGetGameDataWithoutGameId";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = Platform.GlobalId,
				Key = key,
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			var response = Fixture.SUGARClient.GameData.Add(evaluationDataRequest);

			var get = Fixture.SUGARClient.GameData.Get(loggedInAccount.User.Id, Platform.GlobalId, new[] { key });

			Assert.Equal(1, get.Count());
			Assert.Equal(get.First().CreatingActorId, response.CreatingActorId);
			Assert.Equal(get.First().GameId, response.GameId);
			Assert.Equal(get.First().Key, response.Key);
			Assert.Equal(get.First().Value, response.Value);
			Assert.Equal(get.First().EvaluationDataType, response.EvaluationDataType);
		}

		[Fact]
		public void CanGetGameDataByMultipleKeys()
		{
			var key = "GameData_CanGetGameDataByMultipleKeys";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);
			var game = Helpers.GetGame(Fixture.SUGARClient.Game, key);

			var evaluationDataRequestOne = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key + "1",
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			var evaluationDataRequestTwo = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key + "2",
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			var evaluationDataRequestThree = new EvaluationDataRequest
			{
				CreatingActorId = loggedInAccount.User.Id,
				GameId = game.Id,
				Key = key + "3",
				Value = "Test Value",
				EvaluationDataType = EvaluationDataType.String
			};

			Fixture.SUGARClient.GameData.Add(evaluationDataRequestOne);
			Fixture.SUGARClient.GameData.Add(evaluationDataRequestTwo);
			Fixture.SUGARClient.GameData.Add(evaluationDataRequestThree);

			var get = Fixture.SUGARClient.GameData.Get(loggedInAccount.User.Id, game.Id, new[] { evaluationDataRequestOne.Key, evaluationDataRequestTwo.Key, evaluationDataRequestThree.Key });

			Assert.Equal(3, get.Count());
			foreach (var g in get)
			{
				Assert.Equal("Test Value", g.Value);
			}
		}

		[Theory]
		[InlineData("a")]
		[InlineData("My_key0")]
		[InlineData("1mykey")]
		[InlineData("_key")]
		[InlineData("_")]
		[InlineData("9291")]
		public void CanCreateWithValidKey(string dataKey)
		{
			var key = "GameData_CanCreateWithValidKey";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);

			// Act
			var evaluationDataRequest = new EvaluationDataRequest
			{
				Key = dataKey,
				GameId = Platform.GlobalId,
				Value = "TestValue",
				EvaluationDataType = EvaluationDataType.String,
				CreatingActorId = loggedInAccount.User.Id
			};

			var response = Fixture.SUGARClient.GameData.Add(evaluationDataRequest);

			// Assert
			Assert.NotNull(response);
		}

		[Theory]
		[InlineData("")]
		[InlineData("$")]
		[InlineData("dj_+das")]
		public void CantCreateWithInValidKey(string dataKey)
		{
			var key = "GameData_CantCreateWithInValidKey";
			var loggedInAccount = Helpers.CreateAndLoginGlobal(Fixture.SUGARClient, key);

			var evaluationDataRequest = new EvaluationDataRequest
			{
				Key = dataKey,
				Value = "TestValue",
				EvaluationDataType = EvaluationDataType.String,
				CreatingActorId = loggedInAccount.User.Id
			};

			// Act Assert
			Assert.Throws<ArgumentException>(() => Fixture.SUGARClient.GameData.Add(evaluationDataRequest));
		}

		public GameDataClientTests(ClientTestsFixture fixture)
			: base(fixture)
		{
		}
	}
}