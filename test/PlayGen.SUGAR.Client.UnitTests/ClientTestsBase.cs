﻿using System;
using NUnit.Framework;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Client.UnitTests
{
    public class ClientTestsBase
    {
        protected readonly TestSUGARClient SUGARClient;

        protected AccountResponse LoggedInAccount { get; private set; }

        public ClientTestsBase()
        {
            SUGARClient = new TestSUGARClient();
        }

        [SetUp]
        public void Setup()
        {
            var accountRequest = new AccountRequest
            {
                Name = "admin",
                Password = "admin",
                SourceToken = "SUGAR",
            };

            try
            {
                LoggedInAccount = SUGARClient.Session.Login(accountRequest);
            }
            catch
            {
                LoggedInAccount = SUGARClient.Session.CreateAndLogin(accountRequest);
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                SUGARClient.Session.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
