﻿using System.Diagnostics;
using System.Linq;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Contracts.Shared;
using NUnit.Framework;

namespace PlayGen.SUGAR.Client.UnitTests
{
	public class GroupMemberClientTests
	{
		#region Configuration

		private readonly GroupMemberClient _groupMemberClient;
		private readonly GroupClient _groupClient;
		private readonly UserClient _userClient;

		public GroupMemberClientTests()
		{
			var testSugarClient = new TestSUGARClient();
			_groupMemberClient = testSugarClient.GroupMember;
			_groupClient = testSugarClient.Group;
			_userClient = testSugarClient.User;

			Helpers.RegisterAndLogin(testSugarClient.Account);
		}
		#endregion

		#region Tests

		[Test]
		public void CanCreateRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanCreateRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanCreateRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			Assert.AreEqual(relationshipRequest.RequestorId, relationshipResponse.RequestorId);
			Assert.AreEqual(relationshipRequest.AcceptorId, relationshipResponse.AcceptorId);

			var sent = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			var received = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(1, received.Count());
		}

		[Test]
		public void CanCreateAutoAcceptedRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanCreateAutoAcceptedRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanCreateAutoAcceptedRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				AutoAccept = true,
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			Assert.AreEqual(relationshipRequest.RequestorId, relationshipResponse.RequestorId);
			Assert.AreEqual(relationshipRequest.AcceptorId, relationshipResponse.AcceptorId);

