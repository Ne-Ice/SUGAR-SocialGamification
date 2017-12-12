﻿using PlayGen.SUGAR.Server.Model.Interfaces;

namespace PlayGen.SUGAR.Server.Model
{
    public class ActorRelationshipRequest : IRelationship
    {
	    public int RequestorId { get; set; }

	    public Actor Requestor { get; set; }

	    public int AcceptorId { get; set; }

	    public Actor Acceptor { get; set; }
	}
}
