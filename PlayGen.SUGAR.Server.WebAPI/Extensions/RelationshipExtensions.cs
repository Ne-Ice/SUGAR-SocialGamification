﻿using System.Diagnostics.CodeAnalysis;

using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.WebAPI.Extensions
{
	// Values ensured to not be nulled by model validation
	[SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
	public static class RelationshipExtensions
	{
		public static RelationshipResponse ToContract(this UserToGroupRelationship relationshipModel)
		{
			if (relationshipModel == null)
			{
				return null;
			}

			return new RelationshipResponse {
				RequestorId = relationshipModel.RequestorId,
				AcceptorId = relationshipModel.AcceptorId
			};
		}

		public static RelationshipResponse ToContract(this UserToUserRelationship relationshipModel)
		{
			if (relationshipModel == null)
			{
				return null;
			}

			return new RelationshipResponse {
				RequestorId = relationshipModel.RequestorId,
				AcceptorId = relationshipModel.AcceptorId
			};
		}


		public static UserToUserRelationship ToUserModel(this RelationshipRequest relationContract)
		{
			return new UserToUserRelationship {
				RequestorId = relationContract.RequestorId.Value,
				AcceptorId = relationContract.AcceptorId.Value
			};
		}

		public static UserToGroupRelationship ToGroupModel(this RelationshipRequest relationContract)
		{
			return new UserToGroupRelationship {
				RequestorId = relationContract.RequestorId.Value,
				AcceptorId = relationContract.AcceptorId.Value
			};
		}
	}
}