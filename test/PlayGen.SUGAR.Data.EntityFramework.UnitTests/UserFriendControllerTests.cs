﻿using System;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.Model;
using NUnit.Framework;
using System.Linq;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	public class UserFriendControllerTests
	{
		#region Configuration
		private readonly UserRelationshipController _userRelationshipDbController;
		private readonly UserController _userDbController;

		public UserFriendControllerTests()
		{
			_userRelationshipDbController = TestEnvironment.UserRelationshipController;
			_userDbController = TestEnvironment.UserController;
		}
		#endregion

		#region Tests
		[Test]
		public void CreateAndGetUserFriendRequest()
		{
			string userFriendName = "CreateUserFriendRequest";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			var userRequests = _userRelationshipDbController.GetRequests(newFriend.AcceptorId);

			int matches = userRequests.Count(g => g.Name == userFriendName + " Requestor");

			Assert.AreEqual(matches, 1);
		}

		[Test]
		public void CreateUserFriendWithNonExistingRequestor()
		{
			string userFriendName = "CreateUserFriendWithNonExistingRequestor";
			var acceptor = CreateUser(userFriendName);
			Assert.Throws<MissingRecordException>(() => CreateUserFriend(-1, acceptor.Id));
		}

		[Test]
		public void CreateUserFriendWithNonExistingAcceptor()
		{
			string userFriendName = "CreateUserFriendWithNonExistingAcceptor";
			var requestor = CreateUser(userFriendName);
			Assert.Throws<MissingRecordException>(() => CreateUserFriend(requestor.Id, -1));
		}

		[Test]
		public void CreateDuplicateUserFriend()
		{
			string userFriendName = "CreateDuplicateUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			CreateUserFriend(requestor.Id, acceptor.Id);

			Assert.Throws<DuplicateRecordException>(() => CreateUserFriend(requestor.Id, acceptor.Id));
		}

		[Test]
		public void CreateDuplicateReversedUserFriend()
		{
			string userFriendName = "CreateDuplicateReversedUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			CreateUserFriend(requestor.Id, acceptor.Id);

			Assert.Throws<DuplicateRecordException>(() => CreateUserFriend(acceptor.Id, requestor.Id));
		}

		[Test]
		public void GetNonExistingUserFriendRequests()
		{
			var userFriends = _userRelationshipDbController.GetRequests(-1);

			Assert.IsEmpty(userFriends);
		}

		[Test]
		public void GetUserSentFriendRequests()
		{
			string userFriendName = "GetUserSentFriendRequests";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			var userRequests = _userRelationshipDbController.GetSentRequests(newFriend.RequestorId);

			int matches = userRequests.Count(g => g.Name == userFriendName + " Acceptor");

			Assert.AreEqual(matches, 1);
		}

		[Test]
		public void GetNonExistingUserSentFriendRequests()
		{
			var userFriends = _userRelationshipDbController.GetSentRequests(-1);

			Assert.IsEmpty(userFriends);
		}

		[Test]
		public void AcceptUserFriendRequest()
		{
			string userFriendName = "AcceptUserFriendRequest";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			_userRelationshipDbController.UpdateRequest(newFriend, true);

			var userRequests = _userRelationshipDbController.GetRequests(newFriend.AcceptorId);

			var matches = userRequests.Count(g => g.Name == userFriendName + " Requestor");

			Assert.AreEqual(matches, 0);

			var userFriends = _userRelationshipDbController.GetFriends(newFriend.RequestorId);

			matches = userFriends.Count(g => g.Name == userFriendName + " Acceptor");

			Assert.AreEqual(matches, 1);
		}

		[Test]
		public void RejectUserFriendRequest()
		{
			string userFriendName = "RejectUserFriendRequest";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			_userRelationshipDbController.UpdateRequest(newFriend, false);

			var userRequests = _userRelationshipDbController.GetRequests(newFriend.AcceptorId);

			int matches = userRequests.Count(g => g.Name == userFriendName + " Requestor");

			Assert.AreEqual(matches, 0);

			var userFriends = _userRelationshipDbController.GetFriends(newFriend.RequestorId);

			matches = userFriends.Count(g => g.Name == userFriendName + " Acceptor");

			Assert.AreEqual(matches, 0);
		}

		[Test]
		public void UpdateNonExistingUserFriendRequest()
		{
			string userFriendName = "UpdateNonExistingUserFriendRequest";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = new UserToUserRelationship
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			Assert.Throws<InvalidOperationException>(() => _userRelationshipDbController.UpdateRequest(newFriend, true));
		}

		[Test]
		public void GetNonExistingUserFriends()
		{
			var userFriends = _userRelationshipDbController.GetFriends(-1);

			Assert.IsEmpty(userFriends);
		}

		[Test]
		public void CreateDuplicateAcceptedUserFriend()
		{
			string userFriendName = "CreateDuplicateAcceptedUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			_userRelationshipDbController.UpdateRequest(newFriend, true);

			Assert.Throws<DuplicateRecordException>(() => CreateUserFriend(requestor.Id, acceptor.Id));
		}

		[Test]
		public void CreateDuplicateReversedAcceptedUserFriend()
		{
			string userFriendName = "CreateDuplicateReversedAcceptedUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			_userRelationshipDbController.UpdateRequest(newFriend, true);

			Assert.Throws<DuplicateRecordException>(() => CreateUserFriend(acceptor.Id, requestor.Id));
		}

		[Test]
		public void UpdateUserFriend()
		{
			string userFriendName = "UpdateUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = CreateUserFriend(requestor.Id, acceptor.Id);

			_userRelationshipDbController.UpdateRequest(newFriend, true);

			_userRelationshipDbController.Update(newFriend);
			var friends = _userRelationshipDbController.GetFriends(acceptor.Id);

			Assert.IsEmpty(friends);
		}

		[Test]
		public void UpdateNonExistingUserFriend()
		{
			string userFriendName = "UpdateNonExistingUserFriend";

			var requestor = CreateUser(userFriendName + " Requestor");
			var acceptor = CreateUser(userFriendName + " Acceptor");

			var newFriend = new UserToUserRelationship
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			Assert.Throws<InvalidOperationException>(() => _userRelationshipDbController.Update(newFriend));
		}
		#endregion

		#region Helpers
		private User CreateUser(string name)
		{
			var user = new User
			{
				Name = name,
			};

			_userDbController.Create(user);

			return user;
		}

		private UserToUserRelationship CreateUserFriend(int requestor, int acceptor)
		{
			var userFriend = new UserToUserRelationship
			{
				RequestorId = requestor,
				AcceptorId = acceptor
			};
			_userRelationshipDbController.Create(userFriend, false);

			return userFriend;
		}
		#endregion
	}
}
