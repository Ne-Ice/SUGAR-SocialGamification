﻿using System.Linq;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Contracts.Shared;
using NUnit.Framework;

namespace PlayGen.SUGAR.Client.UnitTests
{
	public class GroupClientTests : ClientTestsBase
	{
		[Test]
		public void CanCreateGroup()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CanCreateGroup",
			};

			var response = SUGARClient.Group.Create(groupRequest);

			Assert.AreEqual(groupRequest.Name, response.Name);
			Assert.True(response.Id > 0);
		}

		[Test]
		public void CannotCreateDuplicateGroup()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CannotCreateDuplicateGroup",
			};

			SUGARClient.Group.Create(groupRequest);

			Assert.Throws<ClientException>(() => SUGARClient.Group.Create(groupRequest));
		}

		[Test]
		public void CannotCreateGroupWithNoName()
		{
			var groupRequest = new ActorRequest { };

			Assert.Throws<ClientException>(() => SUGARClient.Group.Create(groupRequest));
		}

		[Test]
		public void CanGetGroupsByName()
		{
			var groupRequestOne = new ActorRequest
			{
				Name = "CanGetGroupsByName 1",
			};

			var responseOne = SUGARClient.Group.Create(groupRequestOne);

			var groupRequestTwo = new ActorRequest
			{
				Name = "CanGetGroupsByName 2",
			};

			var responseTwo = SUGARClient.Group.Create(groupRequestTwo);

			var getGroups = SUGARClient.Group.Get("CanGetGroupsByName");

			Assert.AreEqual(2, getGroups.Count());
		}

		[Test]
		public void CannotGetNotExistingGroupByName()
		{
			var getGroups = SUGARClient.Group.Get("CannotGetNotExistingGroupByName");

			Assert.IsEmpty(getGroups);
		}

		[Test]
		public void CannotGetGroupByEmptyName()
		{
			Assert.Throws<ClientException>(() => SUGARClient.Group.Get(""));
		}

		[Test]
		public void CanGetGroupById()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CanGetGroupById",
			};

			var response = SUGARClient.Group.Create(groupRequest);

			var getGroup = SUGARClient.Group.Get(response.Id);

			Assert.AreEqual(response.Name, getGroup.Name);
			Assert.AreEqual(groupRequest.Name, getGroup.Name);
		}

		[Test]
		public void CannotGetNotExistingGroupById()
		{
			var getGroup = SUGARClient.Group.Get(-1);

			Assert.Null(getGroup);
		}

		[Test]
		public void CanUpdateGroup()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CanUpdateGroup",
			};

			var response = SUGARClient.Group.Create(groupRequest);

			var updateRequest = new ActorRequest
			{
				Name = "CanUpdateGroup Updated"
			};

			SUGARClient.Group.Update(response.Id, updateRequest);

			var getGroup = SUGARClient.Group.Get(response.Id);

			Assert.AreNotEqual(response.Name, updateRequest.Name);
			Assert.AreEqual("CanUpdateGroup Updated", getGroup.Name);
		}

		[Test]
		public void CannotUpdateGroupToDuplicateName()
		{
			var groupRequestOne = new ActorRequest
			{
				Name = "CannotUpdateGroupToDuplicateName 1"
			};

			var responseOne = SUGARClient.Group.Create(groupRequestOne);

			var groupRequestTwo = new ActorRequest
			{
				Name = "CannotUpdateGroupToDuplicateName 2"
			};

			var responseTwo = SUGARClient.Group.Create(groupRequestTwo);

			var updateGroup = new ActorRequest
			{
				Name = groupRequestOne.Name
			};

			Assert.Throws<ClientException>(() => SUGARClient.Group.Update(responseTwo.Id, updateGroup));
		}

		[Test]
		public void CannotUpdateNonExistingGroup()
		{
			var updateGroup = new ActorRequest
			{
				Name = "CannotUpdateNonExistingGroup"
			};

			Assert.Throws<ClientException>(() => SUGARClient.Group.Update(-1, updateGroup));
		}

		[Test]
		public void CannotUpdateGroupToNoName()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CannotUpdateGroupToNoName",
			};

			var response = SUGARClient.Group.Create(groupRequest);

			var updateRequest = new ActorRequest
			{
			};

			Assert.Throws<ClientException>(() => SUGARClient.Group.Update(response.Id, updateRequest));
		}

		[Test]
		public void CanDeleteGroup()
		{
			var groupRequest = new ActorRequest
			{
				Name = "CanDeleteGroup",
			};

			var response = SUGARClient.Group.Create(groupRequest);

			var getGroup = SUGARClient.Group.Get(response.Id);

			Assert.NotNull(getGroup);

			SUGARClient.Group.Delete(response.Id);

			getGroup = SUGARClient.Group.Get(response.Id);

			Assert.Null(getGroup);
		}

		[Test]
		public void CannotDeleteNonExistingGroup()
		{
			SUGARClient.Group.Delete(-1);
		}
	}
}