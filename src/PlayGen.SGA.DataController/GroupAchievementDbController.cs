﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PlayGen.SGA.DataAccess;
using PlayGen.SGA.DataController.Exceptions;
using PlayGen.SGA.DataModel;

namespace PlayGen.SGA.DataController
{
    public class GroupAchievementDbController : DbController
    {
        public GroupAchievementDbController(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public IEnumerable<GroupAchievement> Get(int[] gameIds)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var achievements = context.GroupAchievements.Where(a => gameIds.Contains(a.GameId)).ToList();

                return achievements;
            }
        }

        public GroupAchievement Create(GroupAchievement newAchievement)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var hasConflicts = context.GroupAchievements.Any(a => a.Name == newAchievement.Name && a.GameId == newAchievement.GameId);

                if (hasConflicts)
                {
                    throw new DuplicateRecordException(string.Format("An achievement with the name {0} for this game already exists.", newAchievement.Name));
                }

                var gameExists = context.Games.Any(g => g.Id == newAchievement.GameId);

                if (!gameExists)
                {
                    throw new DuplicateRecordException(string.Format("The provided game does not exist."));
                }

                var achievement = newAchievement;
                context.GroupAchievements.Add(achievement);
                context.SaveChanges();

                return achievement;
            }
        }

        public void Delete(int[] id)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var achievement = context.GroupAchievements.Where(g => id.Contains(g.Id)).ToList();

                context.GroupAchievements.RemoveRange(achievement);
                context.SaveChanges();
            }
        }
    }
}
