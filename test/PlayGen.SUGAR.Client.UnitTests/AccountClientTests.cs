﻿using System;
using PlayGen.SUGAR.Contracts;
using System.Net;
using NUnit.Framework;
using PlayGen.SUGAR.Client.Exceptions;
using PlayGen.SUGAR.Client.UnitTests;

namespace PlayGen.SUGAR.Client.IntegrationTests
{
    [TestFixture]
	public class AccountClientTests : ClientTestBase
	{
		#region Configuration
		private readonly AccountClient _accountClient;

		public AccountClientTests()
		{
			_accountClient = TestSugarClient.Account;
		}
		#endregion

		#region Tests
		[Test]
		public void CanRegisterNewUserAndLogin()
		{
			var accountRequest = new AccountRequest
			{
				Name = "CanRegisterNewUserAndLogin",
				Password = "CanRegisterNewUserAndLoginPassword",
				AutoLogin = true,
			};

			var registerResponse = _accountClient.Register(accountRequest);

			Assert.True(registerResponse.User.Id > 0);
			Assert.AreEqual(accountRequest.Name, registerResponse.User.Name);
		}
        /*
		[Test]
		public void CannotRegisterDuplicate()
		{
			var accountRequest = new AccountRequest
			{
				Name = "CannotRegisterDuplicate",
				Password = "CannotRegisterDuplicatePassword",
			};

			var registerResponse = _accountClient.Register(accountRequest);

			Assert.Throws<ClientException>(() => _accountClient.Register(accountRequest));
		}

		[Test]
		public void CannotRegisterInvalidUser()
		{
			var accountRequest = new AccountRequest();
			Assert.Throws<ClientException>(() => _accountClient.Register(accountRequest));
		}

		[Test]
		public void CanLoginValidUser()
		{
			var accountRequest = new AccountRequest
			{
				Name = "CanLoginValidUser",
				Password = "CanLoginValidUserPassword",
			};

			var registerResponse = _accountClient.Register(accountRequest);

			var logged = _accountClient.Login(accountRequest);

			Assert.True(logged.User.Id > 0);
			Assert.AreEqual(accountRequest.Name, logged.User.Name);
		}

		[Test]
		public void CannotLoginInvalidUser()
		{
			var accountRequest = new AccountRequest();
			Assert.Throws<ClientException>(() => _accountClient.Login(accountRequest));
		}*/
		#endregion
	}
}