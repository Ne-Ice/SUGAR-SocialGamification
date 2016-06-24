﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayGen.SUGAR.Data.EntityFramework.Controllers;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using Xunit;

namespace PlayGen.SUGAR.Data.EntityFramework.UnitTests
{
	public class GroupMemberControllerTests : TestController
	{

		#region Configuration
		private readonly GroupRelationshipController _groupMemberDbController;

		public GroupMemberControllerTests()
		{
			_groupMemberDbController = new GroupRelationshipController(NameOrConnectionString);
		}
		#endregion


		#region Tests
		[Fact]
		public void CreateAndGetGroupMemberRequest()
		{
			string groupMemberName = "CreateGroupMemberRequest";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			var groupRequests = _groupMemberDbController.GetRequests(newMember.AcceptorId);

			int matches = groupRequests.Count(g => g.Name == groupMemberName + " Requestor");

			Assert.Equal(matches, 1);
		}

		[Fact]
		public void CreateGroupMemberWithNonExistingRequestor()
		{
			string groupMemberName = "CreateGroupMemberWithNonExistingRequestor";
			var acceptor = CreateGroup(groupMemberName);
			bool hadException = false;

			try
			{
				CreateGroupMember(-1, acceptor.Id);
			}
			catch (DuplicateRecordException)
			{
				hadException = true;
			}

			Assert.True(hadException);
		}

		[Fact]
		public void CreateGroupMemberWithNonExistingAcceptor()
		{
			string groupMemberName = "CreateGroupMemberWithNonExistingAcceptor";
			var requestor = CreateUser(groupMemberName);
			bool hadException = false;

			try
			{
				CreateGroupMember(requestor.Id, -1);
			}
			catch (DuplicateRecordException)
			{
				hadException = true;
			}

			Assert.True(hadException);
		}

		[Fact]
		public void CreateDuplicateGroupMember()
		{
			string groupMemberName = "CreateDuplicateGroupMember";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			bool hadDuplicateException = false;

			try
			{
				CreateGroupMember(requestor.Id, acceptor.Id);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}

		[Fact]
		public void CreateDuplicateReversedGroupMember()
		{
			string groupMemberName = "CreateDuplicateReversedGroupMember";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			bool hadDuplicateException = false;

			try
			{
				CreateGroupMember(acceptor.Id, requestor.Id);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}

		[Fact]
		public void GetNonExistingGroupMemberRequests()
		{
			var groupMembers = _groupMemberDbController.GetRequests(-1);

			Assert.Empty(groupMembers);
		}

		[Fact]
		public void AcceptGroupMemberRequest()
		{
			string groupMemberName = "AcceptGroupMemberRequest";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			_groupMemberDbController.UpdateRequest(newMember, true);

			var groupRequests = _groupMemberDbController.GetRequests(newMember.AcceptorId);

			int matches = groupRequests.Count(g => g.Name == groupMemberName + " Requestor");

			Assert.Equal(matches, 0);

			var groupMembers = _groupMemberDbController.GetMembers(newMember.AcceptorId);

			matches = groupMembers.Count(g => g.Name == groupMemberName + " Requestor");

			Assert.Equal(matches, 1);
		}

		[Fact]
		public void RejectGroupMemberRequest()
		{
			string groupMemberName = "RejectGroupMemberRequest";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			_groupMemberDbController.UpdateRequest(newMember, false);

			var groupRequests = _groupMemberDbController.GetRequests(newMember.AcceptorId);

			int matches = groupRequests.Count(g => g.Name == groupMemberName + " Requestor");

			Assert.Equal(matches, 0);

			var groupMembers = _groupMemberDbController.GetMembers(newMember.RequestorId);

			matches = groupMembers.Count(g => g.Name == groupMemberName + " Acceptor");

			Assert.Equal(matches, 0);
		}

		[Fact]
		public void UpdateNonExistingGroupMemberRequest()
		{
			string groupMemberName = "UpdateNonExistingGroupMemberRequest";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = new UserToGroupRelationship
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			bool hadException = false;

			try
			{
				_groupMemberDbController.UpdateRequest(newMember, true);
			}
			catch (Exception)
			{
				hadException = true;
			}

			Assert.True(hadException);
		}

		[Fact]
		public void GetNonExistingGroupMembers()
		{
			var groupMembers = _groupMemberDbController.GetMembers(-1);

			Assert.Empty(groupMembers);
		}

		[Fact]
		public void CreateDuplicateAcceptedGroupMember()
		{
			string groupMemberName = "CreateDuplicateAcceptedGroupMember";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			_groupMemberDbController.UpdateRequest(newMember, true);

			bool hadDuplicateException = false;

			try
			{
				CreateGroupMember(requestor.Id, acceptor.Id);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}

		[Fact]
		public void CreateDuplicateReversedAcceptedGroupMember()
		{
			string groupMemberName = "CreateDuplicateReversedAcceptedGroupMember";

			var requestor = CreateUser(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			_groupMemberDbController.UpdateRequest(newMember, true);

			bool hadDuplicateException = false;

			try
			{
				CreateGroupMember(acceptor.Id, requestor.Id);
			}
			catch (DuplicateRecordException)
			{
				hadDuplicateException = true;
			}

			Assert.True(hadDuplicateException);
		}

		[Fact]
		public void UpdateGroupMember()
		{
			string groupMemberName = "UpdateGroupMember";

			var requestor = CreateGroup(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = CreateGroupMember(requestor.Id, acceptor.Id);

			_groupMemberDbController.UpdateRequest(newMember, true);

			_groupMemberDbController.Update(newMember);
			var members = _groupMemberDbController.GetMembers(acceptor.Id);

			Assert.Empty(members);
		}

		[Fact]
		public void UpdateNonExistingGroupMember()
		{
			string groupMemberName = "UpdateNonExistingGroupMember";

			var requestor = CreateGroup(groupMemberName + " Requestor");
			var acceptor = CreateGroup(groupMemberName + " Acceptor");

			var newMember = new UserToGroupRelationship
			{
				RequestorId = requestor.Id,
				AcceptorId = acceptor.Id
			};

			bool hadException = false;

			try
			{
				_groupMemberDbController.Update(newMember);
			}
			catch (Exception)
			{
				hadException = true;
			}

			Assert.True(hadException);
		}
		#endregion

		#region Helpers
		private User CreateUser(string name)
		{
			UserController userDbController = new UserController(NameOrConnectionString);
			var user = new User
			{
				Name = name,
			};

			userDbController.Create(user);

			return user;
		}

		private Group CreateGroup(string name)
		{
			GroupController groupDbController = new GroupController(NameOrConnectionString);
			var group = new Group
			{
				Name = name,
			};
			groupDbController.Create(group);

			return group;
		}

		private UserToGroupRelationship CreateGroupMember(int requestor, int acceptor)
		{
			var groupMember = new UserToGroupRelationship
			{
				RequestorId = requestor,
				AcceptorId = acceptor
			};
			_groupMemberDbController.Create(groupMember, false);

			return groupMember;
		}
		#endregion
	}
}