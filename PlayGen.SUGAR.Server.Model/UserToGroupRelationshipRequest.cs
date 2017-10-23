﻿using PlayGen.SUGAR.Data.Model.Interfaces;

namespace PlayGen.SUGAR.Data.Model
{
	public class UserToGroupRelationshipRequest : IRelationship
	{
		public int RequestorId { get; set; }

		public User Requestor { get; set; }

		public int AcceptorId { get; set; }

		public Group Acceptor { get; set; }
	}
}
