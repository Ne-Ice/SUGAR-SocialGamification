﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Server.Core.Authorization;
using PlayGen.SUGAR.Server.EntityFramework;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.Core.Controllers
{
    public class RelationshipController
    {
	    private readonly ILogger _logger;
	    private readonly EntityFramework.Controllers.RelationshipController _relationshipDbController;
		private readonly EntityFramework.Controllers.ActorClaimController _actorClaimController;
		private readonly ActorRoleController _actorRoleController;
		private readonly EntityFramework.Controllers.ActorController _actorController;
	    private readonly EntityFramework.Controllers.ClaimController _claimController;

		public RelationshipController(
		    ILogger<RelationshipController> logger,
		    EntityFramework.Controllers.ActorClaimController actorClaimController,
		    ActorRoleController actorRoleController,
			EntityFramework.Controllers.ActorController actorController,
		    EntityFramework.Controllers.ClaimController claimController,
			EntityFramework.Controllers.RelationshipController relationshipDbController)
		{
		    _logger = logger;
			_actorClaimController = actorClaimController;
			_actorRoleController = actorRoleController;
			_actorController = actorController;
			_claimController = claimController;
		    _relationshipDbController = relationshipDbController;
		}

		/// <summary>
		/// Get relationship requests from an actor type to an actor
		/// </summary>
		/// <param name="actorId">The recipient of the request</param>
		/// <param name="fromActorType">The actor type that sent the request</param>
		/// <returns></returns>
		public List<Actor> GetRequests(int actorId, ActorType fromActorType)
	    {
		    var requesting = _relationshipDbController.GetRequests(actorId, fromActorType);

		    _logger.LogInformation($"{requesting?.Count} Requests for ActorId: {actorId}");

		    return requesting;
	    }

		/// <summary>
		/// Get relationship requests that an actor has sent to an actor type
		/// </summary>
		/// <param name="actorId">The actor who has sent requests</param>
		/// <param name="toActorType">The actor type that has received the requests</param>
		/// <returns></returns>
	    public List<Actor> GetSentRequests(int actorId, ActorType toActorType)
	    {
		    var requests = _relationshipDbController.GetSentRequests(actorId, toActorType);

		    _logger.LogInformation($"{requests?.Count} Sent Requests for ActorId: {requests}");

		    return requests;
	    }

		/// <summary>
		/// Get relationships shared between an actor and other actor types
		/// </summary>
		/// <param name="actorId">The actor to get list of relationsips with</param>
		/// <param name="actorType">The tyoe of actor that relationship is shared with</param>
		/// <returns></returns>
	    public List<Actor> GetRelationships(int actorId, ActorType actorType, int? requestingId)
	    {
		    var relationships = _relationshipDbController.GetRelationships(actorId, actorType);

		    // ReSharper disable once SimplifyLinqExpression
		    if (requestingId == null || !_actorRoleController.GetControlled(requestingId.Value).Any(c => c.ClaimScope == ClaimScope.Global))
		    {
			    relationships = relationships.Where(r => !r.Private || r.Id == requestingId).ToList();
		    }

			_logger.LogInformation($"{relationships?.Count} relationships for ActorId: {actorId}");

		    return relationships;
	    }

		/// <summary>
		/// Get a count of relationships shared between an actor and other actor types
		/// </summary>
		/// <param name="actorId">The actor to get a list of relationships with</param>
		/// <param name="actorType">The type of actor that relationship is shared with</param>
		/// <returns></returns>
	    public int GetRelationshipCount(int actorId, ActorType actorType)
	    {
		    var count = _relationshipDbController.GetRelationshipCount(actorId, actorType);
		    return count;
	    }

	    /// <summary>
	    /// Create a new relationship between actores
	    /// </summary>
	    /// <param name="newRelationship"></param>
	    /// <param name="autoAccept">If the relationship is accepted immediately</param>
	    /// <param name="context">Optional DbContext to perform opperations on. If ommitted a DbContext will be created.</param>
	    public void CreateRequest(ActorRelationship newRelationship, bool autoAccept, SUGARContext context = null)
	    {
			// HACK auto accept default to false when using SUGAR Unity 1.0.2 or prior, not the expected behaviour
		    // autoAccept = true;
			// END HACK
		    if (autoAccept)
		    {
			    _relationshipDbController.CreateRelationship(newRelationship, context);
				// Assign users claims to the group that they do not get by default
			    AssignUserResourceClaims(newRelationship);
		    }
		    else
		    {
			    _relationshipDbController.CreateRelationshipRequest(newRelationship, context);
		    }

		    _logger.LogInformation($"{newRelationship?.RequestorId} -> {newRelationship?.AcceptorId}, Auto Accept: {autoAccept}");
	    }

	    /// <summary>
	    /// Update an existing relationship request between actors
	    /// </summary>
	    /// <param name="newRelationship"></param>
	    /// <param name="accepted">If the relationship has been accepted by one of the actors</param>
		public void UpdateRequest(ActorRelationship relationship, bool accepted)
	    {
		    _relationshipDbController.UpdateRequest(relationship, accepted);

		    _logger.LogInformation($"{relationship?.RequestorId} -> {relationship?.AcceptorId}, Accepted: {accepted}");
	    }

		/// <summary>
		/// Update an existing relationship between actors
		/// </summary>
		/// <param name="relationship"></param>
	    public void Update(ActorRelationship relationship)
	    {
			//todo Check if user is only group admin
		    _relationshipDbController.Update(relationship);
		    //todo Remove ActorRole for group if user has permissions

		    _logger.LogInformation($"{relationship?.RequestorId} -> {relationship?.AcceptorId}");
	    }

		// TODO This is assigning new users default claims to the group, to be moved to its own table
		/// <summary>
		/// Assign the user claims to resources for a newly created relationship with a group
		/// </summary>
		/// <param name="relation">the user/group relationship</param>
	    private void AssignUserResourceClaims(ActorRelationship relation)
	    {
		    relation.Requestor = _actorController.Get(relation.RequestorId);
		    relation.Acceptor = _actorController.Get(relation.AcceptorId);
			// Group to user relationship
		    if (relation.Requestor.ActorType == ActorType.Group && relation.Acceptor.ActorType == ActorType.User || relation.Acceptor.ActorType == ActorType.Group && relation.Requestor.ActorType == ActorType.User)
		    {
			    // Get user
			    var user = relation.Requestor.ActorType == ActorType.User
				    ? relation.Requestor
				    : relation.Acceptor;

			    var group = relation.Requestor.ActorType == ActorType.Group
				    ? relation.Requestor
				    : relation.Acceptor;

			    var GetClaim = _claimController.Get(ClaimScope.Group, "Get-Resource");
			    var CreateClaim = _claimController.Get(ClaimScope.Group, "Create-Resource");
			    var UpdateClaim = _claimController.Get(ClaimScope.Group, "Update-Resource");
			    if (GetClaim != null)
			    {
				    var getActorClaim = new ActorClaim
				    {
					    ActorId = user.Id,
					    ClaimId = GetClaim.Id,
					    EntityId = group.Id,
				    };
				    _actorClaimController.Create(getActorClaim);
			    }
			    if (UpdateClaim != null)
			    {
				    var updateActorClaim = new ActorClaim
				    {
					    ActorId = user.Id,
					    ClaimId = UpdateClaim.Id,
					    EntityId = group.Id,
				    };
				    _actorClaimController.Create(updateActorClaim);

				}
				if (CreateClaim != null)
			    {
				    var createActorClaim = new ActorClaim
				    {
					    ActorId = user.Id,
					    ClaimId = CreateClaim.Id,
					    EntityId = group.Id,
				    };
				    _actorClaimController.Create(createActorClaim);
			    }
		    }
		}
	}
}
