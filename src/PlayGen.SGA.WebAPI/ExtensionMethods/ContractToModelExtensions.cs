﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using PlayGen.SGA.Contracts;
using PlayGen.SGA.DataModel;
using Newtonsoft.Json;

namespace PlayGen.SGA.WebAPI.ExtensionMethods
{
    public static class ContractToModelExtensions
    {
        public static Account ToModel(this AccountRequest accountContract)
        {
            return new Account
            {
                Name = accountContract.Name,
                PasswordHash = accountContract.Password
            };
        }

        public static Game ToModel(this GameRequest gameContract)
        {
            var gameModel = new Game();
            gameModel.Name = gameContract.Name;

            return gameModel;
        }

        public static User ToUserModel(this ActorRequest actorContract)
        {
            var actorModel = new User();
            actorModel.Name = actorContract.Name;

            return actorModel;
        }

        public static Group ToGroupModel(this ActorRequest actorContract)
        {
            var actorModel = new Group();
            actorModel.Name = actorContract.Name;

            return actorModel;
        }

        // TODO test
        public static UserAchievement ToUserModel(this AchievementRequest achieveContract)
        {
            var achieveModel = new UserAchievement();
            achieveModel.Name = achieveContract.Name;
            achieveModel.GameId = achieveContract.GameId;
            achieveModel.CompletionCriteriaCollection = achieveContract.CompletionCriteria.ToModel();

            return achieveModel;
        }

        // TODO test
        // TODO also set the completioncriteriacollection to convert to a contract and hook it up so it can be sent in a reponse if need be.
        public static CompletionCriteriaCollection ToModel(this List<AchievementCriteria> achievementContracts)
        {
            var achievementCollection = new CompletionCriteriaCollection();
            // Todo can probably do this with linq
            foreach (var achievementContract in achievementContracts)
            {
                achievementCollection.Add(achievementContract.ToModel());
            }

            return achievementCollection;
        }

        // TODO test - should also probably make the naming consistent - go with AchievemetnCriteria OR CompletionCriteria
        public static CompletionCriteria ToModel(this AchievementCriteria achievementContract)
        {
            return new CompletionCriteria
            {
                Key = achievementContract.Key,
                ComparisonType = (DataModel.ComparisonType) achievementContract.ComparisonType,
                DataType = (DataModel.DataType) achievementContract.DataType,
                Value = achievementContract.Value,
            };
        }

        // TODO test
        public static GroupAchievement ToGroupModel(this AchievementRequest achieveContract)
        {
            var achieveModel = new GroupAchievement();
            achieveModel.Name = achieveContract.Name;
            achieveModel.GameId = achieveContract.GameId;
            achieveModel.CompletionCriteriaCollection = achieveContract.CompletionCriteria.ToModel();

            return achieveModel;
        }

        public static UserToUserRelationship ToUserModel(this RelationshipRequest relationContract)
        {
            var relationModel = new UserToUserRelationship();
            relationModel.RequestorId = relationContract.RequestorId;
            relationModel.AcceptorId = relationContract.AcceptorId;

            return relationModel;
        }

        public static UserToGroupRelationship ToGroupModel(this RelationshipRequest relationContract)
        {
            var relationModel = new UserToGroupRelationship();
            relationModel.RequestorId = relationContract.RequestorId;
            relationModel.AcceptorId = relationContract.AcceptorId;

            return relationModel;
        }

        public static UserData ToUserModel(this SaveDataRequest dataContract)
        {
            var dataModel = new UserData();
            dataModel.UserId = dataContract.ActorId;
            dataModel.GameId = dataContract.GameId;
            dataModel.Key = dataContract.Key;
            dataModel.Value = dataContract.Value;
            dataModel.DataType = (DataModel.DataType)dataContract.DataType;

            return dataModel;
        }

        public static GroupData ToGroupModel(this SaveDataRequest dataContract)
        {
            var dataModel = new GroupData();
            dataModel.GroupId = dataContract.ActorId;
            dataModel.GameId = dataContract.GameId;
            dataModel.Key = dataContract.Key;
            dataModel.Value = dataContract.Value;
            dataModel.DataType = (DataModel.DataType)dataContract.DataType;

            return dataModel;
        }
    }
}
