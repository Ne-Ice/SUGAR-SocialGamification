﻿using System.Diagnostics;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

namespace PlayGen.SUGAR.Client.UnitTests
{
    public static class Helpers
    {
        public static AccountResponse LoggedInAccount { get; private set; }

        public static void RegisterAndLogin(AccountClient client)
        {
            var accountRequest = new AccountRequest
            {
                Name = "admin",
                Password = "admin",
                SourceToken = "SUGAR",
                AutoLogin = true,
            };

            try
            {
                LoggedInAccount = client.Login(accountRequest);
            }
            catch
            {
                LoggedInAccount = client.Register(accountRequest);
            }
        }

        public static ActorResponse GetOrCreateUser(UserClient userClient, string suffix = null)
        {
            var name = GetCallingClassName() + (suffix ?? $"_{suffix}") + "_User";// "_User" suffix as temporary fix while lack of TPH in EF.Core means types inheriting from Actor cannot have the same "Name" value.

            var users = userClient.Get(name, true);
            ActorResponse user;

            if (users.Any())
            {
                user = users.Single();
            }
            else
            {
                user = userClient.Create(new ActorRequest
                {
                    Name = name
                });
            }

            return user;
        }

        public static ActorResponse GetOrCreateGroup(GroupClient groupClient, string suffix = null)
        {
            var name = GetCallingClassName() + (suffix ?? $"_{suffix}") + "_Group"; // "_Group" suffix as temporary fix while lack of TPH in EF.Core means types inheriting from Actor cannot have the same "Name" value.

            var groups = groupClient.Get(name);
            ActorResponse group;

            if (groups.Any())
            {
                group = groups.Single();
            }
            else
            {
                group = groupClient.Create(new ActorRequest
                {
                    Name = name
                });
            }

            return group;
        }

        public static GameResponse GetOrCreateGame(GameClient gameClient, string suffix = null)
        {
            var name = GetCallingClassName() + suffix ?? $"_{suffix}";

            var games = gameClient.Get(name);
            GameResponse game;

            if (games.Any())
            {
                game = games.Single();
            }
            else
            {
                game = gameClient.Create(new GameRequest
                {
                    Name = name
                });
            }

            return game;
        }

        private static string GetCallingClassName()
        {
            var method = new StackTrace().GetFrame(2).GetMethod();
            var className = method.ReflectedType.Name;
            return className;
        }
    }
}
