﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGen.SUGAR.Authorization;
using PlayGen.SUGAR.Common.Shared.Permissions;
using PlayGen.SUGAR.WebAPI.Extensions;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.WebAPI.Attributes;

namespace PlayGen.SUGAR.WebAPI.Controllers
{
	/// <summary>
	/// Web Controller that facilitates User to User relationship specific operations.
	/// </summary>
	[Route("api/[controller]")]
    [Authorize("Bearer")]
    [ValidateSession]
    public class UserFriendController : Controller
	{
        private readonly IAuthorizationService _authorizationService;
        private readonly Core.Controllers.UserFriendController _userFriendCoreController;

		public UserFriendController(Core.Controllers.UserFriendController userFriendCoreController,
                    IAuthorizationService authorizationService)
		{
			_userFriendCoreController = userFriendCoreController;
            _authorizationService = authorizationService;
        }

		/// <summary>
		/// Get a list of all Users that have relationship requests for this <param name="userId"/>.
		/// 
		/// Example Usage: GET api/userfriend/requests/1
		/// </summary>
		/// <param name="userId">ID of the group.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("requests/{userId:int}")]
        //[ResponseType(typeof(IEnumerable<ActorResponse>))]
        [Authorization(ClaimScope.User, AuthorizationOperation.Get, AuthorizationOperation.UserFriendRequest)]
        public IActionResult GetFriendRequests([FromRoute]int userId)
		{
            if (_authorizationService.AuthorizeAsync(User, userId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var actor = _userFriendCoreController.GetFriendRequests(userId);
                var actorContract = actor.ToActorContractList();
                return new ObjectResult(actorContract);
            }
            return Forbid();
        }

		/// <summary>
		/// Get a list of all Users that have been sent relationship requests for this <param name="userId"/>.
		/// 
		/// Example Usage: GET api/userfriend/sentrequests/1
		/// </summary>
		/// <param name="userId">ID of the user.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("sentrequests/{userId:int}")]
        //[ResponseType(typeof(IEnumerable<ActorResponse>))]
        [Authorization(ClaimScope.User, AuthorizationOperation.Get, AuthorizationOperation.UserFriendRequest)]
        public IActionResult GetSentRequests([FromRoute]int userId)
		{
            if (_authorizationService.AuthorizeAsync(User, userId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var actor = _userFriendCoreController.GetSentRequests(userId);
                var actorContract = actor.ToActorContractList();
                return new ObjectResult(actorContract);
            }
            return Forbid();
        }

		/// <summary>
		/// Get a list of all Users that have relationships with this <param name="userId"/>.
		/// 
		/// Example Usage: GET api/userfriend/friends/1
		/// </summary>
		/// <param name="userId">ID of the user.</param>
		/// <returns>A list of <see cref="ActorResponse"/> which match the search criteria.</returns>
		[HttpGet("friends/{userId:int}")]
		//[ResponseType(typeof(IEnumerable<ActorResponse>))]
		public IActionResult GetFriends([FromRoute]int userId)
		{
			var actor = _userFriendCoreController.GetFriends(userId);
			var actorContract = actor.ToActorContractList();
			return new ObjectResult(actorContract);
		}

		/// <summary>
		/// Create a new relationship request between two Users.
		/// Requires a relationship between the two to not already exist.
		/// 
		/// Example Usage: POST api/userfriend
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipRequest"/> object that holds the details of the new relationship request.</param>
		/// <returns>A <see cref="RelationshipResponse"/> containing the new Relationship details.</returns>
		[HttpPost]
		//[ResponseType(typeof(RelationshipResponse))]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.User, AuthorizationOperation.Create, AuthorizationOperation.UserFriendRequest)]
        public IActionResult CreateFriendRequest([FromBody]RelationshipRequest relationship)
		{
            if (_authorizationService.AuthorizeAsync(User, relationship.RequestorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result ||
                _authorizationService.AuthorizeAsync(User, relationship.AcceptorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var request = relationship.ToGroupModel();
                _userFriendCoreController.CreateFriendRequest(relationship.ToUserModel(), relationship.AutoAccept);
                var relationshipContract = request.ToContract();
                return new ObjectResult(relationshipContract);
            }
            return Forbid();
        }

		/// <summary>
		/// Update an existing relationship request between <param name="relationship.RequestorId"/> and <param name="relationship.AcceptorId"/>.
		/// Requires the relationship request to already exist between the two Users.
		/// 
		/// Example Usage: PUT api/userfriend/request
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipStatusUpdate"/> object that holds the details of the relationship.</param>
		[HttpPut("request")]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.User, AuthorizationOperation.Update, AuthorizationOperation.UserFriendRequest)]
        public IActionResult UpdateFriendRequest([FromBody] RelationshipStatusUpdate relationship)
		{
            if (_authorizationService.AuthorizeAsync(User, relationship.RequestorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result ||
                _authorizationService.AuthorizeAsync(User, relationship.AcceptorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var relation = new RelationshipRequest
                {
                    RequestorId = relationship.RequestorId,
                    AcceptorId = relationship.AcceptorId
                };
                _userFriendCoreController.UpdateFriendRequest(relation.ToUserModel(), relationship.Accepted);
                return Ok();
            }
            return Forbid();
        }

		/// <summary>
		/// Update an existing relationship between <param name="relationship.RequestorId"/> and <param name="relationship.AcceptorId"/>.
		/// Requires the relationship to already exist between the two Users.
		/// 
		/// Example Usage: PUT api/userfriend
		/// </summary>
		/// <param name="relationship"><see cref="RelationshipStatusUpdate"/> object that holds the details of the relationship.</param>
		[HttpPut]
		[ArgumentsNotNull]
        [Authorization(ClaimScope.User, AuthorizationOperation.Delete, AuthorizationOperation.UserFriend)]
        public IActionResult UpdateFriend([FromBody] RelationshipStatusUpdate relationship)
		{
            if (_authorizationService.AuthorizeAsync(User, relationship.RequestorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result ||
                _authorizationService.AuthorizeAsync(User, relationship.AcceptorId, (AuthorizationRequirement)HttpContext.Items["Requirements"]).Result)
            {
                var relation = new RelationshipRequest
                {
                    RequestorId = relationship.RequestorId,
                    AcceptorId = relationship.AcceptorId
                };
                _userFriendCoreController.UpdateFriend(relation.ToUserModel());
                return Ok();
            }
            return Forbid();
        }
	}
}