﻿using System.Linq;
using PlayGen.SUGAR.Server.EntityFramework.Controllers;
using PlayGen.SUGAR.Server.EntityFramework.Exceptions;
using PlayGen.SUGAR.Server.Model;
using Xunit;

namespace PlayGen.SUGAR.Server.EntityFramework.Tests
{
	public class AccountControllerTests : EntityFrameworkTestBase
	{
		#region Configuration
		private readonly AccountController _accountController = ControllerLocator.AccountController;
		private readonly AccountSourceController _accountSourceController = ControllerLocator.AccountSourceController;
		private readonly UserController _userController = ControllerLocator.UserController;
		#endregion

		#region Tests
		[Fact]
		public void CreateAndGetAccount()
		{
			var name = "CreateAndGetAccount";
			string password = $"{name}Password";

			var source = CreateAccountSource(name);
			CreateAccount(name, password, source.Id);

			var accounts = _accountController.Get(new string[] { name }, source.Id);

			var matches = accounts.Count(a => a.Name == name);

			Assert.Equal(1, matches);
		}

		[Fact]
		public void CreateDuplicateAccount()
		{
			var name = "CreateDuplicateAccount";
			string password = $"{name}Password";

			var source = CreateAccountSource(name);
			CreateAccount(name, password, source.Id);

			Assert.Throws<DuplicateRecordException>(() => CreateAccount(name, password, source.Id));
		}

		[Fact]
		public void GetMultipleAccountsByName()
		{
			var names = new[]
			{
				"GetMultipleAccountsByName1",
				"GetMultipleAccountsByName2",
				"GetMultipleAccountsByName3",
				"GetMultipleAccountsByName4",
			};

			var source = CreateAccountSource("GetMultipleAccountsByName");

			foreach (var name in names)
			{
				CreateAccount(name, $"{name}Password", source.Id);
			}

			CreateAccount("GetMultipleAccountsByName_DontGetThis", "GetMultipleAccountsByName_DontGetThisPassword", source.Id);

			var accounts = _accountController.Get(names, source.Id);

			var matchingAccounts = accounts.Select(a => names.Contains(a.Name));

			Assert.Equal(names.Length, matchingAccounts.Count());
		}

		[Fact]
		public void GetNonExistingAccounts()
		{
			var source = CreateAccountSource("GetNonExistingAccounts");

			var accounts = _accountController.Get(new string[] { "GetNonExsitingAccounts" }, source.Id);

			Assert.Empty(accounts);
		}

		[Fact]
		public void DeleteExistingAccount()
		{
			var name = "DeleteExistingAccount";
			string password = $"{name}Password";

			var source = CreateAccountSource(name);

			var account = CreateAccount(name, password, source.Id);

			var accounts = _accountController.Get(new string[] { name }, source.Id);
			Assert.Equal(accounts.Count(), 1);
			Assert.Equal(accounts.ElementAt(0).Name, name);

			_accountController.Delete(account.Id);
			accounts = _accountController.Get(new string[] { name }, source.Id);

			Assert.Empty(accounts);
		}

		[Fact]
		public void DeleteNonExistingAccount()
		{
			Assert.Throws<MissingRecordException>(() => _accountController.Delete(-1));
		}
		#endregion

		#region Helpers
		private Account CreateAccount(string name, string password, int sourceId)
		{
			var user = CreateUser(name);

			var account = new Account {
				Name = name,
				Password = password,
				UserId = user.Id,
				User = user,
				AccountSourceId = sourceId
			};

			return _accountController.Create(account);
		}

		private AccountSource CreateAccountSource(string name)
		{
			var source = new AccountSource {
				Description = name,
				Token = name,
				RequiresPassword = true,
			};

			return _accountSourceController.Create(source);
		}

		private User CreateUser(string name)
		{
			var user = new User() {
				Name = name,
			};

			_userController.Create(user);

			return user;
		}
		#endregion
	}
}