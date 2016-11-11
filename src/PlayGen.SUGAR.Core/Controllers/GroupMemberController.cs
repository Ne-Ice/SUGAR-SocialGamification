﻿using PlayGen.SUGAR.Data.Model;
using System.Collections.Generic;

namespace PlayGen.SUGAR.Core.Controllers
{
    public class GroupMemberController
    {
        private readonly Data.EntityFramework.Controllers.GroupRelationshipController _groupRelationshipDbController;

        public GroupMemberController(Data.EntityFramework.Controllers.GroupRelationshipController groupRelationshipDbController)
        {
            _groupRelationshipDbController = groupRelationshipDbController;
        }
        
        public IEnumerable<User> GetMemberRequests(int groupId)
        {
            var requestingMembers = _groupRelationshipDbController.GetRequests(groupId);
            return requestingMembers;
        }
        
        public IEnumerable<Group> GetSentRequests(int userId)
        {
            var requestedGroups = _groupRelationshipDbController.GetSentRequests(userId);
            return requestedGroups;
        }
        
        public IEnumerable<User> GetMembers(int groupId)
        {
            var members = _groupRelationshipDbController.GetMembers(groupId);
            return members;
        }
        
        public IEnumerable<Group> GetUserGroups(int userId)
        {
            var membershipGroups = _groupRelationshipDbController.GetUserGroups(userId);
            return membershipGroups;
        }
        
        public UserToGroupRelationship CreateMemberRequest(UserToGroupRelationship newRelationship, bool autoAccept)
        {
            newRelationship =  _groupRelationshipDbController.Create(newRelationship, autoAccept);
            return newRelationship;
        }
      
        public void UpdateMemberRequest(UserToGroupRelationship relationship, bool autoAccept)
        {
            _groupRelationshipDbController.UpdateRequest(relationship, autoAccept);
        }
      
        public void UpdateMember(UserToGroupRelationship relationship)
        {
            _groupRelationshipDbController.Update(relationship);
        }
    }
}