			var sent = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			var received = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(1, received.Count());
		}

		[Test]
		public void CannotCreateDuplicateRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CannotCreateDuplicateRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CannotCreateDuplicateRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			Assert.Throws<ClientException>(() => _groupMemberClient.CreateMemberRequest(relationshipRequest));
		}

		[Test]
		public void CannotCreateDuplicateRequestOfAccepted()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "DuplicateRequestOfAccepted");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "DuplicateRequestOfAccepted");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				AutoAccept = true
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			relationshipRequest.AutoAccept = false;

			Assert.Throws<ClientException>(() => _groupMemberClient.CreateMemberRequest(relationshipRequest));
		}

		[Test]
		public void CannotCreateDuplicateAutoAcceptedRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "DuplicateAutoAcceptedRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "DuplicateAutoAcceptedRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			relationshipRequest.AutoAccept = true;

			Assert.Throws<ClientException>(() => _groupMemberClient.CreateMemberRequest(relationshipRequest));
		}

		[Test]
		public void CannotCreateRequestWithNonExistingUser()
		{
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CCRWNonExistingUser");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = -1,
				AcceptorId = acceptor.Id,
			};

			Assert.Throws<ClientException>(() => _groupMemberClient.CreateMemberRequest(relationshipRequest));
		}

		[Test]
		public void CannotCreateRequestWithNonExistingGroup()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "RequestWithNonExistingGroup");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = -1,
			};

			Assert.Throws<ClientException>(() => _groupMemberClient.CreateMemberRequest(relationshipRequest));
		}

		[Test]
		public void CanAcceptRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanAcceptRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanAcceptRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			var sent = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			var received = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(1, received.Count());

			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				Accepted = true
			};

			_groupMemberClient.UpdateMemberRequest(relationshipStatusUpdate);

			sent = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(0, sent.Count());

			received = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(0, received.Count());

			sent = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			received = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(1, received.Count());
		}

		[Test]
		public void CanRejectRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanRejectRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanRejectRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			var sent = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			var received = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(1, received.Count());

			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				Accepted = false
			};

			_groupMemberClient.UpdateMemberRequest(relationshipStatusUpdate);

			sent = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(0, sent.Count());

			received = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(0, received.Count());

			sent = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(0, sent.Count());

			received = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(0, received.Count());
		}

		[Test]
		public void CannotUpdateAlreadyAcceptedRequest()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CannotUpdateAlreadyAcceptedRequest");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CannotUpdateAlreadyAcceptedRequest");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				AutoAccept = true
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				Accepted = true
			};

			Assert.Throws<ClientException>(() => _groupMemberClient.UpdateMemberRequest(relationshipStatusUpdate));
		}

		[Test]
		public void CannotUpdateNotExistingRequest()
		{
			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = -1,
				AcceptorId = -1,
				Accepted = true
			};

			Assert.Throws<ClientException>(() => _groupMemberClient.UpdateMemberRequest(relationshipStatusUpdate));
		}

		[Test]
		public void CanUpdateRelationship()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanUpdateRelationship");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanUpdateRelationship");

			var relationshipRequest = new RelationshipRequest()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
				AutoAccept = true,
			};

			var relationshipResponse = _groupMemberClient.CreateMemberRequest(relationshipRequest);

			var sent = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(1, sent.Count());

			var received = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(1, received.Count());

			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
			};

			_groupMemberClient.UpdateMember(relationshipStatusUpdate);

			sent = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(0, sent.Count());

			received = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(0, received.Count());
		}

		[Test]
		public void CannotUpdateNotExistingRelationship()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CannotUpdateNotExistingRelationship");
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CannotUpdateNotExistingRelationship");

			var relationshipStatusUpdate = new RelationshipStatusUpdate()
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id,
			};

			Assert.Throws<ClientException>(() => _groupMemberClient.UpdateMember(relationshipStatusUpdate));
		}

		[Test]
		public void CanGetMemberRequests()
		{
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanGetMemberRequests");
			var requestorNames = new string[] {
				"CanGetMemberRequests1",
				"CanGetMemberRequests2",
				"CanGetMemberRequests3",
				"CanGetMemberRequests4",
				"CanGetMemberRequests5"
			};

			foreach (var name in requestorNames)
			{
				var requestor = Helpers.GetOrCreateUser(_userClient, name);
				var relationshipRequest = new RelationshipRequest() {
					RequestorId = requestor.Id,
					AcceptorId = acceptor.Id
				};
				_groupMemberClient.CreateMemberRequest(relationshipRequest);
			}

			var requests = _groupMemberClient.GetMemberRequests(acceptor.Id);

			Assert.AreEqual(5, requests.Count());

			var requestCheck = requests.Select(r => requestorNames.Contains(r.Name));

			Assert.AreEqual(5, requestCheck.Count());
		}

		[Test]
		public void CanGetSentRequests()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanGetSentRequests");
			var acceptorNames = new string[] {
				"CanGetSentRequests1",
				"CanGetSentRequests2",
				"CanGetSentRequests3",
				"CanGetSentRequests4",
				"CanGetSentRequests5"
			};

			foreach (var name in acceptorNames)
			{
				var acceptor = Helpers.GetOrCreateGroup(_groupClient, name);
				var relationshipRequest = new RelationshipRequest()
				{
					RequestorId = requestor.Id,
					AcceptorId = acceptor.Id
				};
				_groupMemberClient.CreateMemberRequest(relationshipRequest);
			}

			var requests = _groupMemberClient.GetSentRequests(requestor.Id);

			Assert.AreEqual(5, requests.Count());

			var requestCheck = requests.Select(r => acceptorNames.Contains(r.Name));

			Assert.AreEqual(5, requestCheck.Count());
		}

		[Test]
		public void CanGetMembers()
		{
			var acceptor = Helpers.GetOrCreateGroup(_groupClient, "CanGetMembers");
			var requestorNames = new string[] {
				"CanGetMembers1",
				"CanGetMembers2",
				"CanGetMembers3",
				"CanGetMembers4",
				"CanGetMembers5"
			};

			foreach (var name in requestorNames)
			{
				var requestor = Helpers.GetOrCreateUser(_userClient, name);
				var relationshipRequest = new RelationshipRequest()
				{
					RequestorId = requestor.Id,
					AcceptorId = acceptor.Id,
					AutoAccept = true
				};
				_groupMemberClient.CreateMemberRequest(relationshipRequest);
			}

			var members = _groupMemberClient.GetMembers(acceptor.Id);

			Assert.AreEqual(5, members.Count());

			var memberCheck = members.Select(r => requestorNames.Contains(r.Name));

			Assert.AreEqual(5, memberCheck.Count());
		}

		[Test]
		public void CanGetUserGroups()
		{
			var requestor = Helpers.GetOrCreateUser(_userClient, "CanGetUserGroups");
			var acceptorNames = new string[] {
				"CanGetUserGroups1",
				"CanGetUserGroups2",
				"CanGetUserGroups3",
				"CanGetUserGroups4",
				"CanGetUserGroups5"
			};

			foreach (var name in acceptorNames)
			{
				var acceptor = Helpers.GetOrCreateGroup(_groupClient, name);
				var relationshipRequest = new RelationshipRequest()
				{
					RequestorId = requestor.Id,
					AcceptorId = acceptor.Id,
					AutoAccept = true
				};
				_groupMemberClient.CreateMemberRequest(relationshipRequest);
			}

			var userGroups = _groupMemberClient.GetUserGroups(requestor.Id);

			Assert.AreEqual(5, userGroups.Count());

			var groupCheck = userGroups.Select(r => acceptorNames.Contains(r.Name));

			Assert.AreEqual(5, groupCheck.Count());
		}

		#endregion
	}
}
