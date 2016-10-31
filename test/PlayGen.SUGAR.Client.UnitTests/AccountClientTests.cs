﻿using System;
using PlayGen.SUGAR.Contracts;
using System.Net;
using NUnit.Framework;

namespace PlayGen.SUGAR.Client.IntegrationTests
{
	public class AccountClientTests
	{
		#region Configuration
		private readonly AccountClient _accountClient;

		public AccountClientTests()
		{
			var testSugarClient = new TestSUGARClient();
			_accountClient = testSugarClient.Account;
		}
		#endregion

		#region Tests
		[Test]
		public void CanRegisterNewUser()
		{
			var accountRequest = new AccountRequest
			{
				Name = "CanRegisterNewUser",
				Password = "CanRegisterNewUserPassword",
				AutoLogin = false,
			};

			var registerResponse = _accountClient.Register(accountRequest);

			Assert.True(registerResponse.User.Id > 0);
			Assert.AreEqual(accountRequest.Name, registerResponse.User.Name);
		}

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

		[Test]
		public void CannotRegisterDuplicate()
		{
			var accountRequest = new AccountRequest
			{
				Name = "CannotRegisterDuplicate",
				Password = "CannotRegisterDuplicatePassword",
			};

			var registerResponse = _accountClient.Register(accountRequest);

			Assert.Throws<Exception>(() => _accountClient.Register(accountRequest));
		}

		[Test]
		public void CannotRegisterInvalidUser()
		{
			var accountRequest = new AccountRequest();
			Assert.Throws<Exception>(() => _accountClient.Register(accountRequest));
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
			Assert.Throws<Exception>(() => _accountClient.Login(accountRequest));
		}
		#endregion
	}
}
