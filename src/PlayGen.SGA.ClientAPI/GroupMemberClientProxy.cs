﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SGA.ClientAPI.Extensions;
using PlayGen.SGA.Contracts;
using PlayGen.SGA.Contracts.Controllers;

namespace PlayGen.SGA.ClientAPI
{
    public class GroupMemberClientProxy : ClientProxy, IGroupMemberController
    {
        public GroupMemberClientProxy(string baseAddress) : base(baseAddress)
        {
        }

        public IEnumerable<ActorResponse> GetMemberRequests(int groupId)
        {
            var query = GetUriBuilder("api/groupmember/requests")
                .AppendQueryParameters(new int[] { groupId }, "groupId={0}")
                .ToString();
            return Get<IEnumerable<ActorResponse>>(query);
        }

        public IEnumerable<ActorResponse> GetMembers(int groupId)
        {
            var query = GetUriBuilder("api/groupmember/members")
                .AppendQueryParameters(new int[] { groupId }, "groupId={0}")
                .ToString();
            return Get<IEnumerable<ActorResponse>>(query);
        }

        public RelationshipResponse CreateMemberRequest(RelationshipRequest relationship)
        {
            var query = GetUriBuilder("api/groupmember").ToString();
            return Post<RelationshipRequest, RelationshipResponse>(query, relationship);
        }


        public void UpdateMemberRequest(RelationshipStatusUpdate relationship)
        {
            var query = GetUriBuilder("api/groupmember/request").ToString();
            Put(query, relationship);
        }

        public void UpdateMember(RelationshipStatusUpdate relationship)
        {
            var query = GetUriBuilder("api/groupmember").ToString();
            Put(query, relationship);
        }
    }
}
