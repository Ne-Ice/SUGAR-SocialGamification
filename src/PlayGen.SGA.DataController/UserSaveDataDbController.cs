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
    public class UserSaveDataDbController : DbController
    {
        public UserSaveDataDbController(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public IEnumerable<UserData> Get(int gameId, int userId, string[] keys)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var data = context.UserDatas.Where(d => d.UserId == userId && d.GameId == gameId && keys.Contains(d.Key)).ToList();

                return data;
            }
        }

        public UserData Create(UserData data)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var actorExists = context.Users.Any(u => u.Id == data.UserId);
                var gameExists = context.Games.Any(g => g.Id == data.GameId);

                if (!actorExists)
                {
                    throw new MissingRecordException(string.Format("The provided user does not exist."));
                }

                if (!gameExists)
                {
                    throw new MissingRecordException(string.Format("The provided game does not exist."));
                }

                if (context.Entry(data.User).State == EntityState.Detached)
                {
                    context.Users.Attach(data.User);
                }

                if (context.Entry(data.Game).State == EntityState.Detached)
                {
                    context.Games.Attach(data.Game);
                }

                context.UserDatas.Add(data);
                SaveChanges(context);
                return data;
            }
        }
    }
}
