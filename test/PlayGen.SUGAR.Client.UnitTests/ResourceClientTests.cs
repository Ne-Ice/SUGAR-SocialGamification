﻿using System;
using System.Linq;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Contracts.Shared;
using NUnit.Framework;

namespace PlayGen.SUGAR.Client.UnitTests
{
	public class ResourceClientTests
	{
		#region Configuration
		private readonly ResourceClient _resourceClient;
		private readonly UserClient _userClient;
		private readonly GameClient _gameClient;

		public ResourceClientTests()
		{
			var testSugarClient = new TestSUGARClient();
			_resourceClient = testSugarClient.Resource;
			_userClient = testSugarClient.User;
			_gameClient = testSugarClient.Game;

			Helpers.CreateAndLogin(testSugarClient.Session);
		}
		#endregion

		#region Tests
		[Test]
		public void CanCreate()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "Create");
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var resourceRequest = new ResourceAddRequest
			{
				ActorId = user.Id,
				GameId = game.Id,
				Key = "CanCreate",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			Assert.AreEqual(resourceRequest.Key, response.Key);
			Assert.AreEqual(resourceRequest.Quantity, response.Quantity);
		}

		[Test]
		public void CanCreateWithoutGameId()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "Create");

			var resourceRequest = new ResourceAddRequest
			{
				ActorId = user.Id,
				Key = "CanCreateWithoutGameId",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			Assert.AreEqual(resourceRequest.Key, response.Key);
			Assert.AreEqual(resourceRequest.Quantity, response.Quantity);
		}

		[Test]
		public void CanCreateWithoutActorId()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Create");

			var resourceRequest = new ResourceAddRequest
			{
				GameId = game.Id,
				Key = "CanCreateWithoutActorId",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			Assert.AreEqual(resourceRequest.Key, response.Key);
			Assert.AreEqual(resourceRequest.Quantity, response.Quantity);
		}

		[Test]
		public void CanUpdateExisting()
		{
			var resourceRequest = new ResourceAddRequest
			{
				Key = "CanUpdateExisting",
				Quantity = 100,
			};

			var createdResource = _resourceClient.AddOrUpdate(resourceRequest);
			var createdQuantity = createdResource.Quantity;
			var updatedQuantity = createdQuantity + 9000;

			//resourceRequest.Quantity = updatedQuantity;

			var resourceRequestUpdated = new ResourceAddRequest
			{
				Key = createdResource.Key,
				Quantity = updatedQuantity
			};

			_resourceClient.AddOrUpdate(resourceRequestUpdated);

			var updatedResource = _resourceClient.Get(createdResource.GameId, createdResource.ActorId,
				new[] {createdResource.Key}).Single();

			Assert.AreEqual(createdQuantity + updatedQuantity, updatedResource.Quantity);
			Assert.AreEqual(createdResource.Id, updatedResource.Id);
		}

		[Test]
		public void CanTransferCreateResource_FromUserToUser()
		{
			var fromUser = Helpers.GetOrCreateUser(_userClient, "From");
			var toUser = Helpers.GetOrCreateUser(_userClient, "To");

			var fromResource = _resourceClient.AddOrUpdate(new ResourceAddRequest
			{
				GameId = null,
				ActorId = fromUser.Id,
				Key = "CanTransferCreateResource_FromUserToUser",
				Quantity = 100,
			});

			var originalQuantity = fromResource.Quantity;
			var transferQuantity = originalQuantity/3;

			var transferResponse = _resourceClient.Transfer(new ResourceTransferRequest
			{
				GameId = fromResource.GameId,
				SenderActorId = fromUser.Id,
				RecipientActorId = toUser.Id,
				Key = fromResource.Key,
				Quantity = transferQuantity
			});

			Assert.AreEqual(originalQuantity - transferQuantity, transferResponse.FromResource.Quantity);
			Assert.AreEqual(transferQuantity, transferResponse.ToResource.Quantity);
			Assert.AreEqual(toUser.Id, transferResponse.ToResource.ActorId);
			Assert.AreEqual(fromResource.GameId, transferResponse.FromResource.GameId);
			Assert.AreEqual(fromResource.GameId, transferResponse.ToResource.GameId);
		}

		[Test]
		public void CanTransferUpdateResource_FromUserToUser()
		{
			var fromUser = Helpers.GetOrCreateUser(_userClient, "From");
			var toUser = Helpers.GetOrCreateUser(_userClient, "To");

			var fromResource = _resourceClient.AddOrUpdate(new ResourceAddRequest
			{
				GameId = null,
				ActorId = fromUser.Id,
				Key = "CanTransferUpdateResource_FromUserToUser",
				Quantity = 100,
			});

			var toResource = _resourceClient.AddOrUpdate(new ResourceAddRequest
			{
				GameId = fromResource.GameId,
				ActorId = toUser.Id,
				Key = fromResource.Key,
				Quantity = 50,
			});

			var originalFrmoQuantity = fromResource.Quantity;
			var originalToQuantity = toResource.Quantity;
			var transferQuantity = originalFrmoQuantity / 3;

			var transferResponse = _resourceClient.Transfer(new ResourceTransferRequest
			{
				GameId = fromResource.GameId,
				SenderActorId = fromUser.Id,
				RecipientActorId = toUser.Id,
				Key = fromResource.Key,
				Quantity = transferQuantity
			});

			Assert.AreEqual(originalFrmoQuantity - transferQuantity, transferResponse.FromResource.Quantity);
			Assert.AreEqual(originalToQuantity + transferQuantity, transferResponse.ToResource.Quantity);
			Assert.AreEqual(toUser.Id, transferResponse.ToResource.ActorId);
			Assert.AreEqual(fromResource.GameId, transferResponse.FromResource.GameId);
			Assert.AreEqual(fromResource.GameId, transferResponse.ToResource.GameId);
		}

		[Test]
		public void CannotTransferNonExistingResource()
		{
			var fromUser = Helpers.GetOrCreateUser(_userClient, "From");
			var toUser = Helpers.GetOrCreateUser(_userClient, "To");

			Assert.Throws<ClientException>(() => _resourceClient.Transfer(new ResourceTransferRequest
			{

				SenderActorId = fromUser.Id,
				RecipientActorId = toUser.Id,
				Key = new Guid().ToString(),
				GameId = null,
				Quantity = 100,
			}));
		}

		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(-2000)]
		public void CannotTransfer_WithLessThan1Quantity(long transferQuantity)
		{
			var fromUser = Helpers.GetOrCreateUser(_userClient, "From");
			var toUser = Helpers.GetOrCreateUser(_userClient, "To");

			var fromResource = _resourceClient.AddOrUpdate(new ResourceAddRequest
			{
				GameId = null,
				ActorId = null,
				Key = "CannotTransfer_WithLessThan1Quantity" + transferQuantity,
				Quantity = 100,
			});		

			Assert.Throws<ClientException>(() => _resourceClient.Transfer(new ResourceTransferRequest
			{
				SenderActorId = fromUser.Id,
				RecipientActorId = toUser.Id,
				Key = fromResource.Key,
				GameId = fromResource.GameId,
				Quantity = transferQuantity
			}));
		}

		[Test]
		public void CannotTransfer_WithOutOfRangeQuantity()
		{
			var fromUser = Helpers.GetOrCreateUser(_userClient, "From");
			var toUser = Helpers.GetOrCreateUser(_userClient, "To");

			var fromResource = _resourceClient.AddOrUpdate(new ResourceAddRequest
			{
				GameId = null,
				ActorId = null,
				Key = "CannotTransfer_WithOutOfRangeQuantity",
				Quantity = 100,
			});

			var transferQuantity = fromResource.Quantity*2;

			Assert.Throws<ClientException>(() => _resourceClient.Transfer(new ResourceTransferRequest
			{
				GameId = fromResource.GameId,
				SenderActorId = fromUser.Id,
				RecipientActorId = toUser.Id,
				Key = fromResource.Key,
				Quantity = transferQuantity,
			}));
		}

		[Test]
		public void CanGetResource()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "Get");
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var resourceRequest = new ResourceAddRequest
			{
				ActorId = user.Id,
				GameId = game.Id,
				Key = "CanGetResource",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			var get = _resourceClient.Get(game.Id, user.Id, new string[] { "CanGetResource" });

			Assert.AreEqual(1, get.Count());
			Assert.AreEqual(resourceRequest.Key, get.First().Key);
			Assert.AreEqual(resourceRequest.Quantity, get.First().Quantity);
		}

		[Test]
		public void CanGetResourceWithoutActorId()
		{
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var resourceRequest = new ResourceAddRequest
			{
				GameId = game.Id,
				Key = "CanGetResourceWithoutActorId",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			var get = _resourceClient.Get(game.Id, null, new string[] { "CanGetResourceWithoutActorId" });

			Assert.AreEqual(1, get.Count());
			Assert.AreEqual(resourceRequest.Key, get.First().Key);
			Assert.AreEqual(resourceRequest.Quantity, get.First().Quantity);
		}

		[Test]
		public void CanGetResourceWithoutGameId()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "Get");

			var resourceRequest = new ResourceAddRequest
			{
				ActorId = user.Id,
				Key = "CanGetResourceWithoutGameId",
				Quantity = 100,
			};

			var response = _resourceClient.AddOrUpdate(resourceRequest);

			var get = _resourceClient.Get(null, user.Id, new string[] { "CanGetResourceWithoutGameId" });

			Assert.AreEqual(1, get.Count());
			Assert.AreEqual(resourceRequest.Key, get.First().Key);
			Assert.AreEqual(resourceRequest.Quantity, get.First().Quantity);
		}

		[Test]
		public void CanGetResourceByMultipleKeys()
		{
			var user = Helpers.GetOrCreateUser(_userClient, "Get");
			var game = Helpers.GetOrCreateGame(_gameClient, "Get");

			var resourceRequestOne = new ResourceAddRequest
			{
				ActorId = user.Id,
				GameId = game.Id,
				Key = "CanGetResourceByMultipleKeys1",
				Quantity = 100,
			};

			var resourceRequestTwo = new ResourceAddRequest
			{
				ActorId = user.Id,
				GameId = game.Id,
				Key = "CanGetResourceByMultipleKeys2",
				Quantity = 100,
			};

			var resourceRequestThree = new ResourceAddRequest
			{
				ActorId = user.Id,
				GameId = game.Id,
				Key = "CanGetResourceByMultipleKeys3",
				Quantity = 100,
			};

			var responseOne = _resourceClient.AddOrUpdate(resourceRequestOne);
			var responseTwo = _resourceClient.AddOrUpdate(resourceRequestTwo);
			var responseThree = _resourceClient.AddOrUpdate(resourceRequestThree);

			var get = _resourceClient.Get(game.Id, user.Id, new string[] { "CanGetResourceByMultipleKeys1", "CanGetResourceByMultipleKeys2", "CanGetResourceByMultipleKeys3" });

			Assert.AreEqual(3, get.Count());
			foreach (var r in get)
			{
				Assert.AreEqual(100, r.Quantity);
			}
		}
		#endregion
	}
}