﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Data.EntityFramework;
using PlayGen.SUGAR.Contracts.Controllers;
using PlayGen.SUGAR.WebAPI.ExtensionMethods;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.WebAPI.Exceptions;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates User to Group relationship specific operations.
	/// </summary>
	[Route("api/[controller]")]
	public class GroupMemberController : Controller, IGroupMemberController
	{
		private readonly Data.EntityFramework.Controllers.GroupRelationshipController _groupRelationshipController;

		public GroupMemberController(Data.EntityFramework.Controllers.GroupRelationshipController groupRelationshipController)
		{
			_groupRelationshipController = groupRelationshipController;
		}

		/// <summary>
		/// Get a list of all Users that have relationship requests for this <param name="groupId"/>.
		/// 
		/// Example Usage: GET api/groupmember/requests?groupId=1
		/// </summary>
		/// <param name="groupId">ID of the group.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("requests")]
		public IEnumerable<ActorResponse> GetMemberRequests(int groupId)
		{
			var actor = _groupRelationshipController.GetRequests(groupId);
			var actorContract = actor.ToContract();
			return actorContract;
		}

		/// <summary>
		/// Get a list of all Groups that have been sent relationship requests for this <param name="userId"/>.
		/// 
		/// Example Usage: GET api/groupmember/sentrequests?userId=1
		/// </summary>
		/// <param name="userId">ID of the user.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("sentrequests")]
		public IEnumerable<ActorResponse> GetSentRequests(int userId)
		{
			var actor = _groupRelationshipController.GetSentRequests(userId);
			var actorContract = actor.ToContract();
			return actorContract;
		}

		/// <summary>
		/// Get a list of all Users that have relationships with this <param name="groupId"/>.
		/// 
		/// Example Usage: GET api/groupmember/members?groupId=1
		/// </summary>
		/// <param name="groupId">ID of the group.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("members")]
		public IEnumerable<ActorResponse> GetMembers(int groupId)
		{
			var actor = _groupRelationshipController.GetMembers(groupId);
			var actorContract = actor.ToContract();
			return actorContract;
		}

		/// <summary>
		/// Get a list of all Groups that have relationships with this <param name="userId"/>.
		/// 
		/// Example Usage: GET api/groupmember/usergroups?userId=1
		/// </summary>
		/// <param name="userId">ID of the User.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("usergroups")]
		public IEnumerable<ActorResponse> GetUserGroups(int userId)
		{
			var actor = _groupRelationshipController.GetUserGroups(userId);
			var actorContract = actor.ToContract();
			return actorContract;
		}

		/// <summary>
		/// Create a new relationship request between the User and Group.
		/// Requires a relationship between the User and Group to not already exist.
		/// 
		/// Example Usage: POST api/groupmember
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipRequest"/> object that holds the details of the new relationship request.</param>
		/// <returns>A <see cref="RelationshipResponse"/> containing the new Relationship details.</returns>
		[HttpPost]
		public RelationshipResponse CreateMemberRequest([FromBody]RelationshipRequest relationship)
		{
			if (relationship == null)
			{
				throw new NullObjectException("Invalid object passed");
			}
			var request = relationship.ToGroupModel();
			_groupRelationshipController.Create(relationship.ToGroupModel(), relationship.AutoAccept);
			var relationshipContract = request.ToContract();
			return relationshipContract;
		}

		/// <summary>
		/// Update an existing relationship request between <param name="relationship.UserId"/> and <param name="relationship.GroupId"/>.
		/// Requires the relationship request to already exist between the User and Group.
		/// 
		/// Example Usage: PUT api/groupmember/request
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipStatusUpdate"/> object that holds the details of the relationship.</param>
		[HttpPut("request")]
		public void UpdateMemberRequest([FromBody] RelationshipStatusUpdate relationship)
		{
			var relation = new RelationshipRequest {
				RequestorId = relationship.RequestorId,
				AcceptorId = relationship.AcceptorId
			};
			_groupRelationshipController.UpdateRequest(relation.ToGroupModel(), relationship.Accepted);
		}

		/// <summary>
		/// Update an existing relationship between <param name="relationship.UserId"/> and <param name="relationship.GroupId"/>.
		/// Requires the relationship to already exist between the User and Group.
		/// 
		/// Example Usage: PUT api/groupmember
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipStatusUpdate"/> object that holds the details of the relationship.</param>
		[HttpPut]
		public void UpdateMember([FromBody] RelationshipStatusUpdate relationship)
		{
			var relation = new RelationshipRequest
			{
				RequestorId = relationship.RequestorId,
				AcceptorId = relationship.AcceptorId
			};
			_groupRelationshipController.Update(relation.ToGroupModel());
		}
	}
}